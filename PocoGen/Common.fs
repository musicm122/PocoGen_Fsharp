namespace PocoGen

open System
open Microsoft.FSharp.Reflection
open System.Runtime.CompilerServices
open System.IO
open PocoGen.DomainModels

module Common =
    [<Extension>]
    type StringExtensions =

        [<Extension>]
        static member inline IsNullOrEmpty(_str: string) = String.IsNullOrEmpty(_str)

        [<Extension>]
        static member inline IsNullOrWhiteSpace(_str: string) = String.IsNullOrWhiteSpace(_str)

    let IsValidPath (path: FileOutputPath) =
        try
            let dir = DirectoryInfo(path.FilePath)
            dir.Exists
        with
        | :? ArgumentException -> false
        | :? PathTooLongException -> false
        | :? NotSupportedException -> false
        | _ -> false
