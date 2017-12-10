[<AutoOpen>]
module Client.Global

open Shared.Types

type Page =
  | Home
  | Login
  | Balance of userName:string option
  | Users
  | About

let toHash page =
  match page with
  | About -> "#about"
  | Login -> "#login"
  | Balance None -> "#balance"
  | Balance (Some userName) -> sprintf "#balance/%s" userName
  | Users -> "#users"
  | Home -> "#home"

type UserData = 
  { UserName : string 
    Role : UserRole
    Token : JWT }

type NavigationData = {
  CurrentPage: Page
  User: UserData option
}

[<RequireQualifiedAccess>]
type NotificationType =
  | Success
  | Error

type Notification = {
  NotificationType: NotificationType
  Text: string } with
  static member Success text = { NotificationType = NotificationType.Success; Text = text }
  static member Error text = { NotificationType = NotificationType.Error; Text = text }

type GlobalMsg =
  | LoggedIn of UserData
  | Logout
  | LoggedOut
  | StorageFailure of exn