module Visualization
open System
open FSharp.Charting
open FSharp.Charting.ChartTypes
open System.Drawing
open System.Windows.Forms
open CoroutineMonad

[<STAThread>] //amount of thefts per area List<float, string>
let first_Chart (list1 : List<float*string>) =
    let myChart = Chart.Column(list1, Name = "Relative Safety Index of areas") |> Chart.WithYAxis(Title="Relative Safety Index") |> Chart.WithXAxis(Title="Area") |> Chart.WithLegend
                                                                        ( InsideArea = false, 
                                                                        Alignment = StringAlignment.Center, Docking = Docking.Top)
    let myChartControl = new ChartControl(myChart, Dock=DockStyle.Fill)
    myChartControl
    //dates of thefts and trommels respectively List<string, string>
let second_Chart (list1 : List<string*int>) =
    let months = [for x in 1..36 do yield x]
    let lista, listb = list1 |> List.fold (fun (sa, sb) (xa, xb) -> xa::sa, xb::sb) ([],[])
    let rec foldy s x y r =
      match x, y with
      | x::xs, y::ys  -> foldy (s@[x,y]) xs ys r
      | x::xs, []     -> foldy (s@[x,r]) xs [] r
      | [], y::ys     -> failwith "shouldnt happen, something wrong with months list"
      | [], []        -> s
    let lista' = foldy [] months lista ""
    let listb' = foldy [] months listb 0
    let myChart1 = Chart.Column(lista', Name = "Thefts") |> Chart.WithYAxis(Title="Amount of Thefts") |> Chart.WithXAxis(Title="Month")
                                                                      |> Chart.WithLegend
                                                                          ( InsideArea = false, 
                                                                          Alignment = StringAlignment.Center, Docking = Docking.Top)
    let myChart2 = Chart.Column(lista', Name = "Trommels") |> Chart.WithYAxis(Title="Amount of Trommels") |> Chart.WithXAxis(Title="Month")
                                                                      |> Chart.WithLegend
                                                                          ( InsideArea = false, 
                                                                          Alignment = StringAlignment.Center, Docking = Docking.Top)
    let custom_Chart = Chart.Combine([myChart1;myChart2])
    let myChartControl = new ChartControl(custom_Chart, Dock=DockStyle.Fill)
    myChartControl
    //List<float, float> locations of trommels
let third_Chart (list1 : List<float*float>) = //hier even naar kijken, dit moet een map worden
    let myChart = Chart.Column(list1, Name = "Thefts") |> Chart.WithYAxis(Title="Test") |> Chart.WithLegend
                                                                        ( InsideArea = false, //DIT MOET HELEMAAL VERANDERD WORDEN NAAR EEN MAP DUS MUSTAFA GA JE GANG
                                                                        Alignment = StringAlignment.Center, Docking = Docking.Top)
    let custom_Chart = Chart.Combine([myChart;])
    let myChartControl = new ChartControl(custom_Chart, Dock=DockStyle.Fill)
    myChartControl
    //list of thefts per year List<int, string>
let fourth_Chart (list1 : List<int*string>) =
    let myChart = Chart.Column(list1, Name = "Thefts") |> Chart.WithYAxis(Title="Amount") |> Chart.WithXAxis(Title="Year") |> Chart.WithLegend
                                                                        ( InsideArea = false, 
                                                                        Alignment = StringAlignment.Center, Docking = Docking.Top)
    let custom_Chart = Chart.Combine([myChart])
    let myChartControl = new ChartControl(custom_Chart, Dock=DockStyle.Fill)
    myChartControl
    //list of area and corresponding thefts List<string, int>
let fifth_Chart (list1 : List<string*int>) =
    let myChart = Chart.Column(list1, Name = "Total thefts by area") |> Chart.WithYAxis(Title="Total thefts") |> Chart.WithYAxis(Title="Area") |> Chart.WithLegend
                                                                        ( InsideArea = false, 
                                                                        Alignment = StringAlignment.Center, Docking = Docking.Top)
    let custom_Chart = Chart.Combine([myChart])
    let myChartControl = new ChartControl(custom_Chart, Dock=DockStyle.Fill)
    myChartControl
