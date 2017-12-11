module Shared.Types

open System

// Json web token type.
type JWT = string

// Login credentials.
type Login = 
    { UserName : string
      Password : string
      PasswordId : Guid }

type UserRole =
  | Employee
  | Manager
  | HumanResources
 
type AuthSuccess = {
  Token: JWT
  Role: UserRole
}

open Fable.Core

[<Pojo>]
type Employee = {
  UserName : string
  FirstName: string
  LastName: string
}

[<Pojo>]
type UserVacationBalance = {
  UserName : string
  BalanceYear: int
  CarriedOver: float
  PortionAccruedToDate: float
  TakenToDate: float
  CurrentBalance: float
}