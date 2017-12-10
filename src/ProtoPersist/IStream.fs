namespace ProtoPersist

exception WrongExpectedVersionException

type IStream<'TValue> =
  abstract member Read: fromVersion: int -> Async<'TValue seq>
  abstract member Append: expectedVersion: int -> values:'TValue list -> Async<int>

type internal StreamEntry = {
  Version: int
  Payload: string
}
