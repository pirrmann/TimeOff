module Client.Employees.Types

open System
open Shared.Types
open Client

type EmployeeFormModel = {
  UserData: UserData
  Creating: bool
  UserName: string
  FirstName: string
  StartDate: DateTime
  LastName: string
  MonthlyVacationRate: float
}

type EmployeeFormMsg =
  | SetUserName of string
  | SetFirstName of string
  | SetLastName of string
  | SetStartDate of DateTime
  | SetMonthlyVacationRate of float
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
