module Client.Balance.Rest

open Fable.PowerPack
open Fable.PowerPack.Fetch

open Shared.Types

let getUserBalance token userName =
    promise {
        let props = [
            requestHeaders [ Authorization ("Bearer " + token) ]
        ]
        let! res = fetchAs<UserVacationBalance> (Shared.ServerUrls.UserVacation + userName) props
        return res
    }
