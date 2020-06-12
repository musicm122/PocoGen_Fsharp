module Tests

open System
open Xunit
open FsUnit
open FsUnit.Xunit
open PocoGen
open PocoGen.App
open Fabulous
open Models


    [<Fact>]
    let ``Init should return a valid initial state``() =
        let expectedInitState=
            { ConnectionPageModel= ConnectionPage.init()
              CodeGenPageModel = CodeGenPage.initModel()
              WorkaroundNavPageBug= false
              WorkaroundNavPageBugPendingCmd = Cmd.none}, Cmd.none
        let actualInitState = App.init()
        let expectedFst = expectedInitState|> fst
        let actualFst = actualInitState |> fst

        let expectedSnd = expectedInitState|> snd
        let actualSnd = actualInitState |> snd

        expectedFst.CodeGenPageModel |> should equal actualFst.CodeGenPageModel
        expectedFst.ConnectionPageModel |> should equal actualFst.ConnectionPageModel
        expectedFst.WorkaroundNavPageBug |> should equal actualFst.WorkaroundNavPageBug
        expectedFst.WorkaroundNavPageBugPendingCmd |> should equal actualFst.WorkaroundNavPageBugPendingCmd
        expectedSnd |> should equal actualSnd