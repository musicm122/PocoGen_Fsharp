module PocoGen.DomainModels

open System

type DomainMessages =
    | MissingConnectionString
    | InvalidConnectionString

type LogMessage =
    | Debug of string
    | Info of string
    | Error of string

type ConnectionName = string

type ConnectionStringValue = string

[<CLIMutable>]
type ConnectionStringItem =
    { Id: int
      Name: string
      Value: string }

[<NoComparison>]
type ConnectionTestState =
    | Pass
    | Fail of Exception
    | NotStarted

[<Struct>]
[<NoComparison>]
type ConnectionTestResult =
    { State: ConnectionTestState
      Message: string }

type DbItem = { Name: string }

let CreateDbItem (dbVal: string): DbItem = { DbItem.Name = dbVal }

type Table = { Name: string; Database: DbItem }

type Query =
    { Database: DbItem
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
