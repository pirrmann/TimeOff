module Shared.Types

open System

// Json web token type.
type JWT = string

// Login credentials.
type Login = 
    { UserName : string
      Password : string
      PasswordId : Guid }

open Fable.Core

[<Pojo>]
type UserVacationBalance = {
  UserName : string
  BalanceYear: int
  CarriedOver: float
  PortionAccruedToDate: float
  TakenToDate: float
  CurrentBalance: float
}