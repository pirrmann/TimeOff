module Client.Menu.View

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Client
open Global

let menuItem label page currentPage =
    li
      [ ]
      [ a
          [ classList [ "is-active", page = currentPage ]
            Href (toHash page) ]
          [ str label ] ]

let view (model: NavigationData) =
  let currentPage = model.CurrentPage
  let loggedIn = model.User.IsSome
  aside
    [ ClassName "menu" ]
    [ p
        [ ClassName "menu-label" ]
        [ str "General" ]
      ul
        [ ClassName "menu-list" ]
        [ yield menuItem "Home" Home currentPage
          if loggedIn then
            yield menuItem "Balance" Balance currentPage
          yield menuItem "About" About currentPage ] ]