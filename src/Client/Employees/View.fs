module Client.Employees.View

open System

open Fable.Core
open Fable.Core.JsInterop
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack.Date
open Fulma.Elements
open Fulma.Elements.Form
open Fulma.Extra.FontAwesome

open Client

open Shared.Types
open Types

let employeeForm (employeeFormModel: EmployeeFormModel) dispatch =
  let formActionText = if employeeFormModel.Creating then "Create" else "Save"
  Box.box' []
    [
      form [ ]
        [
          Field.field_div [ ]
            [ Label.label [ ]
                [ str "Username" ]
              (
                if employeeFormModel.Creating then
                  Control.control_div [ Control.hasIconLeft ]
                    [ Input.input [ Input.typeIsText
                                    Input.id "username"
                                    Input.placeholder "Username"
                                    Input.defaultValue employeeFormModel.UserName
                                    Input.props [
                                      OnChange (fun ev -> dispatch (SetUserName !!ev.target?value))
                                      AutoFocus true ] ]
                      Icon.faIcon [ Icon.isSmall; Icon.isLeft ] [ Fa.icon Fa.I.User ] ]
                else
                  Control.control_p [] [ str employeeFormModel.UserName ]
              )
            ]

          Field.field_div [ ]
            [ Label.label [ ]
                [ str "First name" ]
              Control.control_div [ ]
                [ Input.input [ Input.typeIsText
                                Input.placeholder "First name"
                                Input.id "first-name"
                                Input.defaultValue employeeFormModel.FirstName
                                Input.props [
                                  OnChange (fun ev -> dispatch (SetFirstName !!ev.target?value)) ] ] ] ]

          Field.field_div [ ]
            [ Label.label [ ]
                [ str "Last name" ]
              Control.control_div [ ]
                [ Input.input [ Input.typeIsText
                                Input.placeholder "Last name"
                                Input.id "last-name"
                                Input.defaultValue employeeFormModel.LastName
                                Input.props [
                                  OnChange (fun ev -> dispatch (SetLastName !!ev.target?value)) ] ] ] ]

          Field.field_div [ ]
            [ Label.label [ ]
                [ str "Start date" ]
              Control.control_div [ ]
                [ Input.input [ Input.typeIsDate
                                Input.id "start-date"
                                Input.defaultValue (toDateString employeeFormModel.StartDate)
                                Input.props [
                                  OnChange (fun ev -> dispatch (SetStartDate !!ev.target?value)) ] ] ] ]

          Field.field_div [ ]
            [ Label.label [ ]
                [ str "Monthly vacation rate" ]
              Control.control_div [ ]
                [ Input.input [ Input.typeIsNumber
                                Input.id "monthly-vacation-rate"
                                Input.defaultValue (sprintf "%f" employeeFormModel.MonthlyVacationRate)
                                Input.props [
                                  OnChange (fun ev -> dispatch (SetMonthlyVacationRate !!ev.target?value)) ] ] ] ]
           
          Field.field_div [ Field.isGrouped ]
            [ Control.control_div [ ]
                [
                  Button.button_a [Button.isActive; Button.onClick  (fun _ -> dispatch ActionClicked)] [ str formActionText ]
                ]
              Control.control_div [ ]
                [
                  Button.button_a [Button.isActive; Button.onClick  (fun _ -> dispatch CancelClicked)] [ str "Cancel" ]
                ]
            ]
        ]
    ]

let employeeList model dispatch =

  let employeeLine (employee: Employee) =
    tr [ ]
      [
        td [] [ a [ OnClick (fun _ -> dispatch (EditEmployeeClicked employee.UserName))] [ str employee.UserName ] ]
        td [] [ str employee.FirstName ]
        td [] [ str employee.LastName ]
        td [] [ a [ Href (toHash (Balance (Some employee.UserName))) ] [ str "View balance" ] ]
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

              for employee in model.Employees do
                yield employeeLine employee
            ]
        ]
      
      Button.button_btn [ Button.isActive; Button.onClick (fun _ -> dispatch CreateNewEmployeeClicked ) ]
        [
          str "Create new employee"
        ]
    ]

let mainContent model dispatch =

  match model.Fetching, model.EmployeeForm with
  | true, None ->
    [
      Icon.faIcon [ Icon.isLarge ]
                  [ Fa.icon Fa.I.Spinner
                    Fa.pulse ]
      str "Loading employees"
    ]
  | false, None ->
    [
      employeeList model dispatch
    ]
  | true, Some employeeFormModel ->
    [
      Icon.faIcon [ Icon.isLarge ]
                  [ Fa.icon Fa.I.Spinner
                    Fa.pulse ]
      str (sprintf "Loading details for employee %s" employeeFormModel.UserName)
    ]
  | false, Some employeeFormModel ->
    [
      employeeForm employeeFormModel (EmployeeFormMsg >> dispatch)
    ]

let root model dispatch =
  div []
    [
      yield! model.Notification |> Option.map notificationBox |> Option.toList
      yield! mainContent model dispatch
    ]

