open Communication
open System.Net
open System.Net.Sockets

[<EntryPoint>]
let main args =
  let settings = CreateSettings ()
  let socket = CreateSocket settings
  do ReceiveLoop socket
  0