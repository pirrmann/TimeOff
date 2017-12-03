module TimeOff.Server.Auth

open Shared.Types

open Suave
open Suave.RequestErrors

/// Login web part that authenticates a user and returns a token in the HTTP body.
let login (ctx: HttpContext) = async {
    let login = 
        ctx.request.rawForm 
        |> System.Text.Encoding.UTF8.GetString
        |> fromJson<Shared.Types.Login>

    try
        if ((login.UserName <> "employee1" &&
             login.UserName <> "employee2" &&
             login.UserName <> "manager" &&
             login.UserName <> "hr")
            || login.Password <> login.UserName) then
            return! failwithf "Could not authenticate %s" login.UserName

        let user : UserRights = { UserName = login.UserName }
        let token = JsonWebToken.encode user

        return! Successful.OK token ctx
    with
    | _ -> return! UNAUTHORIZED (sprintf "User '%s' can't be logged in." login.UserName) ctx
}

/// Invokes a function that produces the output for a web part if the HttpContext
/// contains a valid auth token. Use to authorise the expressions in your web part
/// code (e.g. WishList.getWishList).
let useToken ctx f = async {
    match ctx.request.header "Authorization" with
    | Choice1Of2 accesstoken when accesstoken.StartsWith "Bearer " -> 
        let jwt = accesstoken.Replace("Bearer ","")
        match JsonWebToken.isValid jwt with
        | None -> return! FORBIDDEN "Accessing this API is not allowed" ctx
        | Some token -> return! f token
    | _ -> return! BAD_REQUEST "Request doesn't contain a JSON Web Token" ctx
}
