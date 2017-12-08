module Client.State

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Fable.Import
open Global
open Types

let pageParser: Parser<Page->Page,Page> =
  oneOf [
    map Home (s "home")
    map Login (s "login")
    map Balance (s "balance")
    map About (s "about")
  ]

let stayOnCurrentPage model =
  model, Navigation.modifyUrl (toHash model.Navigation.CurrentPage)

let urlUpdate (result: Option<Page>) model =
  match result with
  | None ->
    Browser.console.error("Error parsing url: " + Browser.window.location.href)
    stayOnCurrentPage model
  | Some (Login as page) ->
    let m, cmd = Login.State.init model.Navigation.User
    { model with Navigation = { model.Navigation with CurrentPage = page }; TransientPageModel = LoginModel m }, Cmd.map LoginMsg cmd
  | Some (Balance as page) ->
    match model.Navigation.User with
    | Some user ->
      let m, cmd = Balance.State.init user None
      { model with Navigation = { model.Navigation with CurrentPage = page }; TransientPageModel = BalanceModel m }, Cmd.map BalanceMsg cmd
    | None ->
      stayOnCurrentPage model
  | Some page ->
    { model with Navigation = { model.Navigation with CurrentPage = page }; TransientPageModel = NoPageModel }, []

let init result =
  let (home, homeCmd) = Home.State.init()
  let (model, cmd) =
    urlUpdate result
      {
        Navigation =
          {
            User = LocalStorage.load "user"
            CurrentPage = Home
          }
        TransientPageModel = NoPageModel
        Home = home }
  model, Cmd.batch [ cmd
                     Cmd.map HomeMsg homeCmd ]

let loadUser () =
    LocalStorage.load "user"

let saveUserCmd user =
    Cmd.ofFunc (LocalStorage.save "user") user (fun _ -> LoggedIn user) StorageFailure
    |> Cmd.map GlobalMsg

let deleteUserCmd =
    Cmd.ofFunc LocalStorage.delete "user" (fun _ -> LoggedOut) StorageFailure
    |> Cmd.map GlobalMsg

let update msg model =
  match msg, model.TransientPageModel with
  | GlobalMsg (StorageFailure e), _ ->
      printfn "Unable to access local storage: %A" e
      model, Cmd.none

  | GlobalMsg (LoggedIn newUser), _ ->
    { model with Navigation = { model.Navigation with User = Some newUser } }, Navigation.newUrl (toHash Home)

  | GlobalMsg LoggedOut, _ ->
    { model with Navigation = { model.Navigation with User = None } }, Navigation.newUrl (toHash Home)

  | GlobalMsg Logout, _ ->
    model, deleteUserCmd

  | LoginMsg msg, LoginModel login ->
    let (login, cmd) = Login.State.update LoginMsg saveUserCmd msg login
    { model with TransientPageModel = LoginModel login }, cmd

  | LoginMsg _, _ -> model, Cmd.none

  | BalanceMsg msg, BalanceModel balance ->
    let (balance, balanceCmd) = Balance.State.update msg balance
    { model with TransientPageModel = BalanceModel balance }, Cmd.map BalanceMsg balanceCmd

  | BalanceMsg _, _ -> model, Cmd.none

  | HomeMsg msg, _ ->
    let (home, homeCmd) = Home.State.update msg model.Home
    { model with Home = home }, Cmd.map HomeMsg homeCmd
