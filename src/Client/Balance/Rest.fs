module Client.Balance.Rest

open Fable.PowerPack
open Fable.PowerPack.Fetch
open Shared.Types
open Client

let getUserBalance token userName =
    promise {
        let props = propsOfToken token
        return! fetchAs<UserVacationBalance> (Shared.ServerUrls.UserVacation + userName) props
    }
