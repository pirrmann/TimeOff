module TimeOff.Server.Server

open Shared.Types

open Suave
open Suave.Operators
open Suave.Filters
open Suave.RequestErrors
open Suave.Successful
open Suave.Writers

let scanSingleStringFormat pattern = new PrintfFormat<(string -> string),unit,string,string,string>(pattern)

let mainWebPart =
  choose [
    GET >=> pathScan (scanSingleStringFormat (Shared.ServerUrls.UserVacation + "%s")) UserVacation.balanceForUser

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