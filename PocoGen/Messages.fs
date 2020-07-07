module PocoGen.Messages

open PocoGen.Models

type Msg =
    | UpdateConnectionStringValue of string
    | UpdateConnectionStringName of string
    | TestConnection
    | TestConnectionComplete of Model
    | SetSelectedDatabase of string option  
    | SetSelectedDatabaseComplete of Model
    | SetSelectedLanguage of Language
    | SetSelectedConnection of ConnectionStringItem
    | BrowseForOutputFolder of FileOutputPath
    | GenerateCode
    | FetchTables of DbItem
    | FetchTablesComplete of Model
    | FetchDatabases
    | FetchDatabasesComplete of Model
