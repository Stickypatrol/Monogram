open Communication
open System.Net
open System.Net.Sockets
open AuxTypes

[<EntryPoint>]
let main args =
  let settings = CreateSettings ()
  let socket = CreateSocket settings
  let dbConnection = AuxTypes.dbSchema.GetDataContext()
  do ReceiveLoop socket dbConnection
  0