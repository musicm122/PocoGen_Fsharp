module PocoGen.DataAccess

open System.Data
open Dapper
open System.Data.SqlClient
open PocoGen.Models
open System
open Extensions

let testConnectionAsync (connectionString: ConnectionStringValue): Async<ConnectionTestResult> =
    async {
        try
            match connectionString.IsNullOrWhiteSpace() with
            | true ->
                let ex =
                    ArgumentException("Missing required argument")

                return { ConnectionTestResult.Message = "SQL Error " + ex.Message
                         ConnectionTestResult.State = Fail ex.Message }
            | false ->
                use conn = new SqlConnection(connectionString)
                let! connOpenResult = conn.OpenAsync() |> Async.AwaitTask
                
                match conn.State with
                | ConnectionState.Open ->
                    return { ConnectionTestResult.Message = "Success"
                             ConnectionTestResult.State = Pass }
                | _ ->
                    let connVal =
                        Enum.GetName(typeof<ConnectionState>, conn.State)

                    let err =
                        sprintf "Could not connect: Connection state of %s" connVal

                    return { ConnectionTestResult.Message = "Fail"
                             ConnectionTestResult.State = Fail err }
        with :? SqlException as ex ->
            return { ConnectionTestResult.Message = "Error : SQL Exception :" + ex.Message
                     ConnectionTestResult.State = Fail ex.Message }
    }

let getTableNamesFromMSSqlServerQueryAsync (database: DbItem) (conString: ConnectionStringItem): Async<Table list> =
    async {
        use connection = new SqlConnection(conString.Value)
        do! connection.OpenAsync() |> Async.AwaitTask

        let sql = @"SELECT [t0].[TABLE_NAME]
        FROM [INFORMATION_SCHEMA].[TABLES] AS [t0]
        WHERE [t0].[TABLE_CATALOG] = @dbName"

        let! result =
            connection.QueryAsync<string>(sql, { Name = database.Name })
            |> Async.AwaitTask
        return result
               |> Seq.map (fun tVals ->
                   { Table.Name = tVals
                     Table.Database = database })
               |> Seq.toList
    }

let getTableNamesFromMSSqlServerQuery (database: DbItem) (conString: ConnectionStringItem): Table list =
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

let getDatabaseNames (connString: ConnectionStringItem): DbItem list =
    use connection = new SqlConnection(connString.Value)
    do connection.Open()
    let sql = @"select name from sys.databases"
    let result = connection.Query<string>(sql)
    connection.Close()
    result
    |> Seq.map (fun dVal -> { DbItem.Name = dVal })
    |> Seq.toList

let getDatabaseNamesAsync (connString: ConnectionStringItem): Async<DbItem list> =
    async {
        use connection = new SqlConnection(connString.Value)
        do! connection.OpenAsync() |> Async.AwaitTask
        let sql = @"select name from sys.databases"
        let! result =
            connection.QueryAsync<string>(sql)
            |> Async.AwaitTask
        connection.Close()
        return result
               |> Seq.map (fun dVal -> { DbItem.Name = dVal })
               |> Seq.toList
    }
