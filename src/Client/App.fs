module Client.View

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Browser
open Client.Types
open Client.State

importAll "../../sass/main.sass"

open Fable.Helpers.React
open Fable.Helpers.React.Props

let root model dispatch =

  let pageHtml =
    function
    | Home -> [ Home.View.root model.Home (HomeMsg >> dispatch) ]
    | Login ->
      match model.TransientPageModel with
      | LoginModel loginModel -> [ Login.View.root loginModel (LoginMsg >> dispatch) ]
      | _ -> []
    | Balance _ ->
      match model.TransientPageModel with
      | BalanceModel balanceModel -> [ Balance.View.root balanceModel (BalanceMsg >> dispatch) ]
      | _ -> []
    | Employees ->
      match model.TransientPageModel with
      | EmployeesModel employeesModel -> [ Employees.View.root employeesModel (EmployeesMsg >> dispatch) ]
      | _ -> []
    | Page.About -> [ About.View.root ]

  div
    []
    [ div
        [ ClassName "navbar-bg" ]
        [ div
            [ ClassName "container" ]
            [ Navbar.View.view model.Navigation (GlobalMsg >> dispatch) ] ]
      div
        [ ClassName "section" ]
        [ div
            [ ClassName "container" ]
            [ div
                [ ClassName "columns" ]
                [ div
                    [ ClassName "column is-3" ]
                    [ Menu.View.view model.Navigation ]
                  div
                    [ ClassName "column" ]
                    (pageHtml model.Navigation.CurrentPage) ] ] ] ]

open Elmish.React
open Elmish.Debug
open Elmish.HMR

// App
Program.mkProgram init update root
|> Program.toNavigable (parseHash pageParser) urlUpdate
#if DEBUG
|> Program.withConsoleTrace
|> Program.withDebugger
|> Program.withHMR
#endif
|> Program.withReact "elmish-app"
|> Program.run
