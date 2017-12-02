module TimeOff.Server.App

open Shared.Types

open Suave
open Suave.Operators
open Suave.Filters
open Suave.Writers
open Suave.Successful

let user = {
  Id = 1
  Firstname = "Pierre"
  Surname = "Irrmann"
  Email = "john.doe@ex.com"
  Password = "password" }

let mainWebPart =
  choose [
    GET >=> JSON user
  ]

let config =
  { defaultConfig with
     bindings = [ HttpBinding.create HTTP System.Net.IPAddress.Loopback 8081us ] }

[<EntryPoint>]
let main _ =
  startWebServer config mainWebPart
  0