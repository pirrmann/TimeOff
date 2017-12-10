namespace ProtoPersist.FileSystem

open ProtoPersist
open System.IO

type private FileStream<'TValue> (path: string) =
  interface IStream<'TValue> with
    member __.Read(fromVersion: int) = async {
      let values = new ResizeArray<'TValue>()
      use reader = new StreamReader(path)
      let mutable keepLooping = true
      while keepLooping do
        let! line = reader.ReadLineAsync() |> Async.AwaitTask
        if isNull line then
          keepLooping <- false
        else
          let splitIndex = line.IndexOf(":")
          let version = int (line.Substring(0, splitIndex))
          if version > fromVersion then
            let json = line.Substring(splitIndex)
            let value = Serialization.deserialize<'TValue> json
            values.Add value
      return values :> seq<_> }

    member __.Append (expectedVersion: int) (values:'TValue list) = async {
      let stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite)
      use reader = new StreamReader(stream)
      let mutable keepLooping = true
      let mutable lastLine: string = null
      while keepLooping do
        let! line = reader.ReadLineAsync() |> Async.AwaitTask
        if isNull line then
          keepLooping <- false
        else
          lastLine <- line

      let mutable lastVersion =
        if isNull lastLine then -1
        else
          let splitIndex = lastLine.IndexOf(":")
          int (lastLine.Substring(0, splitIndex))

      reader.Close()

      if expectedVersion <> lastVersion then
        return raise WrongExpectedVersionException
      else
        use writer = new StreamWriter(stream)
        for value in values do
          lastVersion <- lastVersion + 1
          let json = Serialization.serialize value
          let line = sprintf "%d:%s" lastVersion json
          do! writer.WriteLineAsync line |> Async.AwaitTask
        return lastVersion }

type FileStream =
  static member Create<'TValue>(fileName) =
    FileStream<'TValue>(fileName) :> IStream<'TValue>