module Client.Balance.Types

open Client
open Shared.Types

type Model = {
  UserData: UserData
  UserToDisplay: string
  Balance: UserVacationBalance option
}

type Msg =
  | FetchBalance
  | DisplayBalance of UserVacationBalance
  | NetworkError of exn
