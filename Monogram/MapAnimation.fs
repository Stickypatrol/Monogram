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
      red_dot.BackColor <- Color.Transparent
      red_dot

    let dummydate = [(51.9229316711426, 4.46130990982056);(51.9220695495605, 4.48883008956909);(51.9263496398926, 4.47619009017944);(51.9297218322754, 4.47138023376465);(51.9301986694336, 4.47891998291016)]
    let pixelY x = ((x - 51.984986) / (51.83827 - 51.984986)) * (759.0 - 0.0)
    let pixelX x = ((x - 4.297714) / (4.710388 - 4.297714)) * (1329.0 - 0.0)
    let pixelY2 x = ((x - 51.973189) / (51.859710 - 51.973189)) * (1080.0 - 0.0)
    let pixelX2 x = ((x - 4.324460) / (4.653627 - 4.324460)) * (1920.0 - 0.0)
    let generate_dot (form : Form) reddot x =
        let reddot = createDot (reddot, new Point(int (fst x), int (snd x)))
        form.Controls.Add reddot
        reddot.BackColor <- Color.Transparent
        reddot.BringToFront()
        

    let extract form reddot x =
        generate_dot form reddot (pixelX2 (snd x), pixelY2 (fst x))

    let animateMap () =
        let funcA () =
            let picture = 
                let picture = new PictureBox()
                let background = new Bitmap("rotterdam.png")
                picture.Image <- background
                picture.Width <- 1920
                picture.Height <- 1080
                picture
            let reddot = new Bitmap("red_dot.png")
            let form = new Form()
            let button = new Button()
            button.Text <- "Exit"
            button.Location <- new Point(900, 10)
            button.Click.Add(fun _ -> form.Hide())
            form.Height <- 1080
            form.Width <- 1920
            form.Controls.Add picture
            form.Controls.Add button
            button.BringToFront()
    
            let updateLoop =
                async {
                    while true do
                        for data in dummydate do
                            extract form reddot data
                            do! Async.Sleep(1000)
                        do! Async.Sleep(16) }

            Async.StartImmediate updateLoop
            form.ShowDialog()
        funcA ()