module Communication

open System
open System.Text
open System.Net
open System.Net.Sockets
open FSharp.Charting
open FSharp.Charting.ChartTypes
open System.Drawing
open System.Windows.Forms
open Newtonsoft.Json
open AuxTypes
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
    let clientSocket = serverSocket.Accept()
    clientSocket, serverSocket
  else
    connectClient serverSocket

let q1 (dbConnection:dbSchema.ServiceTypes.SimpleDataContextTypes.Project) = //werkt nog niet
  let x =
    query{
      for theft in dbConnection.Thefts do
      groupBy theft.Neighbourhood into g
      select g.Key
    }
  let y area =
    query{
      for theft in dbConnection.Thefts do
      where (theft.Neighbourhood = area)
      select theft
      count
    }
  let theftcounts = List.fold (fun s x -> s@[y x]) [] (x |> List.ofSeq)
  let modifier = 1.0/(List.fold (fun s x -> if float x > s then float x else s) 0.0 theftcounts)
  let relsafety = List.map (fun x -> (float x) * modifier) theftcounts
  relsafety, (x |> List.ofSeq)

let q2 (dbConnection:dbSchema.ServiceTypes.SimpleDataContextTypes.Project) =
  let dates = dbConnection.DataContext.ExecuteQuery<int>(@"select COUNT(date) from thefts group by YEAR(Convert(datetime, date ))")
  let dates' = dates |> List.ofSeq
  [2011;2012;2013],[dates'.Item 1;dates'.Item 2;dates'.Item 0]
  

let q3 (dbConnection:dbSchema.ServiceTypes.SimpleDataContextTypes.Project) =
  let xylist =
    query{
      for trommel in dbConnection.Trommel do
      select (trommel.X_location, trommel.Y_location)
    }
  Seq.fold (fun (xs, ys) (x, y) -> x::xs, y::ys) ([],[]) xylist

let q4 (dbConnection:dbSchema.ServiceTypes.SimpleDataContextTypes.Project) = //werkt nog niet
  let dates = dbConnection.DataContext.ExecuteQuery<string>(@"select date from thefts group by date order by Convert(datetime, date)")
  let trommelsperdate = dbConnection.DataContext.ExecuteQuery<int>(@"select Count(date) from thefts group by date order by Convert(datetime, date)") 
  dates |> List.ofSeq, trommelsperdate |> List.ofSeq

let q5 (dbConnection:dbSchema.ServiceTypes.SimpleDataContextTypes.Project) =
  let areas = dbConnection.DataContext.ExecuteQuery<string>(@"select neighbourhood from thefts group by neighbourhood order by neighbourhood")
  let counts = dbConnection.DataContext.ExecuteQuery<int>(@"select count(neighbourhood) from thefts group by neighbourhood order by neighbourhood")
  List.fold2 (fun (counts, areas) count area -> count::counts, area::areas) ([],[]) (counts |> List.ofSeq) (areas |> List.ofSeq)

let SendBackData (clientSocket:Socket) (serverSocket:Socket) dbConnection =
  let buffer = Array.create serverSocket.Available (new Byte())
  let _ = serverSocket.Receive(buffer)
  let questiontype = Encoding.ASCII.GetString(buffer.[0..0])
  printfn "request received is for question %A" questiontype
  match questiontype with
  | "1" ->  ignore <| serverSocket.Send(Serialize 1 (q1 dbConnection))
  | "2" ->  ignore <| serverSocket.Send(Serialize 2 (q2 dbConnection))
  | "3" ->  ignore <| serverSocket.Send(Serialize 3 (q3 dbConnection))
  | "4" ->  ignore <| serverSocket.Send(Serialize 4 (q4 dbConnection))
  | "5" ->  ignore <| serverSocket.Send(Serialize 5 (q5 dbConnection))
  | _ -> failwith "incorrect byte received"
  let _ = (clientSocket.Blocking = false)
  ()

//ACTUAL PROGRAM
//let CreateSettings () = {LocalIP = (IPAddress.Parse (Console.ReadLine())); LocalPort = 8888};
let CreateSettings () = {LocalIP = (IPAddress.Parse "145.24.221.121"); LocalPort = 8888};

let CreateSocket settings = connectClient (BootProgram settings)

let print_ x =
  fun s ->
    printfn "%A" x
    Done((), s)

let rec ReceiveLoop() =
  cor{
    let! (serverSocket : Socket), (clientSocket : Socket), dbConnection = getState()
    if serverSocket.Available > 0 then
      SendBackData clientSocket serverSocket dbConnection
      do! ReceiveLoop ()
      return ()
    else
      do! yield_
      return! ReceiveLoop ()
  }