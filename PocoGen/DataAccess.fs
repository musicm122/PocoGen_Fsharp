module PocoGen.DataAccess

open Dapper
open System.Data.SqlClient
open PocoGen.DomainModels
open System
open PocoGen.Common

let testConnection (connectionString: ConnectionStringValue): ConnectionTestResult =
    try
        match connectionString.IsNullOrWhiteSpace() with
        | true ->
            let ex =
                ArgumentException("Missing required argument")

            { ConnectionTestResult.Message = "SQL Error " + ex.Message
              ConnectionTestResult.State = Fail ex }
        | false ->
            use conn = new SqlConnection(connectionString)
            conn.Open()
            conn.Close()
            { ConnectionTestResult.Message = "Success"
              ConnectionTestResult.State = Pass }
    with :? SqlException as ex ->
        { ConnectionTestResult.Message = "Error :" + ex.Message
          ConnectionTestResult.State = Fail ex }

let testConnectionAsync (connectionString: ConnectionStringValue): Async<ConnectionTestResult> =
    async {        
        try
            match connectionString.IsNullOrWhiteSpace() with
            | true ->
                let ex =
                    ArgumentException("Missing required argument")

                return { ConnectionTestResult.Message = "SQL Error " + ex.Message
                         ConnectionTestResult.State = Fail ex }
            | false ->
                use conn = new SqlConnection(connectionString)
                conn.OpenAsync()
                |> Async.AwaitTask
                |> ignore
                conn.Close()
                return { ConnectionTestResult.Message = "Success"
                         ConnectionTestResult.State = Pass }
        with :? SqlException as ex ->
            return { ConnectionTestResult.Message = "Error :" + ex.Message
                     ConnectionTestResult.State = Fail ex }
    }

let getTableNamesFromMSSqlServerQueryAsync (database: DbItem) (conString: ConnectionStringItem) =
    async {
        use connection = new SqlConnection(conString.Value)
        do! connection.OpenAsync() |> Async.AwaitTask

        let sql = @"SELECT [t0].[TABLE_NAME]
        FROM [INFORMATION_SCHEMA].[TABLES] AS [t0]
        WHERE [t0].[TABLE_CATALOG] = @dbName"

        let! result = connection.QueryAsync<string>(sql, { Name = database.Name }) |> Async.AwaitTask
        return result
               |> Seq.map (fun tVals ->
                   { Table.Name = tVals
                     Table.Database = database })
               |> Seq.toList
    }

let getTableNamesFromMSSqlServerQuery (database: DbItem) (conString: ConnectionStringItem) =
    use connection = new SqlConnection(conString.Value)
    do connection.Open()

    let sql = @"SELECT [t0].[TABLE_NAME]
        FROM [INFORMATION_SCHEMA].[TABLES] AS [t0]
        WHERE [t0].[TABLE_CATALOG] = @dbName"

    let result =
        connection.Query<string>(sql, { Name = database.Name })

    result
    |> Seq.map (fun tVals ->
        { Table.Name = tVals
          Table.Database = database })
    |> Seq.toList

let getDatabaseNames (connString: ConnectionStringItem) =
    use connection = new SqlConnection(connString.Value)
    do connection.Open()
    let sql = @"select name from sys.databases"
    let result = connection.Query<string>(sql)
    connection.Close()
    result
    |> Seq.map (fun dVal -> { DbItem.Name = dVal })
    |> Seq.toList

let getDatabaseNamesAsync (connString: ConnectionStringItem) =
    async {
        use connection = new SqlConnection(connString.Value)
        do! connection.OpenAsync() |> Async.AwaitTask
        let sql = @"select name from sys.databases"
        let! result = connection.QueryAsync<string>(sql) |> Async.AwaitTask
        connection.Close()
        return result
               |> Seq.map (fun dVal -> { DbItem.Name = dVal })
               |> Seq.toList
    }
