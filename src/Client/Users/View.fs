module Client.Users.View

open Fable.Core
open Fable.Core.JsInterop
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma.Elements
open Fulma.Elements.Form
open Fulma.Extra.FontAwesome

open Client

open Shared.Types
open Types

let userForm (userFormModel: UserFormModel) dispatch =
  let formActionText = if userFormModel.Creating then "Create" else "Save"
  Box.box' []
    [
      form [ ]
        [
          Field.field_div [ ]
            [ Label.label [ ]
                [ str "Username" ]
              (
                if userFormModel.Creating then
                  Control.control_div [ Control.hasIconLeft ]
                    [ Input.input [ Input.typeIsText
                                    Input.id "username"
                                    Input.placeholder "Username"
                                    Input.defaultValue userFormModel.UserName
                                    Input.props [
                                      OnChange (fun ev -> dispatch (SetUserFormUserName !!ev.target?value))
                                      AutoFocus true ] ]
                      Icon.faIcon [ Icon.isSmall; Icon.isLeft ] [ Fa.icon Fa.I.User ] ]
                else
                  Control.control_p [] [ str userFormModel.UserName ]
              )
            ]

          Field.field_div [ ]
            [ Label.label [ ]
                [ str "First name" ]
              Control.control_div [ ]
                [ Input.input [ Input.typeIsText
                                Input.placeholder "First name"
                                Input.id "first-name"
                                Input.defaultValue userFormModel.FirstName
                                Input.props [
                                  OnChange (fun ev -> dispatch (SetUserFormFirstName !!ev.target?value)) ] ] ] ]

          Field.field_div [ ]
            [ Label.label [ ]
                [ str "Last name" ]
              Control.control_div [ ]
                [ Input.input [ Input.typeIsText
                                Input.placeholder "Last name"
                                Input.id "last-name"
                                Input.defaultValue userFormModel.LastName
                                Input.props [
                                  OnChange (fun ev -> dispatch (SetUserFormLastName !!ev.target?value)) ] ] ] ]
           
          Field.field_div [ Field.isGrouped ]
            [ Control.control_div [ ]
                [
                  Button.button_a [Button.isActive; Button.onClick  (fun _ -> dispatch PerformFormActionClicked)] [ str formActionText ]
                ]
              Control.control_div [ ]
                [
                  Button.button_a [Button.isActive; Button.onClick  (fun _ -> dispatch CancelClicked)] [ str "Cancel" ]
                ]
            ]
        ]
    ]

let userList model dispatch =

  let userLine (user: User) =
    tr [ ]
      [
        td [] [ a [ OnClick (fun _ -> dispatch (EditUserClicked user.UserName))] [ str user.UserName ] ]
        td [] [ str user.FirstName ]
        td [] [ str user.LastName ]
        td [] [ a [ Href (toHash (Balance (Some user.UserName))) ] [ str "View balance" ] ]
      ]

  div []
    [
      Table.table [ Table.isBordered
                    Table.isNarrow
                    Table.isStripped ]
        [
          tbody []
            [
              yield tr []
                [
                  th [] [str "Username"]
                  th [] [str "First name"]
                  th [] [str "Last name"]
                  th [] []
                ]

              for user in model.Users do
                yield userLine user
            ]
        ]
      
      Button.button_btn [ Button.isActive; Button.onClick (fun _ -> dispatch CreateNewUserClicked ) ]
        [
          str "Create new user"
        ]
    ]

let mainContent model dispatch =

  match model.Fetching, model.UserForm with
  | true, None ->
    [
      Icon.faIcon [ Icon.isLarge ]
                  [ Fa.icon Fa.I.Spinner
                    Fa.pulse ]
      str "Loading users"
    ]
  | false, None ->
    [
      userList model dispatch
    ]
  | true, Some userFormModel ->
    [
      Icon.faIcon [ Icon.isLarge ]
                  [ Fa.icon Fa.I.Spinner
                    Fa.pulse ]
      str (sprintf "Loading details for user %s" userFormModel.UserName)
    ]
  | false, Some userFormModel ->
    [
      userForm userFormModel (UserFormMsg >> dispatch)
    ]

let root model dispatch =
  div []
    [
      yield! model.Notification |> Option.map notificationBox |> Option.toList
      yield! mainContent model dispatch
    ]

