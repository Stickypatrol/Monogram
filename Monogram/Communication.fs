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
    //ignore <| (Encoding.ASCII.GetString(buffer))
    Done(buffer, (sock, ip))

let DeserializeQ1 buffer = //list of relative safety indexes and list of areas
  fun (sock, ip) ->
    let decoded = Encoding.ASCII.GetString(buffer)
    Done(JsonConvert.DeserializeObject<List<float>*List<string>>(decoded), (sock, ip))

let DeserializeQ2 buffer = //list of dates for thefts and list of dates for trommels
  fun (sock, ip) ->
    let decoded = Encoding.ASCII.GetString(buffer)
    let x = JsonConvert.DeserializeObject<List<string>*List<int>>(decoded)
    let rec foldy s x y =
      match x, y with
      | xh::xt, yh::yt -> foldy (s@[xh,yh]) xt yt
      | xh::xt, [] -> foldy (s@[xh,0]) xt []
      | [], xh::xt -> foldy (s@["",xh]) [] xt
      | [], [] -> s
    Done((foldy [] (fst x) (snd x)), (sock, ip))
    
let DeserializeQ3 a b c d e f g= //list of locations list x list y
  fun (sock, ip) ->
    let (ax,ay),(bx,by),(cx,cy),(dx,dy),(ex,ey),(fx,fy),(gx,gy) =
      JsonConvert.DeserializeObject<List<float>*List<float>>(Encoding.ASCII.GetString(a)),
      JsonConvert.DeserializeObject<List<float>*List<float>>(Encoding.ASCII.GetString(b)),
      JsonConvert.DeserializeObject<List<float>*List<float>>(Encoding.ASCII.GetString(c)),
      JsonConvert.DeserializeObject<List<float>*List<float>>(Encoding.ASCII.GetString(d)),
      JsonConvert.DeserializeObject<List<float>*List<float>>(Encoding.ASCII.GetString(e)),
      JsonConvert.DeserializeObject<List<float>*List<float>>(Encoding.ASCII.GetString(f)),
      JsonConvert.DeserializeObject<List<float>*List<float>>(Encoding.ASCII.GetString(g))
    let finalx, finaly = ax@bx@cx@dx@ex@fx@gx, ay@by@cy@dy@ey@fy@gy
    Done((finalx, finaly), (sock, ip))
    
let DeserializeQ4 (buffer:Byte []) = //list of dates for the from xdate to ydate amount of thefts - List of dates which correspond to 1 theft each
  fun (sock, ip) ->
    let decoded = Encoding.ASCII.GetString(buffer)
    Done(JsonConvert.DeserializeObject<List<String>*List<int>>(decoded), (sock, ip))
        
let DeserializeQ5 buffer = //list of areas and list of theft amounts
  fun (sock, ip) ->
    let decoded = Encoding.ASCII.GetString(buffer)
    Done(JsonConvert.DeserializeObject<List<int>*List<String>>(decoded), (sock, ip))
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
      let! result1 = ReceiveStuff()
      do! wait_ 0.21
      let! result2 = ReceiveStuff()
      do! wait_ 0.21
      let! result3 = ReceiveStuff()
      do! wait_ 0.21
      let! result4 = ReceiveStuff()
      do! wait_ 0.21
      let! result5 = ReceiveStuff()
      do! wait_ 0.21
      let! result6 = ReceiveStuff()
      do! wait_ 0.21
      let! result7 = ReceiveStuff()
      do! wait_ 0.21
      let! result' = DeserializeQ3 result1 result2 result3 result4 result5 result6 result7
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

let print_ x =
  fun s ->
    printfn "%A" x
    Done((), s)

let SendAndReceiveQ1() =
  cor{
    do! SendFunction "1"
    do! print_ "1"
    let! result = ReceiveQuestion1()
    return result
  }

let SendAndReceiveQ2() =
  cor{
    do! SendFunction "2"
    do! print_ "2"
    let! result = ReceiveQuestion2()
    return result
  }

let SendAndReceiveQ3() =
  cor{
    do! SendFunction "3"
    do! print_ "3"
    let! result = ReceiveQuestion3()
    return result
  }

let SendAndReceiveQ4() =
  cor{
    do! SendFunction "4"
    do! print_ "4"
    let! result = ReceiveQuestion4()
    return result
  }

let SendAndReceiveQ5() =
  cor{
    do! SendFunction "5"
    do! print_ "5"
    let! result = ReceiveQuestion5()
    return result
  }

