open System
open System.Data
open System.Data.Linq
open FSharp.Data.TypeProviders
open Microsoft.FSharp.Linq
open CoroutineMonad
open Visualization
open Communicatie


[<EntryPoint>]
let main args=
    let test = ["Africa", 1033043; 
                "Asia", 4166741; 
                "Europe", 732759; 
                "South America", 588649; 
                "North America", 351659; 
                "Oceania", 35838]
    let client = BootClient
    do exampleprogram test test

    0