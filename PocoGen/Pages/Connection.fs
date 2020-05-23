module Connection

open System

open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.BindableHelpers
open Fabulous.XamarinForms.LiveUpdate
open Xamarin.Forms
open Models
open PocoGen

type FormState=
    | Valid
    | MissingConnectionString

type Model =
  { ConnectionString: ConnectionString
    CurrentFormState:FormState
    Output: string }

let init () =
  { ConnectionString =
        {ConnectionString.Value=String.Empty; ConnectionString.Name=String.Empty;}
    CurrentFormState = MissingConnectionString
    Output = String.Empty }

type Msg =
  | UpdateConnectionString of string
  | UpdateConnectionStringName of string
  | TestConnection of ConnectionString
  | SaveConnectionString of ConnectionString
  | UpdateOutput of string

let runConnectionTest(model:Model):Model =
    let connectionTestResult = DataAccess.testConnection model.ConnectionString
    let testResult =
        match connectionTestResult.State with
        | Pass -> { model with Output = connectionTestResult.Message }
        | Fail ex -> { model with Output = connectionTestResult.Message }
        | _ -> { model with Output = String.Empty }
    testResult

let saveConnection(model:Model):Model =
    try
        async {Store.add(model.ConnectionString)}
    with
    | :? Exception

let update (msg:Msg) (m:Model) =
  match msg with
  | UpdateConnectionString conStringVal ->
        { m with
            ConnectionString = {ConnectionString.Value = conStringVal};
            CurrentFormState = if String.IsNullOrWhiteSpace conStringVal then MissingConnectionString else Valid
        }, Cmd.none
  | UpdateOutput output -> {m with Output = output},Cmd.none
  | TestConnection con ->
        let testResult = runConnectionTest m
        { m with Output = testResult.Output+"\r\n" }, Cmd.none

let view (model:Model) dispatch =
    let UpdateConnectionString =  UpdateConnectionString >> dispatch
    let testConnection = (fun () -> dispatch (TestConnection model.ConnectionString ) )
    let saveConnectionString = (fun () -> dispatch (TestConnection model.ConnectionString ) )
    let testButton =
        (match model.CurrentFormState with
        | Valid -> Components.formButton "Test" testConnection true
        | MissingConnectionString -> Components.formButton "Test" testConnection false)

    View.ContentPage(
        padding = Thickness(5.0),
        title = "Connection Test",
        content =
            Components.formStack([
                Components.formLabel "Connection String"
                Components.formEntry model.ConnectionString.connStringValue UpdateConnectionString
                Components.formEditor model.ConnectionString.connStringValue UpdateConnectionString
                testButton
                Components.formButton "Save Connection String" testConnection true
                Components.formLabel "Output"
                Components.formLabel (sprintf "%s" model.Output)
            ]))

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