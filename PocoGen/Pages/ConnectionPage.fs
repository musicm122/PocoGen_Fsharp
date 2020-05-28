module ConnectionPage

open System
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms
open Models
open PocoGen
open Common

type FormState =
    | Valid
    | MissingConnStrValue

type Model =
    { ConnectionString: ConnectionStringItem
      CurrentFormState: FormState
      Output: string }

let init () =
    { ConnectionString =
          { ConnectionStringItem.Id = 0
            ConnectionStringItem.Value = String.Empty
            ConnectionStringItem.Name = String.Empty }
      CurrentFormState = MissingConnStrValue
      Output = String.Empty }

type Msg =
    | UpdateConnectionStringValue of string
    | UpdateConnectionStringName of string
    | TestConnection of ConnectionStringItem
    | SaveConnectionString of ConnectionStringItem
    | UpdateOutput of string

let runConnectionTest (model: Model): Model =
    let connectionTestResult =
        DataAccess.testConnection model.ConnectionString.Value

    let testResult =
        match connectionTestResult.State with
        | Pass ->
            { model with
                  Output = connectionTestResult.Message }
        | Fail ex ->
            { model with
                  Output = connectionTestResult.Message }
        | _ -> { model with Output = String.Empty }

    testResult

let hasRequredSaveFields (conStr: ConnectionStringItem): bool =
    conStr.Name.IsNullOrWhiteSpace()
    && conStr.Value.IsNullOrWhiteSpace()

let saveConnection (model: Model): Model =
    let result = Store.addConnectionString model.ConnectionString
    match result with
    | Success ->
        { model with Output = "Saved Successfully" }
    | Error err->
        { model with Output = err }

let update (msg: Msg) (m: Model) =
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
                  | _ -> Valid },
        Cmd.none

    | UpdateConnectionStringName conStringName ->
        { m with
              ConnectionString =
                  { Id = m.ConnectionString.Id
                    Name = conStringName
                    Value = m.ConnectionString.Value }
              CurrentFormState =
                  match hasRequredSaveFields m.ConnectionString with
                  | false -> MissingConnStrValue
                  | _ -> Valid },
        Cmd.none

    | UpdateOutput output -> { m with Output = output }, Cmd.none
    | TestConnection con ->
        let testResult = runConnectionTest m
        { m with
              Output = testResult.Output + "\r\n" },
        Cmd.none
    | SaveConnectionString (_) -> saveConnection m, Cmd.none

let view (model: Model) dispatch =
    let updateConnectionStringValue = UpdateConnectionStringValue >> dispatch
    let updateConnectionStringName = UpdateConnectionStringName >> dispatch

    let testConnection =
        (fun () -> dispatch (TestConnection model.ConnectionString))

    let saveConnectionString =
        (fun () -> dispatch (SaveConnectionString model.ConnectionString))

    let testButton =
        (match model.CurrentFormState with
         | Valid -> Components.formButton "Test" testConnection true
         | _ -> Components.formButton "Test" testConnection false)

    let saveButton =
        (match model.CurrentFormState with
         | Valid -> Components.formButton "Save" saveConnectionString true
         | _ -> Components.formButton "Save" saveConnectionString false)

    View.ContentPage
        (padding = Thickness(5.0),
         title = "Connection Test",
         content =
             Components.verticalStack
                 ([ Components.formLabel "Connection String Label"
                    Components.formEntry model.ConnectionString.Name updateConnectionStringName
                    Components.formLabel "Connection String Value"
                    Components.formMultiLineEditor model.ConnectionString.Value updateConnectionStringValue
                    testButton
                    saveButton
                    Components.formLabel "Output"
                    Components.formLabel (sprintf "%s" model.Output) ]))

//let bindings model dispatch =
//  [
//    "ConnectionString" |> Binding.twoWay
//        (fun m -> m.ConnectionString )
//        (fun v m -> v |> UpdateConnectionString)
//    "Output" |> Binding.twoWay
//        (fun m -> m.Output )
//        (fun v m -> v |> UpdateConnectionString)
//    "TestConnection" |> Binding.cmdIfValid
//        (fun m -> requiredNotEmpty m.ConnectionString |> Result.map TestConnection)
//  ]