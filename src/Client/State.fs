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
    map (Balance << Some) (s "balance" </> str)
    map (Balance None) (s "balance")
    map Users (s "users")
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
  | Some (Balance userName as page) ->
    match model.Navigation.User with
    | Some user ->
      let m, cmd = Balance.State.init user userName
      { model with Navigation = { model.Navigation with CurrentPage = page }; TransientPageModel = BalanceModel m }, Cmd.map BalanceMsg cmd
    | None ->
      stayOnCurrentPage model
  | Some (Users as page) ->
    match model.Navigation.User with
    | Some user ->
      let m, cmd = Users.State.init user
      { model with Navigation = { model.Navigation with CurrentPage = page }; TransientPageModel = UsersModel m }, Cmd.map UsersMsg cmd
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

  | LoginMsg msg, LoginModel loginModel ->
    let (loginModel, cmd) = Login.State.update LoginMsg saveUserCmd msg loginModel
    { model with TransientPageModel = LoginModel loginModel }, cmd
  | LoginMsg _, _ -> model, Cmd.none

  | BalanceMsg msg, BalanceModel balanceModel ->
    let (balanceModel, balanceCmd) = Balance.State.update msg balanceModel
    { model with TransientPageModel = BalanceModel balanceModel }, Cmd.map BalanceMsg balanceCmd
  | BalanceMsg _, _ -> model, Cmd.none

  | UsersMsg msg, UsersModel usersModel ->
    let (usersModel, usersCmd) = Users.State.update msg usersModel
    { model with TransientPageModel = UsersModel usersModel }, Cmd.map UsersMsg usersCmd
  | UsersMsg _, _ -> model, Cmd.none

  | HomeMsg msg, _ ->
    let (home, homeCmd) = Home.State.update msg model.Home
    { model with Home = home }, Cmd.map HomeMsg homeCmd
