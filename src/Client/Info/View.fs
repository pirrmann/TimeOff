module Info.View

open Fable.Helpers.React
open Fable.Helpers.React.Props

let root =
  div
    [ ClassName "content" ]
    [ h1
        [ ]
        [ str "About page" ]
      p
        [ ]
        [ str "This is a starter project to use in an F# coding assignment working with Fable + Elmish + React on the front-end, Suave and event-sourcing on the back-end." ] ]
