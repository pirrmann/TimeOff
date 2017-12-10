module TimeOff.Server.RestFul

open Suave
open Suave.Filters
open Suave.Operators
open Suave.RequestErrors
open Suave.Successful

let getResourceFromReq<'a> (req : HttpRequest) =
  let getString rawForm = System.Text.Encoding.UTF8.GetString(rawForm)
  req.rawForm |> getString |> fromJson<'a>

let rest<'TKey, 'TValue when 'TKey: comparison> resourcePath (resourceKeyPath:PrintfFormat<('TKey->string),unit,string,string,'TKey>) (repository: ProtoPersist.IRepository<'TKey, 'TValue>) =
  let badRequest = BAD_REQUEST "Resource not found"

  let handleResource requestError resource =
    match resource with
    | Some r -> r |> JSON
    | _ -> requestError

  let getAll ctx = async {
    let! values = repository.ReadAll()
    return! JSON values ctx }

  let getResourceByKey key ctx = async {
    let! value = repository.Read key
    return! handleResource (NOT_FOUND "Resource not found") value ctx
  }

  let createResource key (ctx: HttpContext) = async {
    let value = getResourceFromReq<'TValue> ctx.request
    let! created = repository.Create key value
    return! JSON created ctx }

  let updateResource key (ctx: HttpContext) = async {
    let value = getResourceFromReq<'TValue> ctx.request
    let! updated = repository.Update key value
    return! handleResource badRequest updated ctx }

  let deleteResource key ctx = async {
    do! repository.Delete key
    return! NO_CONTENT ctx }

  choose [
      path resourcePath >=> choose [
          GET >=> getAll
      ]
      GET >=> pathScan resourceKeyPath getResourceByKey
      POST >=> pathScan resourceKeyPath createResource
      PUT >=> pathScan resourceKeyPath updateResource
      DELETE >=> pathScan resourceKeyPath deleteResource
  ]