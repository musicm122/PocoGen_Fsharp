module Store

open Xamarin.Essentials
open Models
open System
open Microsoft.Data.Sqlite
open FSharp.Data.Dapper
open LiteDB
open LiteDB.FSharp

(*
let addConnectionString (connString : ConnectionString) : Async<StoreConnectionStringResult> =
    async {
        try
            SecureStorage.SetAsync("con:" + connString.Name, connString.Value)
            |> Async.AwaitTask
            |> ignore
            return StoreConnectionStringResult.Success
        with :? Exception as ex -> return StoreConnectionStringResult.Error ex.Message
    }

let getConnectionString (connName : ConnectionName) : Async<ConnectionString> =
    async {
        let! result = SecureStorage.GetAsync(connName) |> Async.AwaitTask
        return { ConnectionString.Name = connName
                 ConnectionString.Value = result }
    }
*)

module Connection =
    let private mkConnectionString (dataSource : string) =
        sprintf "Data Source = %s; Mode = Memory; Cache = Shared;" dataSource

    let private mkOnDiskConnectionString (dataSource : string) = sprintf "Data Source = %s;" dataSource

    let mkShared() = new SqliteConnection(mkConnectionString "MASTER")

    let mkOnDisk() = new SqliteConnection(mkOnDiskConnectionString "./example.db")

module Types =

    [<CLIMutable>]
    type ConnStr =
        { Id : int64
          Name : string
          Value : string }

module Queries =
    let private connectionF() = Connection.SqliteConnection(Connection.mkOnDisk())

    let querySeqAsync<'R> = querySeqAsync<'R> (connectionF)
    let querySingleAsync<'R> = querySingleOptionAsync<'R> (connectionF)

    module Schema =
        let CreateTables =
            querySingleAsync<int> {
                script """
                    DROP TABLE IF EXISTS ConnStr;
                    CREATE TABLE ConnStr (
                        Id INTEGER PRIMARY KEY,
                        Name VARCHAR(255) NOT NULL,
                        Value TEXT NOT NULL
                    );
                """
            }

    module ConnectionString =
        let New name value =
            querySingleAsync<int> {
                script "INSERT INTO ConnStr (Name, Value) VALUES (@Name, @Value)"
                parameters
                    (dict
                        [ "Name", box name
                          "Value", box value ])
            }

        let GetSingleByName name =
            querySingleAsync<Types.ConnStr> {
                script "SELECT * FROM ConnStr WHERE Name = @Name LIMIT 1"
                parameters (dict [ "Name", box name ])
            }

        let GetAll() = querySeqAsync<Types.ConnStr> { script "SELECT * FROM ConnStr" }

        let UpdateValueByName name value =
            querySingleAsync<int> {
                script "UPDATE ConnStr SET Value = @Value WHERE Name = @Name"
                parameters
                    (dict
                        [ "Value", box value
                          "Name", box name ])
            }

        let DeleteByName name =
            querySingleAsync<int> {
                script "DELETE FROM ConnStr WHERE Name = @Name"
                parameters (dict [ "Name", box name ])
            }
