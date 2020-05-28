module DataAccess

open Dapper
open System.Data.Common
open System.Data.SqlClient
open Models
open System
open PocoGen
open PocoGen.Common

let testConnection (connectionString : ConnectionStringValue) : ConnectionTestResult =
    try
        match connectionString.IsNullOrWhiteSpace() with
        | true ->
            let ex = ArgumentException("Missing required argument")
            { ConnectionTestResult.Message = "SQL Error " + ex.Message
              ConnectionTestResult.State = Fail ex }
        | false ->
            use conn = new SqlConnection(connectionString)
            { ConnectionTestResult.Message = "Success"
              ConnectionTestResult.State = Pass }
    with
    | :? SqlException as ex ->
        { ConnectionTestResult.Message = "SQL Error " + ex.Message
          ConnectionTestResult.State = Fail ex }
    | _ ->
        { ConnectionTestResult.Message = "Unknown Failure"
          ConnectionTestResult.State = Fail(new Exception("Unknown Error")) }

let getTableNamesFromMSSqlServerQueryAsync (database : Database) (conString : ConnectionString) =
    async {
        use connection = new SqlConnection(conString.Value)
        do! connection.OpenAsync() |> Async.AwaitTask
        let sql = @"SELECT [t0].[TABLE_NAME]
        FROM [INFORMATION_SCHEMA].[TABLES] AS [t0]
        WHERE [t0].[TABLE_CATALOG] = @dbName"
        let! result = connection.QueryAsync<string>(sql, { Name = database.Name }) |> Async.AwaitTask
        return result |> Array.ofSeq
    }

let getTableNamesFromMSSqlServerQuery (database : Database) (conString : ConnectionString) =
    use connection = new SqlConnection(conString.Value)
    do connection.Open()
    let sql = @"SELECT [t0].[TABLE_NAME]
        FROM [INFORMATION_SCHEMA].[TABLES] AS [t0]
        WHERE [t0].[TABLE_CATALOG] = @dbName"
    let result = connection.Query<string>(sql, { Name = database.Name })
    result |> Array.ofSeq

let getDatabaseNames (connString : ConnectionString) =
    use connection = new SqlConnection(connString.Value)
    do connection.Open()
    let sql = @"select name from sys.databases"
    let result = connection.Query<string>(sql)
    connection.Close()
    result |> Array.ofSeq

let getDatabaseNamesAsync (connString : ConnectionString) =
    async {
        use connection = new SqlConnection(connString.Value)
        do! connection.OpenAsync() |> Async.AwaitTask
        let sql = @"select name from sys.databases"
        let! result = connection.QueryAsync<string>(sql) |> Async.AwaitTask
        connection.Close()
        return result |> Array.ofSeq
    }