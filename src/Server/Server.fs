module TimeOff.Server.Server

open Shared.Types

open Suave
open Suave.Operators
open Suave.Filters
open Suave.RequestErrors
open Suave.Successful
open Suave.Writers

open ProtoPersist

let mainWebPart (authentifier: Authentifier) (employeeRepository: IRepository<string, Employee>) =
  choose [

    // Login
    POST >=> path Shared.ServerUrls.Login >=> authentifier.LoginWebPart

    // All other endpoints require authentication
    authentifier.Authenticate
      >=> choose [

        // Employee management
        pathStarts Shared.ServerUrls.Employees
          >=> Authorization.whenUserHasRole HumanResources
          >=> RestFul.rest Shared.ServerUrls.Employees (scanSingleStringFormat (Shared.ServerUrls.Employees + "%s")) employeeRepository

        // Vacation management
        GET >=> pathScan (scanSingleStringFormat (Shared.ServerUrls.UserVacation + "%s")) UserVacation.balanceForUser
      ]

    NOT_FOUND "Page not found."
  ]