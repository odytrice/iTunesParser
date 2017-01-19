// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
#load "Scripts/load-references-debug.fsx"

open System.IO
open System.Xml.Linq
open System

// string
let path = Path.Combine(__SOURCE_DIRECTORY__, "Library.xml")

// string -> XDocument
let loadDocument path = 
    use xml = File.OpenText(path)
    let doc = XDocument.Load(xml)
    doc

// XElement
let root = 
    path
    |> loadDocument
    |> fun doc -> doc.Elements() 
    |> Seq.head

type Value = 
    | Integer of int64
    | String of string
    | Date of DateTime
    | Data of string
    | Bool of bool
    | Dict of list<string * Value>
    | Array of list<Value>

// XElement -> Value
let rec toValue (element : XElement) = 
    match element.Name.LocalName with
    | "integer" -> Integer(int64 (element.Value))
    | "string" -> String element.Value
    | "date" -> Date(element.Value |> DateTime.Parse)
    | "data" -> Data element.Value
    | "true" -> Bool true
    | "false" -> Bool false
    | "dict" -> element |> toDict
    | "array" -> element |> toArray
    | s -> failwith ("Unknown Value: " + s)

// XElement -> Value
and toDict dictElement = 
    // (string * Value) list -> XElement list -> (string * Value) list
    let rec processPairs result (elements : XElement list) = 
        match elements with
        | x :: y :: rest -> processPairs ((x.Value, toValue y) :: result) rest
        | [ _ ] | [] -> result

    Dict(processPairs [] (dictElement.Elements() |> List.ofSeq))

// XElement -> value
and toArray arrayElement = 
    // Value list -> XElement list -> Value list
    let rec processSeq result (elements : XElement list) = 
        match elements with
        | x :: rest -> processSeq ((toValue x) :: result) rest
        | [] -> result

    Array(processSeq [] (arrayElement.Elements() |> List.ofSeq))

// Seq<T>
root.Elements() 
|> Seq.map(fun e -> e |> toValue)