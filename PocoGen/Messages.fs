module PocoGen.Messages
open PocoGen.Models

    type Msg =
        | UpdateConnectionStringValue of string
        | UpdateConnectionStringName of string
        | TestConnection
        | TestConnectionComplete of Model
        | SetSelectedDatabase of DbItem option
        | SetSelectedLanguage of Language
        | SetSelectedConnection of ConnectionStringItem
        | BrowseForOutputFolder of FileOutputPath
        | GenerateCode        
        | FetchTables
        | FetchTablesComplete of Model
        | FetchDatabases
        | FetchDatabasesComplete of Model
