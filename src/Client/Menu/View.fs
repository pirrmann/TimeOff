module Client.Menu.View

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Client
open Global
open Shared.Types

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
  let role = model.User |> Option.map (fun user -> user.Role)
  aside
    [ ClassName "menu" ]
    [ p
        [ ClassName "menu-label" ]
        [ str "General" ]
      ul
        [ ClassName "menu-list" ]
        [ yield menuItem "Home" Home currentPage
          if role = Some HumanResources then
            yield menuItem "Users" Users currentPage
          if loggedIn then
            yield menuItem "Balance" (Balance None) currentPage
          yield menuItem "About" About currentPage ] ]