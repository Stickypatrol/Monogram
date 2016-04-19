open Communication
open System.Net
open System.Net.Sockets

[<EntryPoint>]
let main args =
  System.Console.WriteLine("Enter your IP address:")
  let settings = CreateSettings ()
  let socket = CreateSocket settings
  do ReceiveLoop socket
  0