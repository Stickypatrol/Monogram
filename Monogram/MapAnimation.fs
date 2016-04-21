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

    let blur (image:Bitmap) blurSize =
        let blurred = new Bitmap(image.Width, image.Height)
        let graphics = Graphics.FromImage(blurred)
        let rectangle = Rectangle(0, 0, image.Width, image.Height)
        graphics.DrawImage(image, rectangle, rectangle, GraphicsUnit.Pixel);
        blurred

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

    let seed = Random()
    let get_random_pos () = new Point(seed.Next(10,500), seed.Next(10,500))
    let generate_dot (form : Form) reddot =
        let reddot = createDot (reddot, get_random_pos())
        form.Controls.Add reddot
        reddot.BringToFront()

    let rec generate_button (form : Form)  =
        let button = createButton ("Click me", get_random_pos())
        form.Controls.Add button
        button.Click.Add(fun _ -> generate_button form)
        button.BringToFront()
  
    
    let animateMap () =
        let funcA () =
            let picture = 
                let picture = new PictureBox()
                let background = new Bitmap("rotterdam.jpg")
                picture.Image <- background
                picture.Size <- Size(1000,1900)
                picture
            let reddot = new Bitmap("Reddot.png")
            let form = new Form()
            form.Height <- 1000
            form.Width <- 1900
            form.Controls.Add (picture)
    
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
                    generate_button form
                    while true do
                        generate_dot form reddot
                        do! Async.Sleep(16) }

            Async.StartImmediate updateLoop
            Application.Run form
        funcA ()