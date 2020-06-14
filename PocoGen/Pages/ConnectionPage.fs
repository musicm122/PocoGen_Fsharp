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

type Model =
    { ConnectionString : ConnectionStringItem
      CurrentFormState : FormState
      Output : string }

let init() : Model =
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
    | SaveConnectionString of ConnectionStringItem
    | UpdateOutput of string

let runConnectionTestAsyncCmd (model : Model) : Cmd<Msg> =
    async {
        let! connectionTestResult = DataAccess.testConnectionAsync model.ConnectionString.Value

        let result =
            match connectionTestResult.State with
            | Pass -> { model with Output = connectionTestResult.Message }
            | Fail ex -> { model with Output = connectionTestResult.Message }
            | _ -> { model with Output = String.Empty }

        return TestConnectionComplete result
    }
    |> Cmd.ofAsyncMsg

let hasRequredSaveFields (conStr : ConnectionStringItem) : bool =
    conStr.Name.IsNullOrWhiteSpace() && conStr.Value.IsNullOrWhiteSpace()

let saveConnection (model : Model) : Model =
    let connectionTestResult = DataAccess.testConnection model.ConnectionString.Value

    match connectionTestResult.State with
    | Pass ->
        let result = Store.addConnectionString model.ConnectionString

        match result with
        | Success -> { model with Output = "Saved Successfully" }
        | Error err -> { model with Output = err }
    | Fail ex -> { model with Output = "Attempt to save failed with " + connectionTestResult.Message }
    | _ -> { model with Output = String.Empty }

let update (msg : Msg) (m : Model) : Model * Cmd<Msg> =
    match msg with
    | UpdateConnectionStringValue conStringVal ->
        { m with
              ConnectionString =
                  { Id = m.ConnectionString.Id
                    Name = m.ConnectionString.Name
                    Value = conStringVal }
              CurrentFormState =
                  match hasRequredSaveFields m.ConnectionString with
                  | false -> MissingConnStrValue
                  | _ -> Valid }, Cmd.none

    | UpdateConnectionStringName conStringName ->
        { m with
              ConnectionString =
                  { Id = m.ConnectionString.Id
                    Name = conStringName
                    Value = m.ConnectionString.Value }
              CurrentFormState =
                  match hasRequredSaveFields m.ConnectionString with
                  | false -> MissingConnStrValue
                  | _ -> Valid }, Cmd.none

    | UpdateOutput output -> { m with Output = output }, Cmd.none
    | TestConnection -> m, Cmd.none
    | TestConnectionComplete testResult -> { m with Output = testResult.Output + "\r\n" }, Cmd.none
    | SaveConnectionString(_) -> saveConnection m, Cmd.none

let view (model : Model) dispatch : ViewElement =
    let updateConnectionStringValue = UpdateConnectionStringValue >> dispatch
    let updateConnectionStringName = UpdateConnectionStringName >> dispatch

    let testConnection = (fun () -> dispatch (TestConnection))

    let saveConnectionString = (fun () -> dispatch (SaveConnectionString model.ConnectionString))

    let testButton =
        (match model.CurrentFormState with
         | Valid -> Components.formButton "Test" testConnection true
         | _ -> Components.formButton "Test" testConnection false)

    let saveButton =
        (match model.CurrentFormState with
         | Valid -> Components.formButton "Save" saveConnectionString true
         | _ -> Components.formButton "Save" saveConnectionString false)

    let buttonStack =
        View.StackLayout
            (orientation = StackOrientation.Horizontal, verticalOptions = LayoutOptions.Center,
             horizontalOptions = LayoutOptions.Fill, children = [ testButton; saveButton ])

    View.ContentPage
        (padding = Thickness(5.0), title = "Connection Test",
         content = Components.verticalStack
                       ([ Components.formLabel "Connection String Label"
                          Components.formEntry model.ConnectionString.Name updateConnectionStringName
                          Components.formLabel "Connection String Value"
                          Components.formMultiLineEditor model.ConnectionString.Value updateConnectionStringValue
                          buttonStack
                          Components.formLabel "Output"
                          Components.formLabel (sprintf "%s" model.Output) ]))
