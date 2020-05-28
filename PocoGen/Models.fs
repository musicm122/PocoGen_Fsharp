module Models

open System
open System.Data.SqlClient

type LogMessage =
    | Debug of string
    | Info of string
    | Error of string

type ConnectionName = string
type ConnectionStringValue = string

[<CLIMutable>]
type ConnectionStringItem = {
    Id:int
    Name:string
    Value:string
}

//type ConnectionString =
//    { Name: ConnectionName
//      Value: ConnectionStringValue }

[<NoComparison>]
type ConnectionTestState =
    | Pass
    | Fail of Exception
    | WIP
    | NotStarted

[<Struct>]
[<NoComparison>]
type ConnectionTestResult =
    { State: ConnectionTestState
      Message: string }

type Database = { Name: string }

type Table = { Name: string; Database: Database }

type Query =
    { Database: Database
      Sql: string
      ConnectionString: ConnectionStringItem }

type Language =
    | CSharp
    | VbNet

type GetSetValue =
    | Get
    | Set
    | GetAndSet

type AccessModifier =
    | Public
    | Private
    | Internal
    | Protected

type ValueType =
    { Name: string
      Value: Type
      IsNullable: bool }

type Property =
    { TypeDetail: ValueType
      IsNullable: bool
      ColumnName: string
      Access: AccessModifier
      GetSet: GetSetValue }

type ClassObject =
    { ClassName: string
      Access: AccessModifier
      Properties: Property []
      Language: Language }

type ClassResult = { Raw: string }

type RawRowData =
    { IsNullable: bool
      DataType: string
      ColumnName: string }

type FileOutputPath = { FileName: string; FilePath: string }

type StoreConnectionStringResult =
    | Success
    | Error of string
