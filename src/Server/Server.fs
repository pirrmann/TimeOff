module TimeOff.Server.Server

open Shared.Types

open Suave
open Suave.Operators
open Suave.Filters
open Suave.RequestErrors
open Suave.Successful
open Suave.Writers

let userRepository =
  ProtoPersist.FileSystem.DirectoryRepository.Create<string, User>(@"..\..\DB\users", id)
  |> ProtoPersist.Agent.AgentRepository.Wrap

let usersPart =
  let restWebPart = RestFul.rest Shared.ServerUrls.Users (scanSingleStringFormat (Shared.ServerUrls.Users + "%s")) userRepository 
  Auth.useTokenWithRole HumanResources (fun _ -> restWebPart)

let mainWebPart =
  choose [

    // Login
    POST >=> path Shared.ServerUrls.Login >=> Auth.login

    // Users management
    pathStarts Shared.ServerUrls.Users >=> usersPart

    // Vacation management
    GET >=> pathScan (scanSingleStringFormat (Shared.ServerUrls.UserVacation + "%s")) UserVacation.balanceForUser

    NOT_FOUND "Page not found."
  ]

let config =
  { defaultConfig with
     bindings = [ HttpBinding.create HTTP System.Net.IPAddress.Loopback 8081us ] }

[<EntryPoint>]
let main _ =
  startWebServer config mainWebPart
  0