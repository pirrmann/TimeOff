module Client.Login.View

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import
open Fable.PowerPack
open Fable.PowerPack.Fetch.Fetch_types
open Elmish
open Fulma
open Fulma.Elements
open Fulma.Elements.Form
open Fulma.Extra.FontAwesome

open Types

let [<Literal>] ENTER_KEY = 13.

let root model (dispatch: Msg -> unit) = 
  let buttonActive = if String.IsNullOrEmpty model.Login.UserName || String.IsNullOrEmpty model.Login.Password then Button.isDisabled else Button.isPrimary

  let onEnter msg dispatch =
    function 
    | (ev:React.KeyboardEvent) when ev.keyCode = ENTER_KEY ->
        ev.preventDefault()
        dispatch msg
    | _ -> ()
    |> OnKeyDown
      
  let notification =
    if not (String.IsNullOrEmpty model.ErrorMsg) then
      Notification.notification [ Notification.isDanger ] [
        div [] [ str model.ErrorMsg ]
        div [] [ str "Log in with either 'employee1', 'employee2', 'manager' or 'hr', using your login as your password." ]
      ]
    else
      Notification.notification [ Notification.isInfo ] [
        str "Log in with either 'employee1', 'employee2', 'manager' or 'hr', using your login as your password."]

  match model.State with
  | LoggedIn _ ->
      div [] [
        h3 [] [ str (sprintf "You're logged in as %s." model.Login.UserName) ]
      ]

  | LoggedOut ->
    form [ ] [
      notification

      Field.field_div [ ]
        [ Label.label [ ]
            [ str "Username" ]
          Control.control_div [ Control.hasIconLeft ]
            [ Input.input [ Input.typeIsText
                            Input.id "password"
                            Input.placeholder "Username"
                            Input.defaultValue model.Login.UserName
                            Input.props [
                              OnChange (fun ev -> dispatch (SetUserName !!ev.target?value))
                              AutoFocus true ] ]
              Icon.faIcon [ Icon.isSmall; Icon.isLeft ] [ Fa.icon Fa.I.User ] ] ]

      Field.field_div [ ]
        [ Label.label [ ]
            [ str "Password" ]
          Control.control_div [ Control.hasIconLeft ]
            [ Input.input [ Input.typeIsPassword
                            Input.placeholder "Password"
                            Input.id "password"
                            Input.defaultValue model.Login.Password
                            Input.props [
                              Key ("password_" + model.Login.PasswordId.ToString())
                              OnChange (fun ev -> dispatch (SetPassword !!ev.target?value))
                              onEnter ClickLogIn dispatch ] ]
              Icon.faIcon [ Icon.isSmall; Icon.isLeft ] [ Fa.icon Fa.I.Key ] ] ]
       
      div [ ClassName "text-center" ] [
          Button.button_a [buttonActive; Button.onClick  (fun _ -> dispatch ClickLogIn)] [ str "Log In" ]
      ]                   
    ]    
