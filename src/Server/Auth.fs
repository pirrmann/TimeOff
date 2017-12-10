namespace TimeOff.Server

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

type Authentifier (jwtEncoder: JsonWebTokenEncoder) =
    /// Login web part that authenticates a user and returns a token in the HTTP body.
    member __.LoginWebPart (ctx: HttpContext) = async {
        let login = 
            ctx.request.rawForm 
            |> System.Text.Encoding.UTF8.GetString
            |> fromJson<Shared.Types.Login>

        match UserDirectory.authenticate login with
        | Some userRights ->
            let token = jwtEncoder.CreateToken userRights
            let authSuccess = { Role = userRights.Role; Token = token }
            return! JSON authSuccess ctx
        | _ -> return! UNAUTHORIZED (sprintf "User '%s' can't be logged in." login.UserName) ctx
    }

    /// Extracts the user rights from the token and stores them in the context
    member __.Authenticate ctx = async {
        match ctx.request.header "Authorization" with
        | Choice1Of2 accesstoken when accesstoken.StartsWith "Bearer " -> 
            let jwt = accesstoken.Replace("Bearer ","")
            match jwtEncoder.Validate jwt with
            | Some userRights ->
                return Some { ctx with userState = ctx.userState.Add("UserRights", userRights) }
            | _ ->
                return! FORBIDDEN "Accessing this API is not allowed" ctx
        | _ ->
            return! BAD_REQUEST "Request doesn't contain a JSON Web Token" ctx
    }

module Authorization =
    let getUserRights ctx =
        ctx.userState.TryFind "UserRights"
        |> Option.bind (function | :? UserRights as rights -> Some rights | _ -> None)
        
    let withUserRights f ctx = async {
        match getUserRights ctx with
        | Some userRights -> return! f userRights ctx
        | _ -> return! BAD_REQUEST "Request doesn't contain a JSON Web Token" ctx }

    let whenUserHasRole role ctx = async {
        match getUserRights ctx with
        | Some userRights when userRights.Role = role -> return Some ctx
        | _ -> return! FORBIDDEN "Accessing this API is not allowed" ctx }
