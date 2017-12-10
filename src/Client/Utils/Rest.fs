namespace Client

open Fable.Core
open Fable.Core.JsInterop
open Fable.PowerPack
open Fable.PowerPack.Fetch

[<AutoOpen>]
module RestUtils =

  let propsOfToken token = [ requestHeaders [ Authorization ("Bearer " + token) ] ]

  let [<PassGenerics>] parseAs<'T> (fetchedResponse: Response) =
      fetchedResponse.text() |> Promise.map ofJson<'T>