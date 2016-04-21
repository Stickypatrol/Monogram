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

let Serialize x =
  Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(x)) //stick a byte with the question number in front to you can read that byte and deserialize the data properly
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

let q1 (dbConnection:dbSchema.ServiceTypes.SimpleDataContextTypes.Project) = //werkt godverdomme wel
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

let q4 (dbConnection:dbSchema.ServiceTypes.SimpleDataContextTypes.Project) =
  let dates = dbConnection.DataContext.ExecuteQuery<int>(@"select COUNT(date) from thefts group by YEAR(Convert(datetime, date ))")
  let dates' = dates |> List.ofSeq
  [2011;2012;2013],[dates'.Item 1;dates'.Item 2;dates'.Item 0] //klopt
  

let q3 (dbConnection:dbSchema.ServiceTypes.SimpleDataContextTypes.Project) = //klopt
  let xylist =
    query{
      for trommel in dbConnection.Trommel do
      select (trommel.X_location, trommel.Y_location)
    }
  let x = Seq.fold (fun (xs, ys) (x, y) -> x::xs, y::ys) ([],[]) xylist
  x

let q2 (dbConnection:dbSchema.ServiceTypes.SimpleDataContextTypes.Project) = //werkt nog niet
  let dates2011 = dbConnection.DataContext.ExecuteQuery<int>(@"select count(date) from thefts where YEAR(Convert(datetime, date)) = '2011' group by month(Convert(datetime, date))")
  let dates2012 = dbConnection.DataContext.ExecuteQuery<int>(@"select count(date) from thefts where YEAR(Convert(datetime, date)) = '2012' group by month(Convert(datetime, date))")
  let dates2013 = dbConnection.DataContext.ExecuteQuery<int>(@"select count(date) from thefts where YEAR(Convert(datetime, date)) = '2013' group by month(Convert(datetime, date))")
  let trommels2011 = dbConnection.DataContext.ExecuteQuery<int>(@"select count(*) from trommel where YEAR(datetime) = '2011' group by MONTH(datetime)")
  let trommels2012 = dbConnection.DataContext.ExecuteQuery<int>(@"select count(*) from trommel where YEAR(datetime) = '2012' group by MONTH(datetime)")
  let trommels2013 = dbConnection.DataContext.ExecuteQuery<int>(@"select count(*) from trommel where YEAR(datetime) = '2013' group by MONTH(datetime)")
  (dates2011|> List.ofSeq)@(dates2012|> List.ofSeq)@(dates2013|> List.ofSeq), (trommels2011|> List.ofSeq)@(trommels2012|> List.ofSeq)@(trommels2013|> List.ofSeq)

let q5 (dbConnection:dbSchema.ServiceTypes.SimpleDataContextTypes.Project) = //werkt
  let areas = dbConnection.DataContext.ExecuteQuery<string>(@"select neighbourhood from thefts group by neighbourhood order by neighbourhood")
  let counts = dbConnection.DataContext.ExecuteQuery<int>(@"select count(neighbourhood) from thefts group by neighbourhood order by neighbourhood")
  (counts |> List.ofSeq), (areas |> List.ofSeq)

let divide7 (x:float list*float list) =
  let y, z = fst x |> List.toArray, snd x |> List.toArray
  ((List.ofArray y.[0..100]), (List.ofArray z.[0..100])),
  ((List.ofArray y.[101..200]), (List.ofArray z.[101..200])),
  ((List.ofArray y.[201..300]), (List.ofArray z.[201..300])),
  ((List.ofArray y.[301..400]), (List.ofArray z.[301..400])),
  ((List.ofArray y.[401..500]), (List.ofArray z.[401..500])),
  ((List.ofArray y.[501..600]), (List.ofArray z.[501..600])),
  ((List.ofArray y.[601..700]), (List.ofArray z.[601..700]))

let SendPart x =
  fun (s:Socket) ->
    ignore <| s.Send(Serialize x)
    Done((), s)

let q3SendLoop x =
  let a,b,c,d,e,f,g = divide7 x
  cor{
    do! SendPart a
    do! wait_ 0.2
    do! SendPart b
    do! wait_ 0.2
    do! SendPart c
    do! wait_ 0.2
    do! SendPart d
    do! wait_ 0.2
    do! SendPart e
    do! wait_ 0.2
    do! SendPart f
    do! wait_ 0.2
    do! SendPart g
    do! wait_ 0.2
    return ()
  }

let SendBackData (clientSocket:Socket) (serverSocket:Socket) dbConnection =
  let buffer = Array.create serverSocket.Available (new Byte())
  let _ = serverSocket.Receive(buffer)
  let questiontype = Encoding.ASCII.GetString(buffer.[0..0])
  printfn "request received is for question %A" questiontype
  match questiontype with
  | "1" ->  ignore <| serverSocket.Send(Serialize (q1 dbConnection))
  | "2" ->  ignore <| serverSocket.Send(Serialize (q2 dbConnection))
  | "3" ->  let _ = Run (q3SendLoop (q3 dbConnection)) serverSocket
            ()
  //ignore <| serverSocket.Send(Serialize (q3 dbConnection))
  | "4" ->  ignore <| serverSocket.Send(Serialize (q4 dbConnection))
  | "5" ->  ignore <| serverSocket.Send(Serialize (q5 dbConnection))
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