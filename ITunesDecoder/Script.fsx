// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
#load "Scripts/load-references-debug.fsx"
#load "Types.fs"

open System.IO
open ITunesDecoder.Types
open System.Xml.Linq
open System

let path = Path.Combine( __SOURCE_DIRECTORY__, "library.xml")


let loadDocument path =
    use xml = File.OpenText(path);
    let doc = XDocument.Load(xml);
    doc
    
let getRoot(doc: XDocument) =
    doc.Elements()
    |> Seq.head

let root = 
    path
    |> loadDocument
    |> getRoot


root.Elements()
|> Seq.iter (fun e ->  e.Name.LocalName |> (printfn "%A"))

let parseElement (element1: XElement, element2: XElement) =
    match element1.Name.LocalName with
    | "integer" -> Integer (int(element2.Value))
    | "string" -> PList.String element2.Value
    | "date" -> PList.Date (element2.Value |> DateTime.Parse)
    | "data" -> PList.Data element2.Value
    | _ -> PList.None

let parseDict (element:XElement) =
    let values = dict["blue", Integer 40; "red", Integer 700]
    Dict(values)

let parseArray (element: XElement) =
    PList.Array [ Integer 10 ]



let rec parse(element: XElement) (rest: PList list) =
    let children = element.Elements() |> List.ofSeq
    match children with
    | x :: xs when x.Name.LocalName = "dict" -> parseDict x
    | x :: xs when x.Name.LocalName = "array" -> parseArray x
    | x :: y :: rest -> parseElement ( x, y)
    | [_] | [] -> PList.None