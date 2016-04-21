module Visualization
open System
open FSharp.Charting
open FSharp.Charting.ChartTypes
open System.Drawing
open System.Windows.Forms
open CoroutineMonad

[<STAThread>]
let first_Chart (list1 : List<'a*'b>) (list2 : List<'a*'b>) =
    let myChart = Chart.Column(list1, Name = "Thefts") |> Chart.WithYAxis(Title="Test")
    let myChart2 = Chart.Column(list2, Name = "Trommels") |> Chart.WithYAxis(Title="Test") |> Chart.WithLegend
                                                                        ( InsideArea = false, 
                                                                        Alignment = StringAlignment.Center, Docking = Docking.Top)
    let custom_Chart = Chart.Combine([myChart;myChart2])
    let myChartControl = new ChartControl(custom_Chart, Dock=DockStyle.Fill)
    myChartControl

let second_Chart (list1 : List<'a*'a>) (list2 : List<'a*'a>) =
    let myChart = Chart.Column(list1, Name = "Thefts") |> Chart.WithYAxis(Title="Test")
    let myChart2 = Chart.Column(list2, Name = "Trommels") |> Chart.WithYAxis(Title="Test") |> Chart.WithLegend
                                                                        ( InsideArea = false, 
                                                                        Alignment = StringAlignment.Center, Docking = Docking.Top)
    let custom_Chart = Chart.Combine([myChart;myChart2])
    let myChartControl = new ChartControl(custom_Chart, Dock=DockStyle.Fill)
    myChartControl
let third_Chart (list1 : List<'a*'a>) (list2 : List<'a*'a>) =
    let myChart = Chart.Column(list1, Name = "Thefts") |> Chart.WithYAxis(Title="Test")
    let myChart2 = Chart.Column(list2, Name = "Trommels") |> Chart.WithYAxis(Title="Test") |> Chart.WithLegend
                                                                        ( InsideArea = false, 
                                                                        Alignment = StringAlignment.Center, Docking = Docking.Top)
    let custom_Chart = Chart.Combine([myChart;myChart2])
    let myChartControl = new ChartControl(custom_Chart, Dock=DockStyle.Fill)
    myChartControl

let fourth_Chart (list1 : List<'a*'a>) (list2 : List<'a*'a>) =
    let myChart = Chart.Column(list1, Name = "Thefts") |> Chart.WithYAxis(Title="Test")
    let myChart2 = Chart.Column(list2, Name = "Trommels") |> Chart.WithYAxis(Title="Test") |> Chart.WithLegend
                                                                        ( InsideArea = false, 
                                                                        Alignment = StringAlignment.Center, Docking = Docking.Top)
    let custom_Chart = Chart.Combine([myChart;myChart2])
    let myChartControl = new ChartControl(custom_Chart, Dock=DockStyle.Fill)
    myChartControl

let fifth_Chart (list1 : List<'a*'a>) (list2 : List<'a*'a>) =
    let myChart = Chart.Column(list1, Name = "Thefts") |> Chart.WithYAxis(Title="Test")
    let myChart2 = Chart.Column(list2, Name = "Trommels") |> Chart.WithYAxis(Title="Test") |> Chart.WithLegend
                                                                        ( InsideArea = false, 
                                                                        Alignment = StringAlignment.Center, Docking = Docking.Top)
    let custom_Chart = Chart.Combine([myChart;myChart2])
    let myChartControl = new ChartControl(custom_Chart, Dock=DockStyle.Fill)
    myChartControl
