open System
open System.Data
open System.Data.Linq
open FSharp.Data.TypeProviders
open Microsoft.FSharp.Linq
open CoroutineMonad
open Visualization
//
//type DBSchema = SqlDataConnection<"Data Source=145.24.200.232\SQLEXPRESS;Initial Catalog=project;User ID=mustafa;Password=root">
//let db = DBSchema.GetDataContext()
//
//let query1 =
//        query {
//            for row in db.Thefts do
//            select row
//        }
//query1 |> Seq.iter (fun row -> printfn "%A %A" row.Date row.Time)
[<EntryPoint>]
let main args =
    do exampleprogram ()

    0



