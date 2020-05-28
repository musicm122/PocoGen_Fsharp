namespace PocoGen

open System
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms
open Microsoft.Data.Sqlite
open FSharp.Data.Dapper
open Fabulous.XamarinForms.LiveUpdate


module App =

    type Msg =
        | ConnectionPageMsg of ConnectionPage.Msg
        | CodeGenPageMsg of CodeGenPage.Msg
    //| NavigationPopped

    type Model =
        { ConnectionPageModel : ConnectionPage.Model
          CodeGenPageModel : CodeGenPage.Model
          WorkaroundNavPageBug : bool
          WorkaroundNavPageBugPendingCmd : Cmd<Msg> }

    type Pages =
        { ConnectionPage : ViewElement
          CodeGenPage : ViewElement }

    let getPages (allPages : Pages) =
        let connPage = allPages.ConnectionPage
        let codeGenPage = allPages.CodeGenPage
        [ connPage; codeGenPage ]

    let initModels() =
        { Model.CodeGenPageModel = CodeGenPage.initModel()
          Model.ConnectionPageModel = ConnectionPage.init()
          Model.WorkaroundNavPageBug = false
          Model.WorkaroundNavPageBugPendingCmd = Cmd.none }

    let init() =
        let model = initModels()
        let cmd = Cmd.none
        model, cmd

    let update (msg : Msg) (model : Model) =
        match msg with
        | ConnectionPageMsg msg ->
            let m, cmd = ConnectionPage.update msg model.ConnectionPageModel
            { model with ConnectionPageModel = m }, cmd
        | CodeGenPageMsg msg ->
            let m, cmd = CodeGenPage.update msg model.CodeGenPageModel
            { model with CodeGenPageModel = m }, cmd

    let view (model : Model) dispatch =
        let connPage = ConnectionPage.view model.ConnectionPageModel (ConnectionPageMsg >> dispatch)
        let codeGenPage = CodeGenPage.view model.CodeGenPageModel (CodeGenPageMsg >> dispatch)

        let allPages =
            { ConnectionPage = connPage
              CodeGenPage = codeGenPage }

        View.TabbedPage(title = "Poco Gen", children = [ connPage; codeGenPage ])


type App() as app =
    inherit Application()
    do
        Device.SetFlags
            ([ "Shell_Experimental"
               "CollectionView_Experimental"
               "Visual_Experimental"
               "IndicatorView_Experimental"
               "SwipeView_Experimental"
               "MediaElement_Experimental"
               "AppTheme_Experimental"
               "RadioButton_Experimental"
               "Expander_Experimental" ])

    let init = App.init
    let update = App.update
    let view = App.view

    let runner =
        Program.mkProgram init update view
        |> Program.withConsoleTrace
        |> XamarinFormsProgram.run app


#if DEBUG
    // Uncomment this line to enable live update in debug mode.
    // See https://fsprojects.github.io/Fabulous/Fabulous.XamarinForms/tools.html#live-update for further  instructions.
    //
    do runner.EnableLiveUpdate()
#endif

// Uncomment this code to save the application state to app.Properties using Newtonsoft.Json
// See https://fsprojects.github.io/Fabulous/Fabulous.XamarinForms/models.html#saving-application-state for further  instructions.
#if APPSAVE



























#endif
