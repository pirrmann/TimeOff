module Client.Users.State

open Elmish
open Types
open Shared.Types
open Client

let init userData : Model * Cmd<Msg> =
  { UserData = userData; Fetching = false; Notification = None; Users = []; UserForm = None }, Cmd.ofMsg FetchUsers

let update msg model =
  match msg with
  | FetchUsers ->
    { model with Fetching = true }, Cmd.ofPromise (Rest.getUsers model.UserData.Token) () DisplayUsersList NetworkError
  | FetchUser userName ->
    { model with Fetching = true }, Cmd.ofPromise (Rest.getUser model.UserData.Token) userName DisplayUserForm NetworkError
  | DisplayUsersList users ->
    { model with Fetching = false; Users = users }, []
  | DisplayUserForm user ->
    { model with Fetching = false; UserForm = Some { Creating = false
                                                     UserName = user.UserName
                                                     FirstName = user.FirstName
                                                     LastName = user.LastName } }, []
  | NetworkError error ->
    printfn "[Users.State][Network error] %s" error.Message
    model, Cmd.none
  | EditUserClicked userName ->
    { model with UserForm = Some { Creating = false
                                   UserName = userName
                                   FirstName = null
                                   LastName = null } }, Cmd.ofMsg (FetchUser userName)
  | CreateUserClicked ->
    { model with UserForm = Some { Creating = true
                                   UserName = null
                                   FirstName = null
                                   LastName = null } }, Cmd.none
  | SetUserFormUserName userName ->
    let userForm = model.UserForm.Value
    let model' =
      if userForm.Creating then { model with UserForm = Some { userForm with UserName = userName } }
      else model
    model', Cmd.none
  | SetUserFormFirstName firstName ->
    { model with UserForm = model.UserForm |> Option.map (fun f -> {f with FirstName = firstName }) }, Cmd.none
  | SetUserFormLastName lastName ->
    { model with UserForm = model.UserForm |> Option.map (fun f -> {f with LastName = lastName }) }, Cmd.none
  | PerformFormActionClicked ->
    let userForm =  model.UserForm.Value
    let user: User = { UserName = userForm.UserName
                       FirstName = userForm.FirstName
                       LastName = userForm.LastName }
    let restCall = if model.UserForm.Value.Creating then Rest.createUser else Rest.updateUser
    model, Cmd.ofPromise (restCall model.UserData.Token) user UserSaved NetworkError
  | UserSaved user ->
    { model with UserForm = None; Notification = Some (Notification.Success (sprintf "User %s saved" user.UserName)) },
    Cmd.batch [Cmd.ofMsg FetchUsers; Cmd.ofDelayedMsg HideNotification 2000]
  | HideNotification ->
    { model with Notification = None }, Cmd.none
