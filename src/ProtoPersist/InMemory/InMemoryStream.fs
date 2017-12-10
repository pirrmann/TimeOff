namespace ProtoPersist.InMemory

open ProtoPersist

type private InMemoryStream<'TValue> () =
  let stream = ResizeArray<'TValue>()

  interface IStream<'TValue> with
    member __.Read(fromVersion: int) = async {
      if fromVersion = -1 then
        return stream :> seq<_>
      else
        return stream |> Seq.skip (fromVersion + 1) }

    member __.Append (expectedVersion: int) (values:'TValue list) = async {
      let lastVersion = stream.Count - 1

      if expectedVersion <> lastVersion then
        return raise WrongExpectedVersionException
      else
        stream.AddRange values
        return stream.Count - 1 }

type InMemoryStream =
  static member Create<'TValue>() =
    InMemoryStream<'TValue>() :> IStream<'TValue>