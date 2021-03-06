module TimeOff.Server.UserVacation

open Shared.Types

open Suave

let balanceForUser (userToDisplay: string) =
  Authorization.withUserRights (fun userRights ctx -> async {
    let connectedUser = userRights.UserName

    //TODO
    let balance = {
      UserName = userToDisplay
      BalanceYear = 2017
      CarriedOver = 0.
      PortionAccruedToDate = 12.
      TakenToDate = 7.
      CurrentBalance = 5.
    }
    
    return! JSON balance ctx
  })