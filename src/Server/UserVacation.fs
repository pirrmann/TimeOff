module TimeOff.Server.UserVacation

open Shared.Types

open Suave

let balanceForUser (userToDisplay: string) (ctx: HttpContext) =
    Auth.useToken ctx (fun token -> async {

      let connectedUser = token.UserName

      let balance = {
        UserName = "John Doe"
        BalanceYear = 2017
        CarriedOver = 0.
        PortionAccruedToDate = 12.
        TakenToDate = 7.
        CurrentBalance = 5.
      }
      return! JSON balance ctx
    })