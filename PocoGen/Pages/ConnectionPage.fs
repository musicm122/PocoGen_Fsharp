module PocoGen.Page.ConnectionPage

open System
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms
open PocoGen
open PocoGen.DomainModels
open PocoGen.Components
open Common

type FormState =
    | Valid
    | MissingConnStrValue
    | Testing
    | Idle
    | InvalidConnectionString

type Model =
    { ConnectionString: ConnectionStringItem
      CurrentFormState: FormState
      Output: string }

let init (): Model =
    { ConnectionString =
          { ConnectionStringItem.Id = 0
            ConnectionStringItem.Value = String.Empty
            ConnectionStringItem.Name = String.Empty }
      CurrentFormState = MissingConnStrValue
      Output = String.Empty }

type Msg =
    | UpdateConnectionStringValue of string
    | UpdateConnectionStringName of string
    | TestConnection
    | TestConnectionComplete of Model
    | SaveConnectionString
    | SaveConnectionStringComplete of Model
    | ClearOutput

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
                          + Environment.NewLine
                          }
            | Fail ex ->
                { model with
                      CurrentFormState = InvalidConnectionString
                      Output =
                          model.Output
                          + Environment.NewLine
                          + connectionTestResult.Message
                          + Environment.NewLine
                          + ex.Message 
                          }
            | _ ->
                { model with
                      CurrentFormState = InvalidConnectionString
                      Output = model.Output }

        return TestConnectionComplete result
    }
    |> Cmd.ofAsyncMsg

let hasRequiredSaveFields (conStr: ConnectionStringItem): bool =
    conStr.Name.IsNullOrWhiteSpace()
    && conStr.Value.IsNullOrWhiteSpace()

let saveConnectionAsyncCmd (model: Model): Cmd<Msg> =
    async {
        let! connectionTestResult = DataAccess.testConnectionAsync model.ConnectionString.Value

        return match connectionTestResult.State with
               | Pass ->
                   let result =
                       Store.addConnectionString model.ConnectionString

                   match result with
                   | Success ->
                       SaveConnectionStringComplete
                           { model with
                                 Output =
                                     model.Output 
                                     + Environment.NewLine
                                     + "Saved Successfully" }
                   | Error err ->
                       SaveConnectionStringComplete
                           { model with
                                 Output = model.Output + Environment.NewLine + err }
               | Fail ex ->
                   SaveConnectionStringComplete
                       { model with
                             Output =
                                 model.Output
                                 + Environment.NewLine
                                 + "Attempt to save failed with "
                                 + connectionTestResult.Message
                                 + ex.Message }
               | _ -> SaveConnectionStringComplete { model with Output = model.Output }
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
                  match hasRequiredSaveFields m.ConnectionString with
                  | false -> MissingConnStrValue
                  | _ -> Valid },
        Cmd.none

    | UpdateConnectionStringName conStringName ->
        { m with
              ConnectionString =
                  { Id = m.ConnectionString.Id
                    Name = conStringName
                    Value = m.ConnectionString.Value }
              CurrentFormState =
                  match hasRequiredSaveFields m.ConnectionString with
                  | false -> MissingConnStrValue
                  | _ -> Valid },
        Cmd.none

    | ClearOutput -> { m with Output = String.Empty }, Cmd.none
    | TestConnection -> { m with CurrentFormState = Testing }, runConnectionTestAsyncCmd m
    | TestConnectionComplete testResult ->
        { m with CurrentFormState = Idle; Output = m.Output + testResult.Output + Environment.NewLine},
        Cmd.none
    | SaveConnectionString (_) -> m, saveConnectionAsyncCmd m
    | SaveConnectionStringComplete m -> { m with CurrentFormState = Idle }, Cmd.none

let view (model: Model) dispatch: ViewElement =
    let isLoading = (model.CurrentFormState = Testing)
    let updateConnectionStringValue = UpdateConnectionStringValue >> dispatch
    let updateConnectionStringName = UpdateConnectionStringName >> dispatch
    let testConnection = (fun () -> dispatch (TestConnection))
    let clearOutput = (fun () -> dispatch (ClearOutput))

    let saveConnectionString =
        (fun () -> dispatch (SaveConnectionString))

    let testButton =
        (match model.CurrentFormState with
         | Valid -> Components.formButton "Test" testConnection (not isLoading)
         | _ -> Components.formButton "Test" testConnection false)

    let saveButton =
        (match model.CurrentFormState with
         | Valid -> Components.formButton "Save" saveConnectionString (not isLoading)
         | _ -> Components.formButton "Save" saveConnectionString false)

    let clearOutputButton =
        (match model.CurrentFormState with
         | Valid -> Components.formButton "Clear Output" clearOutput (not isLoading)
         | _ -> Components.formButton "Clear Output" clearOutput false)

    let buttonStack =
        View.StackLayout
            (orientation = StackOrientation.Horizontal,
             verticalOptions = LayoutOptions.Center,
             horizontalOptions = LayoutOptions.Fill,
             children = [ testButton; saveButton ])

    View.ContentPage
        (padding = Thickness(5.0),
         title = "Connection Test",
         content =
             Components.verticalStack
                 ([ View.ActivityIndicator(isVisible = isLoading, isRunning = isLoading)
                    Components.formLabel "Connection String Label"
                    Components.formEntry model.ConnectionString.Name updateConnectionStringName
                    Components.formLabel "Connection String Value"
                    Components.formMultiLineEditor model.ConnectionString.Value updateConnectionStringValue
                    buttonStack
                    Components.formLabel "Output"
                    Components.formOutput (sprintf "%s" model.Output)
                    clearOutputButton ]))
