module Client.Balance.State

open Elmish
open Types

let init userData userToDisplay : Model * Cmd<Msg> =
  { UserData = userData; UserToDisplay = defaultArg userToDisplay userData.UserName; Balance = None }, Cmd.ofMsg FetchBalance

let update msg model =
  match msg with
  | FetchBalance ->
      model, Cmd.ofPromise (Rest.getUserBalance model.UserData.Token) model.UserToDisplay DisplayBalance NetworkError
  | DisplayBalance balance ->
      { model with Balance = Some balance }, []
  | NetworkError error ->
    printfn "[Balance.State][Network error] %s" error.Message
    model, Cmd.none
