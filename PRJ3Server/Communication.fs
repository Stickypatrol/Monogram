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

let Serialize (i:int) x =
  Array.concat [|[|(byte i)|];(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(x)))|] //stick a byte with the question number in front to you can read that byte and deserialize the data properly
  //this is probably not even necessary

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

let q1 (dbConnection:dbSchema.ServiceTypes.SimpleDataContextTypes.Project) = //werkt nog niet
  let x =
    query{
      for row in dbConnection.Thefts do
      select row
    }
  x
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

let q3 (dbConnection:dbSchema.ServiceTypes.SimpleDataContextTypes.Project) =
  let x =
    query{
      for row in dbConnection.Trommel do
      select (row.X_location, row.Y_location)
    }
  Seq.fold (fun s xy -> xy::s) [] x

let q4 (dbConnection:dbSchema.ServiceTypes.SimpleDataContextTypes.Project) = //werkt nog niet
  let x =
    query{
      for row in dbConnection.Thefts do
      select row.Date
    }
  x

let q5 (dbConnection:dbSchema.ServiceTypes.SimpleDataContextTypes.Project) = //werkt nog niet
  let x =
    query{
      for x in dbConnection.Thefts do
      groupBy x.Neighbourhood into g
      select (g.Key)
    }
  x
  

let WriteSentData (socket:Socket) dbConnection =
  let buffer = Array.create socket.Available (new Byte())
  let _ = socket.Receive(buffer)
  let questiontype = Encoding.ASCII.GetString(buffer.[0..0])
  printfn "request received is for question %A" questiontype
  match questiontype with
  | "1" -> ignore <| socket.Send(Serialize 1 (q1 dbConnection))
  | "2" ->  let x = Serialize 2 (q2 dbConnection)//i'm sending data in Byte[] JSON format to the client here
            printfn "%A" x
            ignore <| socket.Send(x)
  | "3" -> ignore <| socket.Send(Serialize 3 (q3 dbConnection))
  | "4" -> ignore <| socket.Send(Serialize 4 (q4 dbConnection))
  | _ -> ignore <| socket.Send(Serialize 5 (q5 dbConnection))
  let _ = (socket.Blocking = false)
  ()

//ACTUAL PROGRAM
let CreateSettings () = {LocalIP = (IPAddress.Parse (Console.ReadLine())); LocalPort = 8888};

let CreateSocket settings = connectClient (BootProgram settings)

let rec ReceiveLoop() =
  cor{
    let! serverSocket, dbConnection = getState()
    if serverSocket.Available > 0 then
      WriteSentData serverSocket dbConnection
      do! ReceiveLoop ()
      return ()
    else
      do! yield_
      return! ReceiveLoop ()
  }