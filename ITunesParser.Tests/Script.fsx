// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
#load "Scripts/load-references-debug.fsx"

open System.IO
open ITunesParser.Parser

// string
let path = Path.Combine(__SOURCE_DIRECTORY__, "Library.xml")
let xml = File.OpenText(path).ReadToEnd()
let elements = parsePListXML xml