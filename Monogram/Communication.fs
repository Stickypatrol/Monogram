module Communication

open System
open System.Net
open System.Net.Sockets
open System.Text
open CoroutineMonad
open Newtonsoft.Json

let BootClient() =
  let clientSocket = new Socket(SocketType.Stream, ProtocolType.Tcp)
  printfn "clientsocket has been initialized"
  clientSocket//clientsocket is connected
  
let socket, ip = (BootClient()), (IPAddress.Parse "145.24.200.232")
(*
let rec TryConnect (clientSocket : Socket) (connectIP : IPAddress) =
  if clientSocket.Poll(1000, SelectMode.SelectWrite) || 1=1 then
    try
      printfn "clientsocket is attempting connection..."
      clientSocket.Connect(IPEndPoint(connectIP, 8888))
    with
      | err -> printfn "an error occurred, this one: %A" err
  else
    printfn "no connection available"
  clientSocket

let SendData (clientSocket:Socket) =
  printfn "trying to send data, input some shit please"
  let dataToSend = Console.ReadLine()
  let dataAsBytes = Encoding.ASCII.GetBytes(dataToSend)
  let _ = clientSocket.Send(dataAsBytes)
  ()

let rec TrySendData (clientSocket:Socket) =
  if clientSocket.Poll(1000, SelectMode.SelectWrite) then
    SendData clientSocket
  TrySendData clientSocket
  ()
  *)
let TryConnect () =
  fun (sock:Socket, ip:IPAddress) ->
    if sock.Poll(1000, SelectMode.SelectWrite) then
      try
        printfn "trying to connect to serversocket..."
        sock.Connect(IPEndPoint(ip, 8888))
        Done(true, (sock, ip))
      with
        | err ->  printfn "an error occured, namely: %A" err
                  Done(false, (sock, ip))
    else
      Done(false, (sock, ip))

let rec ConnectLoop() =
  cor{
    let! isConnected = TryConnect()
    if isConnected then
      return ()
    else
      do! yield_
      return! ConnectLoop ()
  }

let TryPoll () =
  fun (sock:Socket, ip:IPAddress) ->
    Done(sock.Poll(1000, SelectMode.SelectWrite), (sock, ip))

let Send (msg:string) =
  fun (sock:Socket, ip:IPAddress) ->
    let serialized = Encoding.ASCII.GetBytes(msg)
    ignore <| sock.Send(serialized)
    Done((), (sock, ip))


let rec SendLoop questionnumber =
  cor{
    let! canSend = TryPoll ()
    if canSend then
      do! Send questionnumber
    else
      do! yield_
      return! SendLoop questionnumber
    }

//THE ACTUAL PROGRAM
let SendFunction questionnumber =
  cor{
    do! ConnectLoop()
    do! SendLoop questionnumber
  }

let CheckReceive() =
  fun (sock:Socket, ip:IPAddress) ->
    if sock.Available > 0 then
      Done(true, (sock, ip))
    else
      Done(false, (sock, ip))

let ReceiveStuff() =
  fun (sock:Socket, ip:IPAddress) ->
    let buffer = Array.create sock.Available (new Byte())
    let _ = sock.Receive(buffer)
    printfn "raw data received: %A" (Encoding.ASCII.GetString(buffer))
    Done(buffer, (sock, ip))

let DeserializeQ1 buffer = //make all functions like this
  fun (sock, ip) ->
    let decoded = Encoding.ASCII.GetString(buffer)
    let typeByte = Encoding.ASCII.GetString([|buffer.[0]|])
    match typeByte with                 //change these types to match the queries sent
    | "1" ->  Done(JsonConvert.DeserializeObject<List<string*int>>(decoded.[1..decoded.Length-1]), (sock, ip))
    | _   ->  failwith "wrong function matched with wrong msgtype"

let DeserializeQ2 buffer = //and so on...
  fun (sock, ip) ->
    let decoded = Encoding.ASCII.GetString(buffer)
    let typeByte = Encoding.ASCII.GetString([|buffer.[0]|])
    match typeByte with                 //change these types to match the queries sent
    | "2" ->  Done(JsonConvert.DeserializeObject<(*change this to the appropriate type for the question*)>(decoded.[1..decoded.Length-1]), (sock, ip))
    | _   ->  failwith "wrong function matched with wrong msgtype"

let rec ReceiveQuestion1 () = //return answer to question 1 here and deserialize it
  cor{
    let! receivestuff = CheckReceive()
    if receivestuff then
      let! result = ReceiveStuff()
      return DeserializeQ1 result
    else
      do! yield_
      return! ReceiveQuestion1 ()
  }
  
let ReceiveQuestion2 () =
  cor{
    let! receivestuff = CheckReceive()
    if receivestuff then
      let! result = ReceiveStuff()
      return DeserializeQ2 result
    else
      do! yield_
      return! ReceiveQuestion1 ()
  }
  
let ReceiveQuestion3 () =
  cor{
    let! receivestuff = CheckReceive()
    if receivestuff then
      let! result = ReceiveStuff()
      return DeserializeQ3 result
    else
      do! yield_
      return! ReceiveQuestion1 ()
  }
  
let ReceiveQuestion4 () =
  cor{
    let! receivestuff = CheckReceive()
    if receivestuff then
      let! result = ReceiveStuff()
      return DeserializeQ4 result
    else
      do! yield_
      return! ReceiveQuestion1 ()
  }
  
let ReceiveQuestion5 () =
  cor{
    let! receivestuff = CheckReceive()
    if receivestuff then
      let! result = ReceiveStuff()
      return DeserializeQ5 result
    else
      do! yield_
      return! ReceiveQuestion1 ()
  }