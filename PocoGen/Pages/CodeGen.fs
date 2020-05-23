module CodeGen

open System
open Models
open Fabulous
open Fabulous.XamarinForms
open CommonExtensions

type Model =
  { OutputLocation: FileOutputPath
    Databases: Database list
    Languages: Language list
    Tables: Table list
    SelectedDatabase: Database option
    SelectedLanguage: Language
    SelectedTables:Table list }

let init () =
  { OutputLocation = { FileOutputPath.filePath= @"c:\"; FileOutputPath.fileName = String.Empty; }
    Databases = []
    Languages = [Language.CSharp; Language.VbNet]
    Tables=[]
    SelectedDatabase = None
    SelectedLanguage = Language.CSharp
    SelectedTables = [] }

type Msg =
  | SetSelectedDatabase of Database option
  | SetSelectedLanguage of Language
  | BrowseForOutputFolder of FileOutputPath
  | PopulateAvailableDatabases of Database list
  | PopulateAvailableTables of Table list
  | GenerateCode

let update msg m =
  match msg with
  | SetSelectedDatabase db ->
        { m with SelectedDatabase = db }, Cmd.none
  | SetSelectedLanguage l ->
        { m with SelectedLanguage = l }, Cmd.none
  | BrowseForOutputFolder f ->
        { m with OutputLocation= f }, Cmd.none
  | PopulateAvailableDatabases dbs ->
        { m with Databases = dbs }, Cmd.none
  | PopulateAvailableTables tbls ->
        {m with Tables = tbls }, Cmd.none
  | GenerateCode ->
        m, Cmd.none

let view (model:Model) dispatch =
    View.ContentPage(
        title = "Code Gen",
        content = View.StackLayout(
            children =[
                View.Label(text = sprintf "Databases")
                View.ListView(items = [
                    for db in model.Databases do
                        yield View.TextCell(db.Name)
                ])
                //View.Label(text = sprintf "Languages")
                //View.ListView(items = [
                //    for lang in model.Languages do
                //        yield View.TextCell(PocoGen.Common.GetUnionCaseName lang)
                //])
                //View.Label(text = sprintf "Tables")
                //View.ListView(items = [
                //    for table in model.Tables do
                //        yield View.TextCell(table.name)
                //])
                View.Label(text = sprintf "%s" "Code Gen Page")
            ]))

//let hasADatabaseSelected(m:Model) =
//    match m.SelectedDatabase.IsSome with
//    | false -> m. Error "Selected database is required"
//    | _ -> Ok m

//let hasAtLeastOneTableSelected(m:Model) =
//    match m.SelectedTables |> List.isEmpty with
//    | true -> Ok m
//    | _ -> Error "At least one table is required"

//let hasValidOutputFolder(m:Model) =
//    match FileSystemService.IsValidPath m.OutputLocation with
//    | true -> Ok m
//    | _ -> Error "Invalid output path"

//let hasRequiredFields =
//    hasADatabaseSelected
//    >> Result.bind hasAtLeastOneTableSelected
//    >> Result.bind hasValidOutputFolder

//let bindings model dispatch =
//  [
//    "OutputLocation"|>Binding.twoWay
//        (fun m -> m.OutputLocation )
//        (fun v m -> v |> BrowseForOutputFolder)
//    "Databases" |> Binding.twoWay
//        (fun m -> m.Databases )
//        (fun v m -> v |> PopulateAvailableDatabases)
//    "Tables" |> Binding.twoWay
//        (fun m -> m.Tables )
//        (fun v m -> v |> PopulateAvailableTables)
//    "SelectedDatabase" |> Binding.twoWay
//        (fun m -> m.SelectedDatabase )
//        (fun v m -> v |> SetSelectedDatabase)
//    "SelectedLanguage" |> Binding.twoWay
//        (fun m -> m.SelectedLanguage)
//        (fun v m -> v |> SetSelectedLanguage)
//    //"GenerateCode" |> Binding.cmdIfValid
//]