module Client.Global

open Shared.Types

type Page =
  | Home
  | Login
  | Balance
  | About

let toHash page =
  match page with
  | About -> "#about"
  | Login -> "#login"
  | Balance -> "#balance"
  | Home -> "#home"

type UserData = 
  { UserName : string 
    Token : JWT }

type NavigationData = {
  CurrentPage: Page
  User: UserData option
}

type GlobalMsg =
  | LoggedIn of UserData
  | Logout
  | LoggedOut
  | StorageFailure of exn