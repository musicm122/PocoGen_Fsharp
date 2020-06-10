module ConnectionPageTests

open System
open Xunit
open FsUnit
open FsUnit.Xunit
open PocoGen
open PocoGen.App
open Fabulous
open Models

let emptyConnectionStringItem =
    { ConnectionStringItem.Id = 0
      ConnectionStringItem.Value = String.Empty
      ConnectionStringItem.Name = String.Empty }

[<Fact>]
let ``Init should return a valid initial state``() =
    let initState =
        { ConnectionPage.Model.ConnectionString = emptyConnectionStringItem
          ConnectionPage.Model.CurrentFormState = ConnectionPage.FormState.MissingConnStrValue
          ConnectionPage.Model.Output = String.Empty }
    ConnectionPage.init() |> should equal initState