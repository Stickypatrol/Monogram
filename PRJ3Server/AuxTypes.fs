module AuxTypes

open System
open System.Data
open System.Data.Linq
open FSharp.Data.TypeProviders
open Microsoft.FSharp.Linq

type dbSchema = SqlDataConnection<"Data Source=145.24.200.232\SQLEXPRESS;Initial Catalog=project;User ID=mustafa;Password=root;">

