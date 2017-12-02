module Counter.Types

open Shared.Types

type Model = {
    Counter: int
    User: User option
}

type Msg =
  | Increment
  | Decrement
  | Reset
  | FetchUser
  | DisplayUser of User
  | NetworkError of exn
