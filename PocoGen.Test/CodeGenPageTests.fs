module CodeGenPageTests

open System
open Xunit
open FsUnit.Xunit
open PocoGen.Models
open PocoGen
open Fabulous

[<Fact>]
let ``Init should return a valid initial state`` () =
    let initState =
        { OutputLocation = DefaultOutputPath
          CurrentFormState = Idle
          Databases = []
          Languages = DefaultLanguages
          Tables = []
          SelectedDatabase = None
          SelectedLanguage = CSharp
          SelectedTables = []
          CodeGenPageState = PageState.Init
          ConnectionString = EmptyConnectionStringItem
          Output = String.Empty }, Cmd.none

    App.init () |> should equal initState
