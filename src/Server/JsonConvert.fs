namespace TimeOff.Server

open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Suave
open Suave.Operators
open Suave.Http
open Suave.Successful

[<AutoOpen>]
module JsonConvert =
    // Always use the same instance of the converter
    // as it will create a cache to improve performance
    let jsonConverter = Fable.JsonConverter() :> JsonConverter

    let toJson v =
        JsonConvert.SerializeObject(v,  [|jsonConverter|])

    let JSON v =
        v |> toJson |> OK

    let fromJson<'a> json =
        JsonConvert.DeserializeObject<'a>(json, [|jsonConverter|])
