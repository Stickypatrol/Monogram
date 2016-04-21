module Interface
    open System
    open System.Data
    open System.Data.Linq
    open FSharp.Data.TypeProviders
    open Microsoft.FSharp.Linq
    open FSharp.Charting
    open FSharp.Charting.ChartTypes
    open System.Drawing
    open System.Windows.Forms
    open CoroutineMonad
    open Communication

    let lbl = new Label(Text="Project 3 - Data Visualization", Top = 10, Left = 300, Width = 200)

    let button1 = new Button(Text="button 1", Top = 70, Left = 70)
    let button2 = new Button(Text="button 2", Top = 100, Left = 70)
    let button3 = new Button(Text="button 3", Top = 130, Left = 70)
    let button4 = new Button(Text="button 4", Top = 160, Left = 70)
    let button5 = new Button(Text="button 5", Top = 190, Left = 70)

    let return_button = new Button(Text="<-", Top = 10, Left = 10)

    let form = new Form(Visible = true, TopMost = true, Width = 700, Height = 500)
    let socket, ip = getsocketandip()
    let SendAndReceiveAll() =
      cor{
        do! ConnectFunction()
        let! result1 = SendAndReceiveQ1()
        let! result2 = SendAndReceiveQ2()
        let! result3 = SendAndReceiveQ3()
        let! result4 = SendAndReceiveQ4()
        let! result5 = SendAndReceiveQ5()
        return result1, result2, result3, result4, result5
      }
    let r1,r2,r3,r4,r5 = Run (SendAndReceiveAll()) (socket, ip)
    let myChartControl1 = Visualization.first_Chart   (List.fold2 (fun s safety area -> (safety, area)::s) [] (fst r1) (snd r1))
    let myChartControl2 = Visualization.second_Chart  r2
    let myChartControl3 = Visualization.third_Chart   (List.fold2 (fun s safety area -> (safety, area)::s) [] (fst r3) (snd r3))
    let myChartControl4 = Visualization.fourth_Chart  (List.fold2 (fun s thefts year -> (thefts, year)::s) [] (snd r4) (fst r4))
    let myChartControl5 = Visualization.fifth_Chart   (List.fold2 (fun s area thefts -> (thefts, area)::s) [] (fst r5) (snd r5))
    

    form.Controls.Add lbl
    form.Controls.Add button1
    form.Controls.Add button2
    form.Controls.Add button3
    form.Controls.Add button4
    form.Controls.Add button5
    form.Controls.Add return_button
    //form.Controls.Add pb

    

    let firstForm () = //here the result
        //within this call the coroutine methods to get the data properly
        myChartControl1.Show()
        button1.Hide()
        button2.Hide()
        button3.Hide()
        button4.Hide()
        button5.Hide()
        return_button.Show()

    let secondForm () =
        //myChartControl.Show()
        myChartControl2.Show()
        button1.Hide()
        button2.Hide()
        button3.Hide()
        button4.Hide()
        button5.Hide()
        return_button.Show()

    let thirdForm () =
        myChartControl3.Show()
        button1.Hide()
        button2.Hide()
        button3.Hide()
        button4.Hide()
        button5.Hide()
        //pb.Show()
        return_button.Show()

    let fourthForm () =
        myChartControl4.Show()
        button1.Hide()
        button2.Hide()
        button3.Hide()
        button4.Hide()
        button5.Hide()
        return_button.Show()

    let fifthForm () =
        myChartControl5.Show()
        button1.Hide()
        button2.Hide()
        button3.Hide()
        button4.Hide()
        button5.Hide()
        return_button.Show()

    let returnFunc () =
        myChartControl1.Hide()
        myChartControl2.Hide()
        myChartControl3.Hide()
        myChartControl4.Hide()
        myChartControl5.Hide()
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
    
    form.Controls.Add(myChartControl1)
    form.Controls.Add(myChartControl2)
    form.Controls.Add(myChartControl3)
    form.Controls.Add(myChartControl4)
    form.Controls.Add(myChartControl5)

    myChartControl1.Hide()
    myChartControl2.Hide()
    myChartControl3.Hide()
    myChartControl4.Hide()
    myChartControl5.Hide()
    return_button.Hide()
    //pb.Hide()
    
    

