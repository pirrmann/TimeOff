module Client.Users.Rest

open Fable.PowerPack
open Fable.PowerPack.Fetch

open Shared.Types
open Client

let getUsers token () =
    promise {
        let props = propsOfToken token
        return! fetchAs<User list> Shared.ServerUrls.Users props
    }

let getUser token userName =
    promise {
        let props = propsOfToken token
        return! fetchAs<User> (Shared.ServerUrls.Users + userName) props
    }

let createUser token (user: User) =
    promise {
        let props = propsOfToken token
        let! response = postRecord (Shared.ServerUrls.Users + user.UserName) user props
        return! parseAs<User> response
    }

let updateUser token (user: User) =
    promise {
        let props = propsOfToken token
        let! response = putRecord (Shared.ServerUrls.Users + user.UserName) user props
        return! parseAs<User> response
    }
