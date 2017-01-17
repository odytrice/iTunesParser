// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
#load "Scripts/load-references-debug.fsx"
#load "Types.fs"

open System.IO
open System.Xml.Linq
open System

let path = Path.Combine(__SOURCE_DIRECTORY__, "Library.xml")

let loadDocument path = 
    use xml = File.OpenText(path)
    let doc = XDocument.Load(xml)
    doc


let root = 
    path
    |> loadDocument
    |> fun doc -> doc.Elements() |> Seq.head

type Element = 
    | Integer of int64
    | String of string
    | Date of DateTime
    | Data of string
    | Bool of bool
    | Dict of list<string * Element>
    | Array of list<Element>

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

and toDict dictElement = 
    let rec processPairs result (elements : XElement list) = 
        match elements with
        | x :: y :: rest -> processPairs ((x.Value, y |> toValue) :: result) rest
        | [ _ ] | [] -> result
    Dict(processPairs [] (dictElement.Elements() |> List.ofSeq))

and toArray arrayElement = 
    let rec processSeq result (elements : XElement list) = 
        match elements with
        | x :: rest -> processSeq ((x |> toValue) :: result) rest
        | [] -> result
    Array(processSeq [] (arrayElement.Elements() |> List.ofSeq))

root.Elements() 
|> Seq.map(fun e -> e |> toValue)
