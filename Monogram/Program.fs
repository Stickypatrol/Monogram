open System
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
    let clientsocket = BootClient()
    printfn "starting client loop"
    do MainClientLoop clientsocket
    0