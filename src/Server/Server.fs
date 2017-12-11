module TimeOff.Server.Server

open Shared.Types

open Suave
open Suave.Operators
open Suave.Filters
open Suave.RequestErrors
open Suave.Successful
open Suave.Writers

open ProtoPersist

let mainWebPart (authentifier: Authentifier) (userRepository: IRepository<string, User>) =
  choose [

    // Login
    POST >=> path Shared.ServerUrls.Login >=> authentifier.LoginWebPart

    // All other endpoints require authentication
    authentifier.Authenticate
      >=> choose [

        // Users management
        pathStarts Shared.ServerUrls.Users
          >=> Authorization.whenUserHasRole HumanResources
          >=> RestFul.rest Shared.ServerUrls.Users (scanSingleStringFormat (Shared.ServerUrls.Users + "%s")) userRepository

        // Vacation management
        GET >=> pathScan (scanSingleStringFormat (Shared.ServerUrls.UserVacation + "%s")) UserVacation.balanceForUser
      ]

    NOT_FOUND "Page not found."
  ]