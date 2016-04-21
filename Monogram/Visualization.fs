module Visualization
open System
open FSharp.Charting
open FSharp.Charting.ChartTypes
open System.Drawing
open System.Windows.Forms
open CoroutineMonad

[<STAThread>] //amount of thefts per area List<float, string>
let first_Chart (list1 : List<'a*'b>) (list2 : List<'a*'b>) =
    let myChart = Chart.Column(list1, Name = "Thefts") |> Chart.WithYAxis(Title="Test")
    let myChart2 = Chart.Column(list2, Name = "Trommels") |> Chart.WithYAxis(Title="Test") |> Chart.WithLegend
                                                                        ( InsideArea = false, 
                                                                        Alignment = StringAlignment.Center, Docking = Docking.Top)
    let custom_Chart = Chart.Combine([myChart;myChart2])
    let myChartControl = new ChartControl(custom_Chart, Dock=DockStyle.Fill)
    myChartControl
    //dates of thefts and trommels respectively List<string, string>
let second_Chart (list1 : List<'a*'a>) (list2 : List<'a*'a>) =
    let myChart = Chart.Column(list1, Name = "Thefts") |> Chart.WithYAxis(Title="Test")
    let myChart2 = Chart.Column(list2, Name = "Trommels") |> Chart.WithYAxis(Title="Test") |> Chart.WithLegend
                                                                        ( InsideArea = false, 
                                                                        Alignment = StringAlignment.Center, Docking = Docking.Top)
    let custom_Chart = Chart.Combine([myChart;myChart2])
    let myChartControl = new ChartControl(custom_Chart, Dock=DockStyle.Fill)
    myChartControl
    //List<float, float> locations of trommels
let third_Chart (list1 : List<'a*'a>) (list2 : List<'a*'a>) =
    let myChart = Chart.Column(list1, Name = "Thefts") |> Chart.WithYAxis(Title="Test")
    let myChart2 = Chart.Column(list2, Name = "Trommels") |> Chart.WithYAxis(Title="Test") |> Chart.WithLegend
                                                                        ( InsideArea = false, 
                                                                        Alignment = StringAlignment.Center, Docking = Docking.Top)
    let custom_Chart = Chart.Combine([myChart;myChart2])
    let myChartControl = new ChartControl(custom_Chart, Dock=DockStyle.Fill)
    myChartControl
    //list of thefts per unit of time List<string>
let fourth_Chart (list1 : List<'a*'a>) (list2 : List<'a*'a>) =
    let myChart = Chart.Column(list1, Name = "Thefts") |> Chart.WithYAxis(Title="Test")
    let myChart2 = Chart.Column(list2, Name = "Trommels") |> Chart.WithYAxis(Title="Test") |> Chart.WithLegend
                                                                        ( InsideArea = false, 
                                                                        Alignment = StringAlignment.Center, Docking = Docking.Top)
    let custom_Chart = Chart.Combine([myChart;myChart2])
    let myChartControl = new ChartControl(custom_Chart, Dock=DockStyle.Fill)
    myChartControl
    //list of area and corresponding thefts List<string, int>
let fifth_Chart (list1 : List<'a*'a>) (list2 : List<'a*'a>) =
    let myChart = Chart.Column(list1, Name = "Thefts") |> Chart.WithYAxis(Title="Test")
    let myChart2 = Chart.Column(list2, Name = "Trommels") |> Chart.WithYAxis(Title="Test") |> Chart.WithLegend
                                                                        ( InsideArea = false, 
                                                                        Alignment = StringAlignment.Center, Docking = Docking.Top)
    let custom_Chart = Chart.Combine([myChart;myChart2])
    let myChartControl = new ChartControl(custom_Chart, Dock=DockStyle.Fill)
    myChartControl
