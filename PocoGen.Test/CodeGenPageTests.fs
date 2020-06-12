module CodeGenPageTests

open Xunit
open FsUnit.Xunit
open PocoGen.DomainModels
open PocoGen.Page
open PocoGen.Page.CodeGenPage

[<Fact>]
let ``Init should return a valid initial state``() =
    let initState =
        { OutputLocation = DefaultOutputPath
          ConnectionStrings = []
          Databases = []
          Languages = DefaultLanguages
          Tables = []
          SelectedConnectionString = None
          SelectedDatabase = None
          SelectedLanguage = Language.CSharp
          SelectedTables = []
          CodeGenPageState = PageState.Init }
    CodeGenPage.initModel() |> should equal initState