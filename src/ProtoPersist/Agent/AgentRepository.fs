namespace ProtoPersist.Agent

open ProtoPersist
open System.Threading.Tasks

type private RepositoryCommand<'TKey, 'TValue> =
  | Create of 'TKey * 'TValue * TaskCompletionSource<'TValue>
  | Read of 'TKey * TaskCompletionSource<'TValue option>
  | ReadAll of TaskCompletionSource<'TValue seq>
  | Update of 'TKey * 'TValue * TaskCompletionSource<'TValue option>
  | Delete of 'TKey * TaskCompletionSource<unit>
  
/// This is just a wrapper which hides a repository behind a mailbox,
/// in order to process a single command at a time
type AgentRepository<'TKey, 'TValue when 'TKey: comparison> (wrappedRepository: IRepository<'TKey, 'TValue>) =

  let tryAndSignal (tcs:TaskCompletionSource<'a>) (asyncResult: Async<'a>) = async {
    try
      let! value = asyncResult
      tcs.SetResult value
    with exn ->
      tcs.SetException exn
    return () }

  let agent =
    MailboxProcessor.Start (
      fun inbox ->
        let rec loop () = async {
          let! command = inbox.Receive()
          match command with
          | Create (key, value, tcs) ->
            do! wrappedRepository.Create key value |> tryAndSignal tcs
          | Read (key, tcs) ->
            do! wrappedRepository.Read key |> tryAndSignal tcs
          | ReadAll tcs ->
            do! wrappedRepository.ReadAll() |> tryAndSignal tcs
          | Update (key, value, tcs) ->
            do! wrappedRepository.Update key value |> tryAndSignal tcs
          | Delete (key, tcs) ->
            do! wrappedRepository.Delete key |> tryAndSignal tcs

          return! loop()
        }
        loop ()
    )

  interface IRepository<'TKey, 'TValue> with
    member __.Create (key: 'TKey) (value: 'TValue) = async {
      let tcs = TaskCompletionSource<_>()
      agent.Post(Create(key, value, tcs))
      return! tcs.Task |> Async.AwaitTask }

    member __.Read (key: 'TKey) = async {
      let tcs = TaskCompletionSource<_>()
      agent.Post(Read(key, tcs))
      return! tcs.Task |> Async.AwaitTask }

    member __.ReadAll() = async {
      let tcs = TaskCompletionSource<_>()
      agent.Post(ReadAll(tcs))
      return! tcs.Task |> Async.AwaitTask }

    member __.Update (key: 'TKey) (value: 'TValue) = async {
      let tcs = TaskCompletionSource<_>()
      agent.Post(Update(key, value, tcs))
      return! tcs.Task |> Async.AwaitTask }

    member __.Delete (key: 'TKey) = async {
      let tcs = TaskCompletionSource<_>()
      agent.Post(Delete(key, tcs))
      return! tcs.Task |> Async.AwaitTask }

type AgentRepository =
  static member Wrap<'TKey, 'TValue when 'TKey: comparison>(wrappedRepository: IRepository<'TKey, 'TValue>) =
    AgentRepository<'TKey, 'TValue>(wrappedRepository) :> IRepository<'TKey, 'TValue>