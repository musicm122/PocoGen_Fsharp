namespace PocoGen.GTK

module Main =

    open PocoGen
    open System
    open Gtk
    open Xamarin.Forms
    open Xamarin.Forms.Platform.GTK


    [<EntryPoint>]
    let Main(args) =
        Application.Init()
        Forms.Init()
        let app = PocoGen.App()
        let win = new FormsWindow()
        win.LoadApplication(app)
        win.SetApplicationTitle("PocoGen")
        win.Show()
        Application.Run()
        0
