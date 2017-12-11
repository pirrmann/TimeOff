module Client.Employees.Types

open Shared.Types
open Client

type EmployeeFormModel = {
  UserData: UserData
  Creating: bool
  UserName: string
  FirstName: string
  LastName: string
}

type EmployeeFormMsg =
  | SetUserName of string
  | SetFirstName of string
  | SetLastName of string
  | ActionClicked
  | CancelClicked

type Model = {
  UserData: UserData
  Fetching: bool
  Notification: Notification option
  Employees: Employee list
  EmployeeForm: EmployeeFormModel option
}

type Msg =
  | FetchEmployees
  | FetchEmployee of string
  | DisplayEmployeesList of Employee list
  | DisplayEmployeeForm of Employee
  | NetworkError of exn
  | EditEmployeeClicked of string
  | CreateNewEmployeeClicked
  | EmployeeFormMsg of EmployeeFormMsg
  | EmployeeSaved of Employee
  | CloseEmployeeForm
  | HideNotification
