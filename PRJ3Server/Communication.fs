module Communication

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

let Serialize (x:List<string* string>) =
  JsonConvert.SerializeObject(x)

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
    x |> Seq.fold (fun s x -> if (List.exists (fun y -> y) s) then x.::s else x::s) []
  dbConnection.Thefts

let WriteSentData (socket:Socket) (dbConnection:dbSchema.ServiceTypes.SimpleDataContextTypes.Project) =
  let buffer = Array.create socket.Available (new Byte())
  let _ = socket.Receive(buffer)
  let questiontype = Encoding.ASCII.GetString(buffer.[0..0])
  printfn "request received is for question %A" questiontype
  match questiontype with
  //| "1" -> socket.Send(Serialize (dbConnection))
  | "2" -> socket.Send(Serialize (q2 dbConnection))
  | "3" -> socket.Send(Serialize ())
  | "4" -> socket.Send(Serialize ())
  | _ -> socket.Send(Serialize ())
  let x = Console.ReadLine()
  let y = Encoding.ASCII.GetBytes(x)
  ignore <| socket.Send(y)
  let _ = (socket.Blocking = false)
  ()

//ACTUAL PROGRAM
let CreateSettings () = {LocalIP = (IPAddress.Parse (Console.ReadLine())); LocalPort = 8888};

let CreateSocket settings = connectClient (BootProgram settings)

let rec ReceiveLoop (serverSocket : Socket) (dbConnection:dbSchema.ServiceTypes.SimpleDataContextTypes.Project) =
  if (serverSocket.Available > 0) then
    WriteSentData serverSocket dbConnection
  ReceiveLoop serverSocket dbConnection