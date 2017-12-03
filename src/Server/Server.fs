module TimeOff.Server.Server

open Shared.Types

open Suave
open Suave.Operators
open Suave.Filters
open Suave.RequestErrors
open Suave.Successful
open Suave.Writers

let user = {
  Id = 1
  Firstname = "Pierre"
  Surname = "Irrmann"
  Email = "john.doe@ex.com"
  Password = "password" }

let mainWebPart =
  choose [
    GET >=> JSON user

    POST >=> path Shared.ServerUrls.Login >=> Auth.login

    NOT_FOUND "Page not found."
  ]

let config =
  { defaultConfig with
     bindings = [ HttpBinding.create HTTP System.Net.IPAddress.Loopback 8081us ] }

[<EntryPoint>]
let main _ =
  startWebServer config mainWebPart
  0