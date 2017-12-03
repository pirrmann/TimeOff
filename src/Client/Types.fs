module Client.Types

open Global

type AppMsg =
  | GlobalMsg of GlobalMsg
  | LoginMsg of Login.Types.Msg
  | CounterMsg of Counter.Types.Msg
  | HomeMsg of Home.Types.Msg

type TransientPageModel =
  | NoPageModel
  | LoginModel of Login.Types.Model

type Model = {
    Navigation: NavigationData
    TransientPageModel: TransientPageModel 
    Home: Home.Types.Model
    Counter: Counter.Types.Model
  }
