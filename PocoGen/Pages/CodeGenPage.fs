module PocoGen.Page.CodeGenPage

open System
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms
open PocoGen.DomainModels
open PocoGen
open Common
open PocoGen.Components

type PageState =
    | MissingRequiredFields of string
    | ValidForm
    | Init

type Model =
    { OutputLocation: FileOutputPath
      ConnectionStrings: ConnectionStringItem list
      Databases: DbItem list
      Languages: Language list
      Tables: Table list
      SelectedConnectionString: ConnectionStringItem option
      SelectedDatabase: DbItem option
      SelectedLanguage: Language
      SelectedTables: Table list
      CodeGenPageState: PageState }

let hasStoredConnectionStrings (): bool = Store.getAllConnectionStrings().IsEmpty

let fetchConnString (): ConnectionStringItem list = Store.getAllConnectionStrings ()

let fetchDatabases (conn: ConnectionStringItem): DbItem list = DataAccess.getDatabaseNames conn

let fetchTables (dbName: DbItem) (conn: ConnectionStringItem): Table list =
    DataAccess.getTableNamesFromMSSqlServerQuery dbName conn

type Msg =
    | LoadConnectionStrings
    | ConnectionStringsLoaded of ConnectionStringItem list
    | ConnectionStringsLoadFailed of string
    | SetSelectedDatabase of DbItem option
    | SetSelectedLanguage of Language
    | SetSelectedConnection of ConnectionStringItem
    | BrowseForOutputFolder of FileOutputPath
    | GenerateCode

let initModel (): Model =

    let conStrings =
        if hasStoredConnectionStrings () then fetchConnString () else []

    let selectedConString: ConnectionStringItem option =
        if conStrings.Length > 0 then Some conStrings.Head else None

    let dbs: DbItem list =
        match selectedConString with
        | Some conStr -> fetchDatabases conStr
        | None -> []

    let selectedDb: DbItem option =
        if dbs.Length > 0 then Some dbs.Head else None

    let tables: Table list =
        match selectedDb with
        | Some db -> fetchTables db selectedConString.Value
        | None -> []

    { OutputLocation = DefaultOutputPath
      ConnectionStrings = conStrings
      Databases = dbs
      Languages = DefaultLanguages
      Tables = tables
      SelectedConnectionString = selectedConString
      SelectedDatabase = selectedDb
      SelectedLanguage = DefaultLanguages.Head
      SelectedTables = []
      CodeGenPageState = PageState.Init }

let init = initModel, Cmd.none

let update msg m: Model * Cmd<Msg> =
    match msg with
    | SetSelectedDatabase db -> { m with SelectedDatabase = db }, Cmd.none
    | SetSelectedLanguage l -> { m with SelectedLanguage = l }, Cmd.none
    | SetSelectedConnection con ->
        { m with
              SelectedConnectionString = Some(con) },
        Cmd.none
    | BrowseForOutputFolder f -> { m with OutputLocation = f }, Cmd.none
    | GenerateCode -> m, Cmd.none
    | ConnectionStringsLoadFailed err -> m, Cmd.none
    | _ -> m, Cmd.none

let hasADatabaseSelected (m: Model): PageState =
    match m.SelectedDatabase.IsSome with
    | false -> MissingRequiredFields "Selected database is required"
    | _ -> ValidForm

let hasAtLeastOneTableSelected (m: Model): PageState =
    match m.SelectedTables |> List.isEmpty with
    | true -> ValidForm
    | _ -> MissingRequiredFields "At least one table is required"

let hasValidOutputFolder (m: Model): PageState =
    match IsValidPath m.OutputLocation with
    | true -> ValidForm
    | _ -> MissingRequiredFields "Invalid output path"

let hasRequiredFields = hasADatabaseSelected

let toDbItemPickerOption (dbs: DbItem list): string list = dbs |> List.map (fun db -> db.Name)

let toLangagePickerOption (langs: Language list): string list =
    langs |> List.map (fun l -> l.ToString())

let toConStrPickerOptions (constrs: ConnectionStringItem list): string list =
    constrs
    |> List.map (fun c -> c.Id.ToString() + c.Name)

let toTableCell (tables: Table list): ViewElement list =
    tables |> List.map (fun i -> View.TextCell i.Name)

let view (model: Model) dispatch: ViewElement =
    let dbItems = model.Databases |> toDbItemPickerOption
    let langs = model.Languages |> toLangagePickerOption

    let conStrs =
        model.ConnectionStrings |> toConStrPickerOptions

    let tables = model.Tables |> toTableCell

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
                             View.Picker(items = langs, title = "Languages") ])
                        innerLayout
                            ([ Components.formLabel "Code Gen"
                               View.ListView(items = tables) ]) ]))
