module PocoGen.DomainPrimitives

open Rop

type StringError =
    | Missing
    | MustNotBeLongerThan of int
    | DoesntMatchPattern of string

module String128 =
    type T = String128 of string

    let create (s: string) =
        match s with
        | null -> fail StringError.Missing
        | _ when s.Length > 128 -> fail (MustNotBeLongerThan 128)
        | _ -> succeed (String128 s)

    let apply f (String128 s) = f s

module String10 =
    type T = String10 of string

    let create (s: string) =
        match s with
        | null -> fail StringError.Missing
        | _ when s.Length > 10 -> fail (MustNotBeLongerThan 10)
        | _ -> succeed (String10 s)

    let apply f (String10 s) = f s
