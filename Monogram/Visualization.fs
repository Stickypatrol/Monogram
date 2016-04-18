module Visualization
open System
open FSharp.Charting
open FSharp.Charting.ChartTypes
open System.Drawing
open System.Windows.Forms
open CoroutineMonad

[<STAThread>]
let exampleprogram () =
    let myChart = [for x in 0.0 .. 0.1 .. 6.0 -> sin x + cos (2.0 * x)]
                    |> Chart.Line |> Chart.WithYAxis(Title="Test")
    let myChartControl = new ChartControl(myChart, Dock=DockStyle.Fill)
    let lbl = new Label(Text="my label")
    let form = new Form(Visible = true, TopMost = true, Width = 700, Height = 500)
    form.Controls.Add lbl
    form.Controls.Add(myChartControl)
    do Application.Run(form) |> ignore
    ()