module CodeGenPage

open System
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms
open Models
open PocoGen
open Common
open Store

type Model =
    { OutputLocation: FileOutputPath
      ConnectionStrings: ConnectionStringItem list
      Databases: Database list
      Languages: Language list
      Tables: Table list
      SelectedConnectionString: ConnectionStringItem option
      SelectedDatabase: Database option
      SelectedLanguage: Language
      SelectedTables: Table list }

let fetchConnString (): ConnectionStringItem list = Store.getAllConnectionStrings ()

type Msg =
    | LoadConnectionStrings
    | ConnectionStringsLoaded of ConnectionStringItem list
    | ConnectionStringsLoadFailed of string
    | SetSelectedDatabase of Database option
    | SetSelectedLanguage of Language
    | SetSelectedConnection of ConnectionStringItem
    | BrowseForOutputFolder of FileOutputPath
    | PopulateAvailableDatabases of Database list
    | PopulateAvailableTables of Table list
    | GenerateCode

let initModel () =
    { OutputLocation =
          { FileName = String.Empty
            FilePath = @"c:\" }
      ConnectionStrings = []
      Databases = []
      Languages = [ Language.CSharp; Language.VbNet ]
      Tables = []
      SelectedConnectionString = None
      SelectedDatabase = None
      SelectedLanguage = Language.CSharp
      SelectedTables = [] }

let init = initModel, Cmd.none

let update msg m =
    match msg with
    | SetSelectedDatabase db -> { m with SelectedDatabase = db }, Cmd.none
    | SetSelectedLanguage l -> { m with SelectedLanguage = l }, Cmd.none
    | SetSelectedConnection con ->
        { m with
              SelectedConnectionString = Some(con) },
        Cmd.none
    | BrowseForOutputFolder f -> { m with OutputLocation = f }, Cmd.none
    | PopulateAvailableDatabases dbs -> { m with Databases = dbs }, Cmd.none
    | PopulateAvailableTables tbls -> { m with Tables = tbls }, Cmd.none
    | GenerateCode -> m, Cmd.none
    | LoadConnectionStrings ->
        { m with
              ConnectionStrings = fetchConnString () },
        Cmd.none
    | ConnectionStringsLoaded cs -> { m with ConnectionStrings = cs }, Cmd.none
    | ConnectionStringsLoadFailed err -> m, Cmd.none

let view (model: Model) dispatch =
    let dbItems =
        model.Databases |> List.map (fun db -> db.Name)

    let languages =
        model.Languages
        |> List.map (fun l -> l.ToString())

    let conStrs =
        fetchConnString ()
        |> List.map (fun c -> c.Id.ToString() + c.Name)

    let tables =
        [ "Table1"
          "Table2"
          "Table3"
          "Table4" ]
        |> List.map (fun i -> View.TextCell i)

    let innerLayout children =
        View.StackLayout
            (orientation = StackOrientation.Vertical,
             verticalOptions = LayoutOptions.StartAndExpand,
             horizontalOptions = LayoutOptions.Start,
             margin = Thickness(1.0),
             children = children)

    View.ContentPage
        (padding = Thickness(5.0),
         title = "Code Gen",
         content =
             View.StackLayout
                 (orientation = StackOrientation.Horizontal,
                  verticalOptions = LayoutOptions.StartAndExpand,
                  horizontalOptions = LayoutOptions.Start,
                  padding = Thickness(1.0),
                  margin = Thickness(20.0),
                  children =
                      [ innerLayout
                          ([ Components.formLabel "Connection Strings"
                             View.Picker(items = conStrs, title = "Connection Strings")
                             Components.formLabel "Databases"
                             View.Picker(items = dbItems, title = "Databases")
                             Components.formLabel "Languages"
                             View.Picker(items = languages, title = "Languages") ])
                        innerLayout
                            ([ Components.formLabel "Code Gen"
                               View.ListView(items = tables) ]) ]))

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
