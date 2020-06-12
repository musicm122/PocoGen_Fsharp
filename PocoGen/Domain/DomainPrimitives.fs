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