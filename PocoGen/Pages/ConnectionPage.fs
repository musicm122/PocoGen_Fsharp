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
    | UpdateOutput of string
    | ClearOutput

let runConnectionTestAsyncCmd (model: Model): Cmd<Msg> =
    async {
        let! connectionTestResult = DataAccess.testConnectionAsync model.ConnectionString.Value

        let result =
            match connectionTestResult.State with
            | Pass ->
                { model with
                      CurrentFormState = Valid
                      Output = connectionTestResult.Message }
            | Fail ex ->
                { model with
                      CurrentFormState = InvalidConnectionString
                      Output = connectionTestResult.Message + ex.Message }
            | _ ->
                { model with
                      CurrentFormState = InvalidConnectionString
                      Output = String.Empty }

        return TestConnectionComplete result
    }
    |> Cmd.ofAsyncMsg

let hasRequiredSaveFields (conStr: ConnectionStringItem): bool =
    conStr.Name.IsNullOrWhiteSpace() && conStr.Value.IsNullOrWhiteSpace()

let saveConnectionAsyncCmd (model: Model): Cmd<Msg> =
    async {
        let! connectionTestResult = DataAccess.testConnectionAsync model.ConnectionString.Value

        return match connectionTestResult.State with
               | Pass ->
                   let result = Store.addConnectionString model.ConnectionString
                   match result with
                   | Success -> SaveConnectionStringComplete { model with Output = "Saved Successfully" }
                   | Error err -> SaveConnectionStringComplete { model with Output = err }
               | Fail ex ->
                   SaveConnectionStringComplete
                       { model with Output = "Attempt to save failed with " + connectionTestResult.Message + ex.Message }
               | _ -> SaveConnectionStringComplete { model with Output = String.Empty }
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
                  | _ -> Valid }, Cmd.none

    | UpdateConnectionStringName conStringName ->
        { m with
              ConnectionString =
                  { Id = m.ConnectionString.Id
                    Name = conStringName
                    Value = m.ConnectionString.Value }
              CurrentFormState =
                  match hasRequiredSaveFields m.ConnectionString with
                  | false -> MissingConnStrValue
                  | _ -> Valid }, Cmd.none

    | UpdateOutput output -> { m with Output = output }, Cmd.none
    | ClearOutput -> { m with Output = String.Empty }, Cmd.none
    | TestConnection -> { m with CurrentFormState = Testing }, runConnectionTestAsyncCmd m
    | TestConnectionComplete testResult ->
        { m with
              Output = testResult.Output + "\r\n"
              CurrentFormState = Idle }, Cmd.none
    | SaveConnectionString (_) -> m, saveConnectionAsyncCmd m
    | _ -> m, Cmd.none

let view (model: Model) dispatch: ViewElement =
    let isLoading = (model.CurrentFormState = Testing)
    let updateConnectionStringValue = UpdateConnectionStringValue >> dispatch
    let updateConnectionStringName = UpdateConnectionStringName >> dispatch
    let testConnection = (fun () -> dispatch (TestConnection))
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

    let buttonStack =
        View.StackLayout
            (orientation = StackOrientation.Horizontal, verticalOptions = LayoutOptions.Center,
             horizontalOptions = LayoutOptions.Fill, children = [ testButton; saveButton ])

    View.ContentPage
        (padding = Thickness(5.0), title = "Connection Test",
         content =
             Components.verticalStack
                 ([ View.ActivityIndicator(isVisible = isLoading, isRunning = isLoading)
                    Components.formLabel "Connection String Label"
                    Components.formEntry model.ConnectionString.Name updateConnectionStringName
                    Components.formLabel "Connection String Value"
                    Components.formMultiLineEditor model.ConnectionString.Value updateConnectionStringValue
                    buttonStack
                    Components.formLabel "Output"
                    Components.formLabel (sprintf "%s" model.Output) ]))
