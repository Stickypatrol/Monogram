module Communication

open System
open System.Net
open System.Net.Sockets
open System.Text
open CoroutineMonad
open Newtonsoft.Json

let BootClient() =
  let clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
  printfn "clientsocket has been initialized"
  clientSocket//clientsocket is connected
  
let getsocketandip () = (BootClient()), (IPAddress.Parse "145.24.221.121")

let TryConnect () =
  fun (sock:Socket, ip:IPAddress) ->
    //if sock.Poll(1000, SelectMode.SelectWrite) then
      try
        printfn "trying to connect to serversocket..."
        sock.Connect(IPEndPoint(ip, 8888))
        Done(true, (sock, ip))
      with
        | err ->  printfn "an error occured, namely: %A" err
                  Done(false, (sock, ip))
    //else
      //Done(false, (sock, ip))

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
    do! SendLoop questionnumber
  }

let ConnectFunction() =
  cor{
    do! ConnectLoop()
    return ()
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
    printfn "raw data received the first byte is: %A" (Encoding.ASCII.GetString(buffer.[1..buffer.Length-1]))
    Done(buffer, (sock, ip))

let DeserializeQ1 buffer = //list of relative safety indexes and list of areas
  fun (sock, ip) ->
    let decoded = Encoding.ASCII.GetString(buffer)
    //let typeByte = Encoding.ASCII.GetString([|buffer.[0]|])
    //match typeByte with                 //change these types to match the queries sent
    //| "1" ->  
    Done(JsonConvert.DeserializeObject<List<float>*List<string>>(decoded.[1..decoded.Length-1]), (sock, ip))
    //| _   ->  failwith "wrong function matched with wrong msgtype"

let DeserializeQ2 buffer = //list of dates for thefts and list of dates for trommels
  fun (sock, ip) ->
    let decoded = Encoding.ASCII.GetString(buffer)
    let typeByte = Encoding.ASCII.GetString([|buffer.[0]|])
    match typeByte with                 //change these types to match the queries sent
    | "2" ->  let x = JsonConvert.DeserializeObject<List<int>*List<int>>(decoded.[1..decoded.Length-1])
              Done((List.fold2 (fun s x y -> s@[x,y]) [] (fst x) (snd x)), (sock, ip))
    | _   ->  failwith "wrong function matched with wrong msgtype"
    
let DeserializeQ3 buffer = //list of locations list x list y
  fun (sock, ip) ->
    let decoded = Encoding.ASCII.GetString(buffer)
    let typeByte = Encoding.ASCII.GetString([|buffer.[0]|])
    match typeByte with                 //change these types to match the queries sent
    | "3" ->  Done(JsonConvert.DeserializeObject<List<float>*List<float>>(decoded.[1..decoded.Length-1]), (sock, ip))
    | _   ->  failwith "wrong function matched with wrong msgtype"
    
let DeserializeQ4 buffer = //list of dates for the from xdate to ydate amount of thefts - List of dates which correspond to 1 theft each
  fun (sock, ip) ->
    let decoded = Encoding.ASCII.GetString(buffer)
    let typeByte = Encoding.ASCII.GetString([|buffer.[0]|])
    match typeByte with                 //change these types to match the queries sent
    | "4" ->  Done(JsonConvert.DeserializeObject<List<String>*List<int>>(decoded.[1..decoded.Length-1]), (sock, ip))
    | _   ->  failwith "wrong function matched with wrong msgtype"
        
let DeserializeQ5 buffer = //list of areas and list of theft amounts
  fun (sock, ip) ->
    let decoded = Encoding.ASCII.GetString(buffer)
    let typeByte = Encoding.ASCII.GetString([|buffer.[0]|])
    match typeByte with                 //change these types to match the queries sent
    | "5" ->  Done(JsonConvert.DeserializeObject<List<int>*List<String>>(decoded.[1..decoded.Length-1]), (sock, ip))
    | _   ->  failwith "wrong function matched with wrong msgtype"
//DESERIALIZING WORKS, DONT FUCKING TOUCH IT
let rec ReceiveQuestion1 () =
  cor{
    let! receivestuff = CheckReceive()
    if receivestuff then
      let! result = ReceiveStuff()
      let! result' = DeserializeQ1 result
      return result'
    else
      do! yield_
      return! (ReceiveQuestion1 ())
  }
  
let rec ReceiveQuestion2 () =
  cor{
    let! receivestuff = CheckReceive()
    if receivestuff then
      let! result = ReceiveStuff()
      let! result' = DeserializeQ2 result
      return result'
    else
      do! yield_
      return! ReceiveQuestion2 ()
  }

let rec ReceiveQuestion3 () =
  cor{
    let! receivestuff = CheckReceive()
    if receivestuff then
      let! result = ReceiveStuff()
      let! result' = DeserializeQ3 result
      return result'
    else
      do! yield_
      return! ReceiveQuestion3 ()
  }
  
let rec ReceiveQuestion4 () =
  cor{
    let! receivestuff = CheckReceive()
    if receivestuff then
      let! result = ReceiveStuff()
      let! result' = DeserializeQ4 result
      return result'
    else
      do! yield_
      return! ReceiveQuestion4 ()
  }
  
let rec ReceiveQuestion5 () =
  cor{
    let! receivestuff = CheckReceive()
    if receivestuff then
      let! result = ReceiveStuff()
      let! result' = DeserializeQ5 result
      return result'
    else
      do! yield_
      return! ReceiveQuestion5 ()
  }

let SendAndReceiveQ1() =
  cor{
    do! SendFunction "1"
    let! result = ReceiveQuestion1()
    return result
  }

let SendAndReceiveQ2() =
  cor{
    do! SendFunction "2"
    let! result = ReceiveQuestion2()
    return result
  }

let SendAndReceiveQ3() =
  cor{
    do! SendFunction "3"
    let! result = ReceiveQuestion3()
    return result
  }

let SendAndReceiveQ4() =
  cor{
    do! SendFunction "4"
    let! result = ReceiveQuestion4()
    return result
  }

let SendAndReceiveQ5() =
  cor{
    do! SendFunction "5"
    let! result = ReceiveQuestion5()
    return result
  }

