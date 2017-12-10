module Client.Users.Types

open Shared.Types
open Client

type UserFormModel = {
  Creating: bool
  UserName: string
  FirstName: string
  LastName: string
}

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
  | CreateUserClicked
  | SetUserFormUserName of string
  | SetUserFormFirstName of string
  | SetUserFormLastName of string
  | PerformFormActionClicked
  | UserSaved of User
  | HideNotification
