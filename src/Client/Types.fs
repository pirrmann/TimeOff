module Client.Types

open Global

type AppMsg =
  | GlobalMsg of GlobalMsg
  | LoginMsg of Login.Types.Msg
  | BalanceMsg of Balance.Types.Msg
  | EmployeesMsg of Employees.Types.Msg
  | HomeMsg of Home.Types.Msg

type TransientPageModel =
  | NoPageModel
  | LoginModel of Login.Types.Model
  | BalanceModel of Balance.Types.Model
  | EmployeesModel of Employees.Types.Model

type Model = {
    Navigation: NavigationData
    TransientPageModel: TransientPageModel 
    Home: Home.Types.Model
  }
