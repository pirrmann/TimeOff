module Cmd

let ofDelayedMsg (msg:'msg) delay =
  Elmish.Cmd.ofAsync Async.Sleep delay (fun () -> msg) (fun _ -> msg)