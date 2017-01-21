## ITunes Parser for F# &nbsp;

This F# Library has both a PList XML Parser and iTunes XML Parser

## Usage
```fsharp
// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
#load "Scripts/load-references-debug.fsx"

open System.IO
open ITunesParser.Parser
open ITunesParser.Types
open ITunesParser.Generator

// string
let path = Path.Combine(__SOURCE_DIRECTORY__, "Library.xml")
let xml = File.OpenText(path).ReadToEnd()

let library = 
    xml
    |> ParsePList
    |> GetLibrary

let playlists = 
    let exceptions = [| "Library"; "Music"; "Movies"; "TV Shows"; "Podcasts" |]
    match library with
    | Success x -> 
        x.Playlists
        |> Seq.filter (fun p -> exceptions |> Array.contains (p.Name) |> not)
        |> List.ofSeq
    | Failure _ -> []

let printPlayList (playList : PlayList) = 
    printfn "## %s" playList.Name
    printfn "| Title | Artist | Album |"
    printfn "| :---- | :----- | :---- |"
    for track in playList.Items do
        printfn "| %s | %s | %s" track.Name track.Artist track.Album

playlists |> Seq.iter printPlayList
```
