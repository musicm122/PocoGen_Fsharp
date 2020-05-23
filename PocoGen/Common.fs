namespace PocoGen

open System
open Microsoft.FSharp.Reflection
open System.Runtime.CompilerServices

module Common =
    ///Returns the case name of the object with union type 'ty.
    let GetUnionCaseName (x:'a) =
        match FSharpValue.GetUnionFields(x, typeof<'a>) with
        | case, _ -> case.Name

    ///Returns the case names of union type 'ty.
    let GetUnionCaseNames<'ty> () =
        FSharpType.GetUnionCases(typeof<'ty>) |> Array.map (fun info -> info.Name)



    [<Extension>]
    type stringExtensions =
        [<Extension>]
        static member inline IsNullOrEmpty(_str: string) =
            String.IsNullOrEmpty(_str)
        [<Extension>]
        static member inline IsNullOrWhiteSpace(_str: string) =
            String.IsNullOrWhiteSpace(_str)