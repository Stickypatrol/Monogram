open System
open System.Net
open System.Net.Sockets
open System.Data
open System.Data.Linq
open FSharp.Data.TypeProviders
open Microsoft.FSharp.Linq
open FSharp.Charting
open CoroutineMonad
open Visualization
open Interface
open System.Windows.Forms
open System.Drawing
open Communication


[<EntryPoint>]
let main args =
    
    do Application.Run(Interface.form) |> ignore
    //let clientsocket = BootClient() these 3 lines shouldnt be necessary
    //printfn "starting client loop"
    //do Run MainClientLoop clientsocket
    0