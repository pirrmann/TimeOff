module Counter.State

open Elmish
open Types
let init () : Model * Cmd<Msg> =
  { Counter  = 0; User = None }, []

let update msg model =
  match msg with
  | Increment ->
      { model with Counter = model.Counter  + 1 }, []
  | Decrement ->
      { model with Counter = model.Counter  - 1 }, []
  | Reset ->
      { model with Counter = 0; User = None }, []
  | FetchUser ->
      model, Cmd.ofPromise Rest.getUser () DisplayUser NetworkError
  | DisplayUser user ->
      { model with User = Some user }, []
  | NetworkError error ->
    printfn "[Counter.State][Network error] %s" error.Message
    model, Cmd.none
