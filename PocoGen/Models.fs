module Models

open System
open System.Data.SqlClient

type LogMessage =
    | Debug of string
    | Info of string
    | Error of string

type ConnectionName = string
type ConnectionStringValue = string

type ConnectionString = {
    Name:ConnectionName
    Value:ConnectionStringValue
}

[<NoComparison>]
type ConnectionTestState =
    |Pass
    |Fail of Exception
    |WIP
    |NotStarted

[<Struct>]
[<NoComparison>]
type ConnectionTestResult = {
    State : ConnectionTestState;
    Message : string
}

type Database ={
    Name:string;
}

type Table = {
    name:string
    database:Database
}

type Query= {
    database:Database;
    sql:string;
    connectionString:ConnectionString
}

type Language =
    |CSharp
    |VbNet

type GetSetValue =
    |Get
    |Set
    |GetAndSet

type AccessModifier =
    | Public
    | Private
    | Internal
    | Protected

type ValueType ={
    name:string;
    value:Type;
    isNullable:bool;
}

type Property = {
    typeDetail:ValueType;
    isNullable:bool;
    columnName:string;
    access:AccessModifier;
    GetSet:GetSetValue
}

type ClassObject = {
    className:string;
    access:AccessModifier;
    properties:Property[]
    language:Language
}

type ClassResult = {
    raw:string;
}

type RawRowData = {
    IsNullable:bool;
    DataType:string;
    ColumnName:string;
}

type FileOutputPath = {
    fileName:string;
    filePath:string;
}

type StoreConnectionStringResult =
    | Success
    | Error of string

