module Client.Login.State

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.PowerPack
open Fable.PowerPack.Fetch.Fetch_types
open Elmish

open Shared
open Shared.Types
open Client
open Client.Login.Types

let authUser (login: Login) =
  promise {
      if String.IsNullOrEmpty login.UserName then return! failwithf "You need to fill in a username." else
      if String.IsNullOrEmpty login.Password then return! failwithf "You need to fill in a password." else

      let body = toJson login

      let props = 
          [ RequestProperties.Method HttpMethod.POST
            Fetch.requestHeaders [
              HttpRequestHeaders.ContentType "application/json" ]
            RequestProperties.Body !^body ]
      
      try
          let! response = Fetch.fetch ServerUrls.Login props

          if not response.Ok then
              return! failwithf "Error: %d" response.Status
          else  
              let! authSuccess = response.text() |> Promise.map ofJson<AuthSuccess>
              return {
                UserName = login.UserName
                Role = authSuccess.Role
                Token = authSuccess.Token
              }
      with
      | _ -> return! failwithf "Could not authenticate user."
  }

let init (user: UserData option) = 
    match user with
    | None ->
        { Login = { UserName = ""; Password = ""; PasswordId = Guid.NewGuid() }
          State = LoggedOut
          ErrorMsg = "" }, Cmd.none
    | Some user ->
        { Login = { UserName = user.UserName; Password = ""; PasswordId = Guid.NewGuid() }
          State = LoggedIn { UserName = user.UserName; Role = user.Role; Token = user.Token }
          ErrorMsg = "" }, Cmd.none

let authUserCmd login = 
  Cmd.ofPromise authUser login AuthSuccess AuthError

let update onInternal onSuccess (msg:Msg) model : Model*Cmd<_> = 
    match msg with
    | AuthSuccess userData ->
        { model with State = LoggedIn userData;  Login = { model.Login with Password = ""; PasswordId = Guid.NewGuid()  } }, onSuccess userData
    | SetUserName name ->
        { model with Login = { model.Login with UserName = name; Password = ""; PasswordId = Guid.NewGuid() } }, Cmd.none
    | SetPassword pw ->
        { model with Login = { model.Login with Password = pw }}, Cmd.none
    | ClickLogIn ->
        model, authUserCmd model.Login |> Cmd.map onInternal
    | AuthError exn ->
        { model with ErrorMsg = string (exn.Message) }, Cmd.none