module TimeOff.Server.Auth

open Shared.Types

open Suave
open RequestErrors

// This is a fake user directory, it could be an LDAP, ActiveDirectory or any custom list of credentials and roles
// We will not handle password for now, and assume they have to be the same as the login
module private UserDirectory =
    let private users =
        [
            "employee1", Employee
            "employee2", Employee
            "manager", Manager
            "hr", HumanResources
        ] |> Map.ofList

    let authenticate (login:Login) =
        match Map.tryFind login.UserName users with
        | Some role when login.UserName = login.Password ->
            let userRights : UserRights = { UserName = login.UserName; Role= role }
            Some userRights
        | _ ->
            None

/// Login web part that authenticates a user and returns a token in the HTTP body.
let login (ctx: HttpContext) = async {
    let login = 
        ctx.request.rawForm 
        |> System.Text.Encoding.UTF8.GetString
        |> fromJson<Shared.Types.Login>

    match UserDirectory.authenticate login with
    | Some userRights ->
        let token = JsonWebToken.encode userRights
        let authSuccess = { Role = userRights.Role; Token = token }
        return! JSON authSuccess ctx
    | _ -> return! UNAUTHORIZED (sprintf "User '%s' can't be logged in." login.UserName) ctx
}

let private useTokenWithCheck check f ctx = async {
    match ctx.request.header "Authorization" with
    | Choice1Of2 accesstoken when accesstoken.StartsWith "Bearer " -> 
        let jwt = accesstoken.Replace("Bearer ","")
        match check, JsonWebToken.isValid jwt with
        | None, Some token  -> // This webpart just requires that you're authenticated
            return! f token ctx
        | Some check, Some token when check token -> // This webpart requires authentication and some authorization
            return! f token ctx
        | _ ->
            return! FORBIDDEN "Accessing this API is not allowed" ctx
    | _ ->
        return! BAD_REQUEST "Request doesn't contain a JSON Web Token" ctx
}

/// Invokes a function that produces the output for a web part if the HttpContext
/// contains a valid auth token with the requested Role.
let useTokenWithRole role = useTokenWithCheck (Some (fun rights -> rights.Role = role))

/// Invokes a function that produces the output for a web part if the HttpContext
/// contains a valid auth token.
let useToken = useTokenWithCheck None
