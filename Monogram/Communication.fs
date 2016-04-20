module Communication

open System
open System.Net
open System.Net.Sockets
open System.Text
open CoroutineMonad


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