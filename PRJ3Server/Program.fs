open System
open System.Net
open System.Net.Sockets
open AuxTypes
open CoroutineMonad
open Communication

[<EntryPoint>]
let main args =
  System.Console.WriteLine("Enter your IP address:")
  let settings = CreateSettings ()
  let clientSocket, serverSocket = CreateSocket settings
  let dbConnection = AuxTypes.dbSchema.GetDataContext()
  let _ = Run (ReceiveLoop()) (clientSocket, serverSocket, dbConnection)
  0