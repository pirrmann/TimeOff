module Client.Employees.State

open System

open Elmish
open Types
open Shared.Types
open Client

let init userData : Model * Cmd<Msg> =
  { UserData = userData; Fetching = false; Notification = None; Employees = []; EmployeeForm = None }, Cmd.ofMsg FetchEmployees

let updateEmployeeForm msg model =
  match msg with
  | SetUserName userName ->
    (if model.Creating then { model with UserName = userName } else model), Cmd.none
  | SetFirstName firstName ->
    { model with FirstName = firstName }, Cmd.none
  | SetLastName lastName ->
    { model with LastName = lastName }, Cmd.none
  | SetStartDate startDate ->
    { model with StartDate = startDate }, Cmd.none
  | SetMonthlyVacationRate rate ->
    { model with MonthlyVacationRate = rate }, Cmd.none
  | ActionClicked ->
    let employee: Employee = { UserName = model.UserName
                               FirstName = model.FirstName
                               LastName = model.LastName
                               StartDate = model.StartDate
                               MonthlyVacationRate = model.MonthlyVacationRate }
    let restCall = if model.Creating then Rest.createEmployee else Rest.updateEmployee
    model, Cmd.ofPromise (restCall model.UserData.Token) employee EmployeeSaved NetworkError
  | CancelClicked ->
    model, Cmd.ofMsg CloseEmployeeForm

let update msg model =
  match msg with
  | FetchEmployees ->
    { model with Fetching = true }, Cmd.ofPromise (Rest.getEmployees model.UserData.Token) () DisplayEmployeesList NetworkError
  | FetchEmployee userName ->
    { model with Fetching = true }, Cmd.ofPromise (Rest.getEmployee model.UserData.Token) userName DisplayEmployeeForm NetworkError
  | DisplayEmployeesList employees ->
    { model with Fetching = false; Employees = employees }, []
  | DisplayEmployeeForm employee ->
    { model with Fetching = false; EmployeeForm = Some { UserData = model.UserData
                                                         Creating = false
                                                         UserName = employee.UserName
                                                         FirstName = employee.FirstName
                                                         LastName = employee.LastName
                                                         StartDate = employee.StartDate
                                                         MonthlyVacationRate = employee.MonthlyVacationRate } }, []
  | NetworkError error ->
    printfn "[Employees.State][Network error] %s" error.Message
    model, Cmd.none
  | EditEmployeeClicked userName ->
    { model with EmployeeForm = Some { UserData = model.UserData
                                       Creating = false
                                       UserName = userName
                                       FirstName = null
                                       LastName = null
                                       StartDate = DateTime.Now
                                       MonthlyVacationRate = 0. } }, Cmd.ofMsg (FetchEmployee userName)
  | CreateNewEmployeeClicked ->
    { model with EmployeeForm = Some { UserData = model.UserData
                                       Creating = true
                                       UserName = null
                                       FirstName = null
                                       LastName = null
                                       StartDate = DateTime.Now
                                       MonthlyVacationRate = 0. } }, Cmd.none
  | EmployeeFormMsg msg ->
    match model.EmployeeForm with
    | Some employeeFormModel ->
      let employeeFormModel', cmd = updateEmployeeForm msg employeeFormModel
      { model with EmployeeForm = Some employeeFormModel' }, cmd
    | None -> model, Cmd.none
  | EmployeeSaved employee ->
    { model with EmployeeForm = None; Notification = Some (Notification.Success (sprintf "Employee %s saved" employee.UserName)) },
    Cmd.batch [Cmd.ofMsg FetchEmployees; Cmd.ofDelayedMsg HideNotification 2000]
  | CloseEmployeeForm ->
    { model with EmployeeForm = None }, Cmd.none
  | HideNotification ->
    { model with Notification = None }, Cmd.none
