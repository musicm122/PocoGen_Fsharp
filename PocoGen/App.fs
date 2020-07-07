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
          CurrentFormState = Idle
          Databases = []
          Languages = DefaultLanguages
          Tables = []
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
                          Output = "Successfully Connected" }
                | Fail ex ->
                    { model with
                          CurrentFormState = InvalidConnectionString
                          Output =
                              connectionTestResult.Message
                              + Environment.NewLine
                              + ex }
                | _ ->
                    { model with
                          CurrentFormState = InvalidConnectionString
                          Output = model.Output }

            let! dialog =
                Application.Current.MainPage.DisplayAlert("Connection Test Result", result.Output, "Ok")
                |> Async.AwaitTask
            return TestConnectionComplete result
        }
        |> Cmd.ofAsyncMsg

    let fetchDatabasesCmd (m: Model): Cmd<Msg> =
        async {
            let! dbs = DataAccess.getDatabaseNamesAsync m.ConnectionString
            let outMessage = sprintf "%i Databases found" dbs.Length
            let! output =
                match dbs.Length with
                | 0 ->
                    Application.Current.MainPage.DisplayAlert("Fetching Databases ", "No Databases found", "Ok")
                    |> Async.AwaitTask
                | _ ->
                    Application.Current.MainPage.DisplayAlert
                        ("Fetching Databases ", (sprintf "%i Databases found" dbs.Length), "Ok")
                    |> Async.AwaitTask

            return FetchDatabasesComplete { m with Databases = dbs }
        }
        |> Cmd.ofAsyncMsg

    let fetchTablesCmd (m: Model): Cmd<Msg> =
        async {
            let db =
                match m.SelectedDatabase with
                | Some db -> db
                | _ -> raise (System.ArgumentException("Missing Database"))

            let! tables = DataAccess.getTableNamesFromMSSqlServerQueryAsync db m.ConnectionString
            let outMessage = sprintf "%i Tables found" tables.Length
            let! output =
                match tables.Length with
                | 0 ->
                    Application.Current.MainPage.DisplayAlert("Fetching Tables ", "No Tables found", "Ok")
                    |> Async.AwaitTask
                | _ ->
                    Application.Current.MainPage.DisplayAlert
                        ("Fetching Tables ", (sprintf "%i Tables found" tables.Length), "Ok")
                    |> Async.AwaitTask

            return SetSelectedDatabaseComplete { m with Tables = tables }
        }
        |> Cmd.ofAsyncMsg

    let update (msg: Msg) (m: Model): Model * Cmd<Msg> =
        match msg with
        | UpdateConnectionStringValue conStringVal ->
            { m with
                  ConnectionString =
                      { Id = m.ConnectionString.Id
                        Name = m.ConnectionString.Name
                        Value = conStringVal }
                  CurrentFormState =
                      match Validation.IsConnectionStringInFormat m with
                      | false -> MissingConnStrValue
                      | _ -> Valid },
            Cmd.none
        | SetSelectedDatabase dbValue ->
            let selectedDb =
                m.Databases
                |> List.find (fun db -> db.Name = db.Name)
            { m with
                  CurrentFormState = FetchingData
                  SelectedDatabase = Some(selectedDb) },
            fetchTablesCmd m
        | SetSelectedLanguage l -> { m with SelectedLanguage = l }, Cmd.none
        | BrowseForOutputFolder f -> { m with OutputLocation = f }, Cmd.none
        | TestConnection -> { m with CurrentFormState = Testing }, runConnectionTestAsyncCmd m
        | TestConnectionComplete testResult -> { m with CurrentFormState = Idle }, Cmd.none
         | FetchDatabases ->
            { m with CurrentFormState = FetchingData }, fetchDatabasesCmd m 
        | FetchDatabasesComplete fetchDbResult ->
            { fetchDbResult with
                  CurrentFormState = Idle },
            Cmd.none
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
