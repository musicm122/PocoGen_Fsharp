namespace PocoGen

open System
open PocoGen
open PocoGen.Components
open PocoGen.Models
open PocoGen.Messages
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms
open Fabulous.XamarinForms.LiveUpdate

module App =
    let defaultModel () =
        { OutputLocation = DefaultOutputPath
          ConnectionStrings = []
          CurrentFormState = Idle
          Databases = []
          Languages = DefaultLanguages
          Tables = []
          SelectedConnectionString = None
          SelectedDatabase = None
          SelectedLanguage = CSharp
          SelectedTables = []
          CodeGenPageState = PageState.Init
          ConnectionString = EmptyConnectionStringItem
          Output = String.Empty }


    let init () = defaultModel (), Cmd.none

    let runConnectionTestAsyncCmd (model: Model): Cmd<Msg> =
        async {
            let! connectionTestResult = DataAccess.testConnectionAsync model.ConnectionString.Value

            let result =
                match connectionTestResult.State with
                | Pass ->
                    { model with
                          CurrentFormState = Valid
                          Output =
                              model.Output
                              + Environment.NewLine
                              + "Successfully Connected"
                              + Environment.NewLine }
                | Fail ex ->
                    { model with
                          CurrentFormState = InvalidConnectionString
                          Output =
                              model.Output
                              + Environment.NewLine
                              + connectionTestResult.Message
                              + Environment.NewLine
                              + ex.Message }
                | _ ->
                    { model with
                          CurrentFormState = InvalidConnectionString
                          Output = model.Output }

            return TestConnectionComplete result
        }
        |> Cmd.ofAsyncMsg

    // todo: add update function
    let update (msg: Msg) (m: Model): Model * Cmd<Msg> =
        match msg with
        | SetSelectedDatabase db -> { m with SelectedDatabase = db }, Cmd.none
        | SetSelectedLanguage l -> { m with SelectedLanguage = l }, Cmd.none
        | SetSelectedConnection con ->
            { m with
                  SelectedConnectionString = Some(con) },
            Cmd.none
        | BrowseForOutputFolder f -> { m with OutputLocation = f }, Cmd.none
        | _ -> m, Cmd.none

    let view (model: Model) dispatch =

        let connectionSection =
            Components.connectionTestView model dispatch

        let codeGenSection = Components.codeGenView model dispatch

        View.ContentPage
            (padding = Thickness(5.0),
             title = "Code Gen",
             content = View.StackLayout(children = [ connectionSection; codeGenSection ]))


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
