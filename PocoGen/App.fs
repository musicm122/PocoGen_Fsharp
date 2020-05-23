namespace PocoGen

open System
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

module App =

    type Msg =
        | ConnectionPageMsg of Connection.Msg
        | CodeGenPageMsg of CodeGen.Msg
        //| NavigationPopped

    type Model =
        { ConnectionPageModel:Connection.Model
          CodeGenPageModel:CodeGen.Model
          WorkaroundNavPageBug: bool
          WorkaroundNavPageBugPendingCmd: Cmd<Msg> }

    type Pages =
        { ConnectionPage: ViewElement
          CodeGenPage: ViewElement }

    let getPages (allPages:Pages) =
        let connPage = allPages.ConnectionPage
        let codeGenPage = allPages.CodeGenPage
        [ connPage; codeGenPage ]

    let init () =
        { Model.CodeGenPageModel =  CodeGen.init();
          Model.ConnectionPageModel = Connection.init();
          Model.WorkaroundNavPageBug =false;
          Model.WorkaroundNavPageBugPendingCmd =Cmd.none;
        },Cmd.none

    let update (msg:Msg) (model:Model) =
        match msg with
        | ConnectionPageMsg msg->
            let m, cmd = Connection.update msg model.ConnectionPageModel
            {model with ConnectionPageModel = m }, cmd
        | CodeGenPageMsg msg ->
            let m, cmd = CodeGen.update msg model.CodeGenPageModel
            {model with CodeGenPageModel = m }, cmd

    let view (model:Model) dispatch =
        let connPage = Connection.view model.ConnectionPageModel (ConnectionPageMsg >> dispatch)
        let codeGenPage = CodeGen.view model.CodeGenPageModel (CodeGenPageMsg >> dispatch)

        let allPages =
            { ConnectionPage = connPage
              CodeGenPage = codeGenPage
            }

        View.TabbedPage(title="Poco Gen", children = [connPage; codeGenPage])


type App() as app =
    inherit Application()

    do Device.SetFlags([
        "Shell_Experimental"; "CollectionView_Experimental"; "Visual_Experimental"; 
        "IndicatorView_Experimental"; "SwipeView_Experimental"; "MediaElement_Experimental"
        "AppTheme_Experimental"; "RadioButton_Experimental"; "Expander_Experimental"
    ])
    
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
//do runner.EnableLiveUpdate()
#endif

// Uncomment this code to save the application state to app.Properties using Newtonsoft.Json
// See https://fsprojects.github.io/Fabulous/Fabulous.XamarinForms/models.html#saving-application-state for further  instructions.
#if APPSAVE
let modelId = "model"
override __.OnSleep() =

    let json = Newtonsoft.Json.JsonConvert.SerializeObject(runner.CurrentModel)
    Console.WriteLine("OnSleep: saving model into app.Properties, json = {0}", json)

    app.Properties.[modelId] <- json

override __.OnResume() =
    Console.WriteLine "OnResume: checking for model in app.Properties"
    try
        match app.Properties.TryGetValue modelId with
        | true, (:? string as json) ->

            Console.WriteLine("OnResume: restoring model from app.Properties, json = {0}", json)
            let model = Newtonsoft.Json.JsonConvert.DeserializeObject<App.Model>(json)

            Console.WriteLine("OnResume: restoring model from app.Properties, model = {0}", (sprintf "%0A" model))
            runner.SetCurrentModel (model, Cmd.none)

        | _ -> ()
    with ex ->
        App.program.onError("Error while restoring model found in app.Properties", ex)

override this.OnStart() =
    Console.WriteLine "OnStart: using same logic as OnResume()"
    this.OnResume()
#endif