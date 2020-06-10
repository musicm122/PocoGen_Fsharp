module PocoGen.DomainPrimitives

open Rop

type LogMessage =
    | Debug of string
    | Info of string
    | Error of string

type StringError =
    | Missing
    | MustNotBeLongerThan of int
    | DoesntMatchPattern of string

module String10 =
    type T = String10 of string

    let create (s : string) =
        match s with
        | null -> fail StringError.Missing
        | _ when s.Length > 10 -> fail (MustNotBeLongerThan 10)
        | _ -> succeed (String10 s)

    let apply f (String10 s) = f s

module String20 =
    type T = String20 of string

    let create (s : string) =
        match s with
        | null -> fail StringError.Missing
        | _ when s.Length > 20 -> fail (MustNotBeLongerThan 20)
        | _ -> succeed (String20 s)

    let apply f (String20 s) = f s

module String128 =
    type T = String128 of string

    let create (s : string) =
        match s with
        | null -> fail StringError.Missing
        | _ when s.Length > 128 -> fail (MustNotBeLongerThan 128)
        | _ -> succeed (String128 s)

    let apply f (String128 s) = f s


module String260 =
    type T = String260 of string

    let create (s : string) =
        match s with
        | null -> fail StringError.Missing
        | _ when s.Length > 260 -> fail (MustNotBeLongerThan 260)
        | _ -> succeed (String260 s)

    let apply f (String260 s) = f s

type ConnectionName = String10.T

type DatabaseName = String10.T

type TableName = String10.T

type ConnectionStringValue = String128.T

let ToString10(rawVal : string) : String10 =
    let result : String10 = rawVal
    result