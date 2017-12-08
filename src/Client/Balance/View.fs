module Client.Balance.View

open Fable.Core
open Fable.Core.JsInterop
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma.Elements
open Fulma.Extra.FontAwesome

open Types

let root model dispatch =
  match model.Balance with
  | Some balance ->
    div []
      [
        Box.box' []
          [
            Table.table [ Table.isNarrow ]
              [
                tbody [ ]
                  [
                    tr [ ]
                      [
                        th [ ClassName "has-text-right" ] [ str "User name" ]
                        td [] []
                        td [ ] [ str balance.UserName ]
                      ]
                    tr [ ]
                      [
                        th [ ClassName "has-text-right" ] [ str (sprintf "Carried over from %d" (balance.BalanceYear - 1)) ]
                        td [] []
                        td [ ] [ str (sprintf "%.2f" balance.CarriedOver) ]
                      ]
                    tr [ ]
                      [
                        th [ ClassName "has-text-right" ] [ str (sprintf "Portion of %d allotment accrued to date" balance.BalanceYear) ]
                        td [] [ str "+" ]
                        td [ ] [ str (sprintf "%.2f" balance.PortionAccruedToDate) ]
                      ]
                    tr [ ]
                      [
                        th [ ClassName "has-text-right" ] [ str "Taken to date" ]
                        td [] [ str "-" ]
                        td [ ] [ str (sprintf "%.2f" balance.TakenToDate) ]
                      ]
                    tr [ ]
                      [
                        th [ ClassName "has-text-right" ] [ str "Current balance" ]
                        td [] [ str "=" ]
                        td [ ] [ str (sprintf "%.2f" balance.CurrentBalance) ]
                      ]
                  ]
              ]
          ]
      ]
  | None ->
    div []
      [
        Icon.faIcon [ Icon.isLarge ]
                    [ Fa.icon Fa.I.Spinner
                      Fa.pulse ]
        str (sprintf "Loading vacation balance for user %s" model.UserToDisplay)
      ]