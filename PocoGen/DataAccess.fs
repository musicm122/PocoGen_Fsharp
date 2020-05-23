module DataAccess

open Dapper
open System.Data.Common
open System.Data.SqlClient
open Models
open System
open PocoGen
open PocoGen.Common

let testConnection(connectionString:ConnectionStringValue): ConnectionTestResult =
    try
        match connectionString.IsNullOrWhiteSpace() with
        | true ->
            let ex = new ArgumentException("Missing required argument")
            { ConnectionTestResult.Message = "SQL Error "+ex.Message;
              ConnectionTestResult.State = Fail ex}
        | false ->
            use conn = new SqlConnection(connectionString)
            { ConnectionTestResult.Message = "Success";
              ConnectionTestResult.State = Pass }
    with
    | :? SqlException as ex ->
        { ConnectionTestResult.Message = "SQL Error "+ex.Message;
          ConnectionTestResult.State = Fail ex}
    | _ ->
        { ConnectionTestResult.Message = "Unknown Failure";
          ConnectionTestResult.State = Fail (new Exception("Unknown Error")) }