open System
open System.Data
open System.Data.Linq
open FSharp.Data.TypeProviders
open Microsoft.FSharp.Linq
open CoroutineMonad
open Visualization
open Communication

[<EntryPoint>]
let main args =
    let clientsocket = BootClient()
    printfn "starting client loop"
    do MainClientLoop clientsocket
    0