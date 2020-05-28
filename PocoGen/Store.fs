module Store

open Xamarin.Essentials
open Models
open System
open FSharp.Data.Dapper
open LiteDB
open LiteDB.FSharp


let dbFileName = "conStrings.db"
let collectionName = "ConnectionStrings"

let addConnectionString (connString : ConnectionStringItem) : StoreConnectionStringResult =
    try
        let mapper = FSharpBsonMapper()
        use db = new LiteDatabase(dbFileName, mapper)
        db.GetCollection<ConnectionStringItem>(collectionName)
            .Insert(
                { ConnectionStringItem.Id=0
                  ConnectionStringItem.Name=connString.Name
                  ConnectionStringItem.Value=connString.Value })
            |> ignore
        StoreConnectionStringResult.Success
    with
        :? Exception as ex -> StoreConnectionStringResult.Error ex.Message

let getConnectionStringById (id:int) : ConnectionStringItem =
    let mapper = FSharpBsonMapper()
    use db = new LiteDatabase(dbFileName, mapper)
    db.GetCollection<ConnectionStringItem>(collectionName).FindOne(fun c->c.Id = id)


let getAllConnectionStrings () : ConnectionStringItem list =
    let mapper = FSharpBsonMapper()
    use db = new LiteDatabase(dbFileName, mapper)
    db.GetCollection<ConnectionStringItem>(collectionName).FindAll() |> List.ofSeq
