module CsharpFormatting

open System
open System.Text.RegularExpressions
open System.Data
open System.Collections.Generic

type propertyValues = {
    typeName:string;
    typeValue:Type;
    isNullable:bool;
    columnName:string;
    accessor:string;
}

let typeLookup name =
    match name with
    | "int" -> typeof<int>
    | "short" -> typeof<int16>
    | "byte" -> typeof<byte>
    | "byte[]" -> typeof<byte[]>
    | "long" -> typeof<int64>
    | "double" -> typeof<double>
    | "decimal" -> typeof<decimal>
    | "float" -> typeof<float>
    | "bool" -> typeof<bool>
    | "string" -> typeof<string>
    | _ -> typeof<Object>

let getNullableTypes =
    let nullables = new HashSet<Type>()
    nullables.Add(typeof<int>) |> ignore
    nullables.Add(typeof<int16>) |> ignore
    nullables.Add(typeof<double>)|> ignore
    nullables.Add(typeof<decimal>)|> ignore
    nullables.Add(typeof<float>)|> ignore
    nullables.Add(typeof<bool>)|> ignore
    nullables.Add(typeof<DateTime>)|> ignore
    nullables


let formatTypeName(dataType:string):string =
    let replaceSystem(item:string):string=
        item.Replace("System.", String.Empty)

    let capitalizeFirstLetter (item:string):string =
        match item with
        | s when String.IsNullOrWhiteSpace(s) -> String.Empty
        | _ ->
            let first = item.[0].ToString().ToUpper()
            let rest = item.Substring(1)
            first+rest

    let replaceUnderscoreWithCapitalLetters(item:string) :string=
        match item with
        | s when String.IsNullOrWhiteSpace(s) -> item
        | _ ->
            let flags = RegexOptions.Compiled ||| RegexOptions.IgnoreCase
            let rx = new Regex(@"_([\w\d]){1}", flags)
            let replace(rxMatch:Match)=
                rxMatch.Value.Substring(rxMatch.Value.Length - 1).ToUpper()
            match rx.IsMatch(item) with
            | false -> item
            | true -> rx.Replace(item, new MatchEvaluator(replace))
    dataType
        |> replaceSystem
        |> capitalizeFirstLetter
        |> replaceUnderscoreWithCapitalLetters


let buildClass (className:string) (dataRows:DataRow[]) :string =

    let buildProperty(row:DataRow) : propertyValues=
        let typeNameDetails = row.["DataType"] |> string
        let typeVal = typeLookup(typeNameDetails)
        let isNullableVal =row.["AllowDBNull"].ToString() |> bool.Parse
        let isNullable = isNullableVal && getNullableTypes.Contains(typeVal)
        {
            typeName = typeNameDetails;
            typeValue = typeVal;
            isNullable = isNullable;
            columnName = row.["ColumnName"] |> string;
            accessor = "public"
        }

    let classDeclaration className =
        "public class "+ className +  "\r\n" + "{"

    let appendClassTermination classVal =
        sprintf "%s\r\n{" classVal

    let convertToPropertyString propValues =
        let typeValue = if propValues.isNullable then propValues.typeName + "?" else propValues.typeName
        sprintf "%s %s %s  {get;set;}" propValues.accessor typeValue propValues.columnName

    let classBody=
        dataRows
        |> Array.map buildProperty
        |> Array.map convertToPropertyString
        |> Array.fold(+) ""

    let classHeader =
        className
        |>classDeclaration

    classHeader + classBody
        |> appendClassTermination