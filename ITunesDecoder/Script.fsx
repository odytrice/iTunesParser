// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
#load "Scripts/load-references-debug.fsx"
#load "Types.fs"

open System.IO
open ITunesDecoder.Types
open System.Xml.Linq
open System

let path = Path.Combine(__SOURCE_DIRECTORY__, "Sample.xml")

let loadDocument path = 
    use xml = File.OpenText(path)
    let doc = XDocument.Load(xml)
    doc

let getRoot (doc : XDocument) = doc.Elements() |> Seq.head

let parseElement (element : XElement) = 
    match element.Name.LocalName with
    | "integer" -> PList.Integer(int64 (element.Value))
    | "string" -> PList.String element.Value
    | "date" -> PList.Date(element.Value |> DateTime.Parse)
    | "data" -> PList.Data element.Value
    | n -> failwith (sprintf "Unknown key %s" n)


let rec parseDict result (element : XElement) = 
    let list = element.Elements() |> List.ofSeq
    match list with
    | x :: y :: tail when x.Name.LocalName = "key" -> Dict (dict ((x.Value, parseElement (y)) :: result))
    | x :: y :: tail when y.Name.LocalName = "dict" -> parseDict [] (y)
    | [x] -> parseElement x
    | [] -> Dict(dict result)

let parseArray (element : XElement) = PList.Array [ Integer 10L ]

let rec parse (results : PList list) (elements : XElement list)  = 
    match elements with
    | x :: tail when x.Name.LocalName = "dict" -> parse ((parseDict [] x) :: results) tail
    | x :: tail when x.Name.LocalName = "array" -> parse ((parseArray x) :: results) tail
    | x :: tail -> parse((parseElement x) :: results) tail
    | [ _ ] | [] -> results






let root = 
    path
    |> loadDocument
    |> getRoot

root.Elements() |> List.ofSeq |> parse []