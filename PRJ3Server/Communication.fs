﻿module Communication

open System
open System.Text
open System.Net
open System.Net.Sockets
open FSharp.Charting
open FSharp.Charting.ChartTypes
open System.Drawing
open System.Windows.Forms
open CoroutineMonad
open Newtonsoft.Json
open AuxTypes

type Settings =
  {
    LocalIP : IPAddress
    LocalPort : int
  }

type ProgramState =
  {
    LocalSettings : Settings
    ClientSockets : List<Socket>
  }

let Serialize (x:List<string* int>) =
  Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(x))

let BootProgram (settings : Settings) =
  let serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
  printfn "Server has started..."
  serverSocket.Bind(IPEndPoint(settings.LocalIP, settings.LocalPort))
  printfn "socket is bound to IP \"%A\" and port \"%A\"..." settings.LocalIP settings.LocalPort
  serverSocket.Listen 60
  printfn "socket is listening for connections..."
  serverSocket

let rec connectClient (serverSocket:Socket) =
  if serverSocket.Poll(1000, SelectMode.SelectRead) then
    printfn "accepting client"
    (serverSocket.Accept())
  else
    connectClient serverSocket

let q2 (dbConnection:dbSchema.ServiceTypes.SimpleDataContextTypes.Project) =
  let x =
    query{
      for row in dbConnection.Thefts do
      select row
    }
  let y =
    x |> Seq.fold (fun s x -> match Map.tryFind x.Date s with
                              | Some(v) -> Map.add x.Date (v+1) s
                              | None -> Map.add x.Date 1 s) Map.empty
  let z = Map.toList y
  z

let WriteSentData (socket:Socket) (dbConnection:dbSchema.ServiceTypes.SimpleDataContextTypes.Project) =
  let buffer = Array.create socket.Available (new Byte())
  let _ = socket.Receive(buffer)
  let questiontype = Encoding.ASCII.GetString(buffer.[0..0])
  printfn "request received is for question %A" questiontype
  match questiontype with
  //| "1" -> socket.Send(Serialize (dbConnection))
  | "2" ->  let x = Serialize (q2 dbConnection)
            printfn "%A" x
            ignore <| socket.Send(x)
  //| "3" -> socket.Send(Serialize (q3 dbConnection))
  //| "4" -> socket.Send(Serialize (q4 dbConnection))
  //| _ -> socket.Send(Serialize (q5 dbConnection))
  | _ -> failwith "not yet implemented"
  let _ = (socket.Blocking = false)
  ()

//ACTUAL PROGRAM
let CreateSettings () = {LocalIP = (IPAddress.Parse (Console.ReadLine())); LocalPort = 8888};

let CreateSocket settings = connectClient (BootProgram settings)

let rec ReceiveLoop (serverSocket : Socket) (dbConnection:dbSchema.ServiceTypes.SimpleDataContextTypes.Project) =
  if (serverSocket.Available > 0) then
    WriteSentData serverSocket dbConnection
  ReceiveLoop serverSocket dbConnection
