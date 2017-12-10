module Client.Users.State

open Elmish
open Types
open Shared.Types
open Client
open Fable.Import.RemoteDev.MsgTypes

let init userData : Model * Cmd<Msg> =
  { UserData = userData; Fetching = false; Notification = None; Users = []; UserForm = None }, Cmd.ofMsg FetchUsers

let updateUserForm msg model =
  match msg with
  | SetUserFormUserName userName ->
    (if model.Creating then { model with UserName = userName } else model), Cmd.none
  | SetUserFormFirstName firstName ->
    { model with FirstName = firstName }, Cmd.none
  | SetUserFormLastName lastName ->
    { model with LastName = lastName }, Cmd.none
  | PerformFormActionClicked ->
    let user: User = { UserName = model.UserName
                       FirstName = model.FirstName
                       LastName = model.LastName }
    let restCall = if model.Creating then Rest.createUser else Rest.updateUser
    model, Cmd.ofPromise (restCall model.UserData.Token) user UserSaved NetworkError
  | CancelClicked ->
    model, Cmd.ofMsg CloseUserForm

let update msg model =
  match msg with
  | FetchUsers ->
    { model with Fetching = true }, Cmd.ofPromise (Rest.getUsers model.UserData.Token) () DisplayUsersList NetworkError
  | FetchUser userName ->
    { model with Fetching = true }, Cmd.ofPromise (Rest.getUser model.UserData.Token) userName DisplayUserForm NetworkError
  | DisplayUsersList users ->
    { model with Fetching = false; Users = users }, []
  | DisplayUserForm user ->
    { model with Fetching = false; UserForm = Some { UserData = model.UserData
                                                     Creating = false
                                                     UserName = user.UserName
                                                     FirstName = user.FirstName
                                                     LastName = user.LastName } }, []
  | NetworkError error ->
    printfn "[Users.State][Network error] %s" error.Message
    model, Cmd.none
  | EditUserClicked userName ->
    { model with UserForm = Some { UserData = model.UserData
                                   Creating = false
                                   UserName = userName
                                   FirstName = null
                                   LastName = null } }, Cmd.ofMsg (FetchUser userName)
  | CreateNewUserClicked ->
    { model with UserForm = Some { UserData = model.UserData
                                   Creating = true
                                   UserName = null
                                   FirstName = null
                                   LastName = null } }, Cmd.none
  | UserFormMsg msg ->
    match model.UserForm with
    | Some userFormModel ->
      let userFormModel', cmd = updateUserForm msg userFormModel
      { model with UserForm = Some userFormModel' }, cmd
    | None -> model, Cmd.none
  | UserSaved user ->
    { model with UserForm = None; Notification = Some (Notification.Success (sprintf "User %s saved" user.UserName)) },
    Cmd.batch [Cmd.ofMsg FetchUsers; Cmd.ofDelayedMsg HideNotification 2000]
  | CloseUserForm ->
    { model with UserForm = None }, Cmd.none
  | HideNotification ->
    { model with Notification = None }, Cmd.none
