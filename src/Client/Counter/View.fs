module Counter.View

open Fable.Core
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Types

let simpleButton txt action dispatch =
  div
    [ ClassName "column is-narrow" ]
    [ a
        [ ClassName "button"
          OnClick (fun _ -> action |> dispatch) ]
        [ str txt ] ]

let userComponent (user: Shared.Types.User option)  =
  match user with
  | Some user ->
    div [] [ str (sprintf "%s %s" user.Firstname user.Surname) ]
  | None -> div [] []
let root model dispatch =
  div
    [ ClassName "columns is-vcentered" ]
    [ div [ ClassName "column" ] [ ]
      div
        [ ClassName "column is-narrow"
          Style
            [ CSSProp.Width "170px" ] ]
        [ str (sprintf "Counter value: %i" model.Counter)
          userComponent model.User ]
      simpleButton "+1" Increment dispatch
      simpleButton "-1" Decrement dispatch
      simpleButton "Fetch" FetchUser dispatch
      simpleButton "Reset" Reset dispatch
      div [ ClassName "column" ] [ ] ]
