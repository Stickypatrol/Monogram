﻿module Interface
    open System
    open System.Data
    open System.Data.Linq
    open FSharp.Data.TypeProviders
    open Microsoft.FSharp.Linq
    open FSharp.Charting
    open FSharp.Charting.ChartTypes
    open System.Drawing
    open System.Windows.Forms

    let lbl = new Label(Text="Project 3 - Data Visualization", Top = 10, Left = 300, Width = 200)

    let button1 = new Button(Text="button 1", Top = 70, Left = 70)
    let button2 = new Button(Text="button 2", Top = 100, Left = 70)
    let button3 = new Button(Text="button 3", Top = 130, Left = 70)
    let button4 = new Button(Text="button 4", Top = 160, Left = 70)
    let button5 = new Button(Text="button 5", Top = 190, Left = 70)

    let return_button = new Button(Text="<-", Top = 10, Left = 10)

    let form = new Form(Visible = true, TopMost = true, Width = 700, Height = 500)
    let myChartControl = Visualization.first_Chart [1,100;2,200;3,300;4,400;5,500] [1,10;2,20;3,30;4,40;5,50]

    form.Controls.Add lbl
    form.Controls.Add button1
    form.Controls.Add button2
    form.Controls.Add button3
    form.Controls.Add button4
    form.Controls.Add button5
    form.Controls.Add return_button
    //form.Controls.Add pb

    let firstForm () =
        myChartControl.Show()
        button1.Hide()
        button2.Hide()
        button3.Hide()
        button4.Hide()
        button5.Hide()
        return_button.Show()

    let secondForm () =
        //myChartControl.Show()
        button1.Hide()
        button2.Hide()
        button3.Hide()
        button4.Hide()
        button5.Hide()
        return_button.Show()

    let thirdForm () =
        //myChartControl.Show()
        button1.Hide()
        button2.Hide()
        button3.Hide()
        button4.Hide()
        button5.Hide()
        //pb.Show()
        return_button.Show()

    let fourthForm () =
        //myChartControl.Show()
        button1.Hide()
        button2.Hide()
        button3.Hide()
        button4.Hide()
        button5.Hide()
        return_button.Show()

    let fifthForm () =
        //myChartControl.Show()
        button1.Hide()
        button2.Hide()
        button3.Hide()
        button4.Hide()
        button5.Hide()
        return_button.Show()

    let returnFunc () =
        myChartControl.Hide()
        button1.Show()
        button2.Show()
        button3.Show()
        button4.Show()
        button5.Show()
        return_button.Hide()
        

    button1.Click.Add(fun _ -> firstForm())
    button2.Click.Add(fun _ -> secondForm())
    button3.Click.Add(fun _ -> thirdForm())
    button4.Click.Add(fun _ -> fourthForm())
    button5.Click.Add(fun _ -> fifthForm())
    return_button.Click.Add(fun _ -> returnFunc())
    
    form.Controls.Add(myChartControl)

    myChartControl.Hide()
    return_button.Hide()
    //pb.Hide()
    
    
