module animateMap
    open System
    open System.Data
    open System.Data.Linq
    open System.IO
    open System.Drawing
    open System.Drawing.Imaging
    open FSharp.Data.TypeProviders
    open Microsoft.FSharp.Linq
    open CoroutineMonad
    open Visualization
    open System.Windows.Forms
    open CoroutineMonad

    type Vector2 = 
        {
        x:float
        y:float}

    let enableDobuleBuffer (control : Control) =
      let controlType = control.GetType()
      let pi = controlType.GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic ||| System.Reflection.BindingFlags.Instance)
      pi.SetValue(control, true, null)

    let createDot (image : Bitmap, pos : Point) =
      let red_dot = new PictureBox()
      red_dot.Width <- image.Width
      red_dot.Height <- image.Height
      red_dot.Image <- image
      red_dot.Location <- pos
      red_dot

    let createButton (text : string, pos : Point) =
      let red_dot = new Button()
      red_dot.Width <- 30
      red_dot.Height <- 50
      red_dot.Text <- text
      red_dot.Location <- pos
      red_dot

    let dummydate = [(51.9229316711426, 4.46130990982056);(51.9220695495605, 4.48883008956909);(51.9263496398926, 4.47619009017944);(51.9297218322754, 4.47138023376465);(51.9301986694336, 4.47891998291016)]

    let targetlat = 51.92293167114258
    let targetlon = 4.461309909820557
    let pixelY x = ((x - 51.984986) / (51.83827 - 51.984986)) * (759.0 - 0.0)
    let pixelX x = ((x - 4.297714) / (4.710388 - 4.297714)) * (1329.0 - 0.0)
    
    let seed = Random()
//    let get_random_pos () = new Point(seed.Next(10,500), seed.Next(10,500))
    let generate_dot (form : Form) reddot x =
        let reddot = createDot (reddot, new Point(int (fst x), int (snd x)))
        form.Controls.Add reddot
        reddot.BringToFront()

//    let rec generate_button (form : Form)  =
//        let button = createButton ("Click me", get_random_pos())
//        form.Controls.Add button
//        button.Click.Add(fun _ -> generate_button form)
//        button.BringToFront()
  
    let extract form reddot x =
        generate_dot form reddot (pixelX (snd x), pixelY (fst x))

    let drawTrommels form reddot =
        dummydate |> List.iter(extract form reddot)
    
    let animateMap () =
        let funcA () =
            let picture = 
                let picture = new PictureBox()
                let background = new Bitmap("rotterdam.jpg")
                picture.Image <- background
                picture.Size <- Size(1900,1000)
                picture
            let reddot = new Bitmap("Reddot.png")
            let form = new Form()
            form.Height <- 1000
            form.Width <- 1900
            form.Controls.Add (picture)
            form.BringToFront()
    
            //take 2 points, figure out where exactly they are -> google maps
            //
    //        let funcCor () =
    //            cor {
    //                do! wait_ 2.0
    //                generate_dot form reddot
    //                do! yield_
    //            }
    //        
    //        let coStep 

            let updateLoop =
                async {
    //                generate_dot form reddot
                    drawTrommels form reddot
                    while true do
                        do! Async.Sleep(16) }

            Async.StartImmediate updateLoop
            form.ShowDialog()
        funcA ()