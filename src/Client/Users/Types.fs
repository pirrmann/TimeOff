module Client.Users.Types

open Shared.Types
open Client

type UserFormModel = {
  UserData: UserData
  Creating: bool
  UserName: string
  FirstName: string
  LastName: string
}

type UserFormMsg =
  | SetUserFormUserName of string
  | SetUserFormFirstName of string
  | SetUserFormLastName of string
  | PerformFormActionClicked
  | CancelClicked

type Model = {
  UserData: UserData
  Fetching: bool
  Notification: Notification option
  Users: User list
  UserForm: UserFormModel option
}

type Msg =
  | FetchUsers
  | FetchUser of string
  | DisplayUsersList of User list
  | DisplayUserForm of User
  | NetworkError of exn
  | EditUserClicked of string
  | CreateNewUserClicked
  | UserFormMsg of UserFormMsg
  | UserSaved of User
  | CloseUserForm
  | HideNotification
