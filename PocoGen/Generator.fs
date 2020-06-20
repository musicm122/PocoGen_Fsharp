module PocoGen.Generator

open System.Data

//let TypeAliases = new Dictionary<Type, string> ()
let DumpCSharpClass (connection: IDbConnection) (sql: string) (className: string): string =

    let buildClassFromTable = CsharpFormatting.buildClass className

    connection.Open()
    let cmd = connection.CreateCommand()
    cmd.CommandText <- sql
    let reader = cmd.ExecuteReader()

    let resultClass =
        reader.GetSchemaTable().Select()
        |> buildClassFromTable

    connection.Close()
    resultClass
