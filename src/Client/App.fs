module Client.View

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Browser
open Client.Global
open Client.Types
open Client.State

importAll "../../sass/main.sass"

open Fable.Helpers.React
open Fable.Helpers.React.Props

let menuItem label page currentPage =
    li
      [ ]
      [ a
          [ classList [ "is-active", page = currentPage ]
            Href (toHash page) ]
          [ str label ] ]

let menu currentPage =
  aside
    [ ClassName "menu" ]
    [ p
        [ ClassName "menu-label" ]
        [ str "General" ]
      ul
        [ ClassName "menu-list" ]
        [ menuItem "Home" Home currentPage
          menuItem "Counter sample" Counter currentPage
          menuItem "About" Page.About currentPage ] ]

let root model dispatch =

  let pageHtml =
    function
    | Home -> [ Home.View.root model.Home (HomeMsg >> dispatch) ]
    | Login ->
      match model.TransientPageModel with
      | LoginModel login -> [ Login.View.root login (LoginMsg >> dispatch) ]
      | _ -> []
    | Counter -> [ Counter.View.root model.Counter (CounterMsg >> dispatch) ]
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
                    [ menu model.Navigation.CurrentPage ]
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
