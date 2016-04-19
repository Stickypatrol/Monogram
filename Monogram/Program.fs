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

[<EntryPoint>]
let main args =
        
        do Application.Run(Interface.form) |> ignore

        0