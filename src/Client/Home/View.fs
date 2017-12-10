module Client.Home.View

open Fable.Core
open Fable.Core.JsInterop
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma.Elements
open Types

let root model dispatch =
  div
    [ ]
    [ 
      Heading.h3 [] [ str "Welcome to the TimeOff application." ]

      p [] [ str "Below is an example of a single counter. In order to understand Elmish better, we'll start by transforming this page so that it can handle a list of counters." ]

      p [] [ str "When the assignement is really started, this page should become a welcome page with links to the most useful features, and the counters code will be removed." ]

      br []

      Box.box' []
        [
          Client.Counter.View.root model.Counter (CounterMsg >> dispatch)
        ]
    ]
