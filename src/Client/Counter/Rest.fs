module Client.Counter.Rest

open Fable.PowerPack
open Fable.PowerPack.Fetch

open Shared.Types

let getUser () =
    promise {
        let url = "/api"
        let! res = fetchAs<User> url []
        return res
    }
