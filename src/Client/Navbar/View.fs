module Client.Navbar.View

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Client
open Global

let navButton classy href onclick faClass txt =
    p
        [ ClassName "control" ]
        [ a
            [
              yield ClassName (sprintf "button %s" classy) :> IHTMLProp
              match href with
              | Some href ->
                yield Href href :> IHTMLProp
              | None -> ()
              match onclick with
              | Some onclick -> yield OnClick (fun _ -> onclick()) :> IHTMLProp           
              | None -> ()
            ]
            [ span
                [ ClassName "icon" ]
                [ i
                    [ ClassName (sprintf "fa %s" faClass) ]
                    [ ] ]
              span
                [ ]
                [ str txt ] ] ]

let loginStatus (model: NavigationData) dispatch =
    span
        [ ClassName "nav-item" ]
        [ div
            [ ClassName "field is-grouped" ]
            [
                if model.User = None then
                    yield navButton "" (Some (toHash Login)) None "fa-sign-in" "Login"
                else
                    yield navButton "" None (Some (fun () -> dispatch Logout)) "fa-sign-out" "Logout"
            ]
        ]

let view (model: NavigationData) dispatch =
    nav
        [ ClassName "nav" ]
        [ div
            [ ClassName "nav-left" ]
            [ h1
                [ ClassName "nav-item is-brand title is-4" ]
                [ str "Time Off" ] ]
          div
            [ ClassName "nav-right" ]
            [ loginStatus model dispatch ] ]