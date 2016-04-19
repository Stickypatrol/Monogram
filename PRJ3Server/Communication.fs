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

let Serialize x =
  

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

let WriteSentData (socket:Socket) =
  let buffer = Array.create socket.Available (new Byte())
  let _ = socket.Receive(buffer)
  let questiontype = Encoding.ASCII.GetString(buffer.[0..0])
  printfn "request received is for question %A" questiontype
  match questiontype with
  | 1 -> //hier moeten de queries komen te staan d.m.v. het vraagnummer dat je binnenkrijgt
  let x = Console.ReadLine()
  let y = Encoding.ASCII.GetBytes(x)
  ignore <| socket.Send(y)
  let _ = (socket.Blocking = false)
  ()

//ACTUAL PROGRAM
let CreateSettings () = {LocalIP = (IPAddress.Parse (Console.ReadLine())); LocalPort = 8888};

let CreateSocket settings = connectClient (BootProgram settings)

let rec ReceiveLoop (serverSocket : Socket) =
  if (serverSocket.Available > 0) then
    WriteSentData serverSocket
  ReceiveLoop serverSocket