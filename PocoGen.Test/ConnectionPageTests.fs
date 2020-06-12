module ConnectionPageTests

open System
open Xunit
open FsUnit.Xunit
open PocoGen.DomainModels
open PocoGen.Page
open PocoGen.Page.ConnectionPage

[<Fact>]
let ``Init should return a valid initial state``() =
    let initState =
        { ConnectionPage.Model.ConnectionString = EmptyConnectionStringItem
          ConnectionPage.Model.CurrentFormState = ConnectionPage.FormState.MissingConnStrValue
          ConnectionPage.Model.Output = String.Empty }
    ConnectionPage.init() |> should equal initState