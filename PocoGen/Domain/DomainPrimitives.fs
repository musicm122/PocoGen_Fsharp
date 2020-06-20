module PocoGen.DomainPrimitives

type LogMessage =
    | Debug of string
    | Info of string
    | Error of string

type StringError =
    | Missing
    | MustNotBeLongerThan of int
    | DoesntMatchPattern of string