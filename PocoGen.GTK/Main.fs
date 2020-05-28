namespace PocoGen.GTK

open Gtk

module Main =

    open System;
    open Xamarin.Forms;
    open Xamarin.Forms.Platform.GTK;

    [<EntryPoint>]
    let Main(args) =
        Gtk.Application.Init();
        Forms.Init()
        let app = PocoGen.App()
        let win = new FormsWindow()
        win.LoadApplication(app)
        win.SetApplicationTitle("PocoGen")
        win.Show()
        Application.Run()
        0
