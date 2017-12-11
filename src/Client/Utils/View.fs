namespace Client

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import
open Fulma.Elements
open Fulma.Elements.Form
open Fulma.Extra.FontAwesome
open Fable.Import.JS
open Fulma.BulmaClasses.Bulma

[<AutoOpen>]
module ViewUtils =

  let [<Literal>] ENTER_KEY = 13.

  let onEnter msg dispatch =
    function 
    | (ev:React.KeyboardEvent) when ev.keyCode = ENTER_KEY ->
        ev.preventDefault()
        dispatch msg
    | _ -> ()
    |> OnKeyDown

  let private getFulmaNotificationType notificationType =
    match notificationType with
    | NotificationType.Error -> Notification.isDanger
    | NotificationType.Success -> Notification.isSuccess

  let notificationBox (notification: Notification) =
    Notification.notification [ getFulmaNotificationType notification.NotificationType ] [
        div [] [ str notification.Text ]
      ]

  [<Emit("if(typeof $0.getMonth === 'function') { return $0.getFullYear() + '-' + ($0.getMonth() + 1) + '-' + $0.getDate();} else { return $0.substring(0, 4) + '-' + $0.substring(5, 7) + '-' + $0.substring(8, 10);}")>]
  let toDateString (_: DateTime) = jsNative