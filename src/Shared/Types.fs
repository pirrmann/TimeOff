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
type User =
    { Id : int
      Firstname: string
      Surname: string
      Email: string
      Password: string }
