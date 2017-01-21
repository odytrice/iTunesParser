module ITunesParser.Tests.Parser

open NUnit.Framework
open FsUnit
open ITunesParser.Parser
open ITunesParser.Types
open System.IO
open ITunesParser.Generator

let path = Path.Combine(__SOURCE_DIRECTORY__, "Sample.xml")
let xml = File.OpenText(path).ReadToEnd()


[<Test>]
let ``It Should Parse Tracks`` () = 
    // Arrange

    // Act
    let fetchLibrary = 
        xml
        |> ParsePList
        |> GetLibrary 

    // Assert
    match fetchLibrary with
    | Success lib -> lib.Tracks.Length |> should equal 2
    | Failure e -> failwith e

[<Test>]
let ``It Should Parse PlayLists``() = 
    // Arrange
    
    // Act
    let fetchLibrary = 
        xml
        |> ParsePList
        |> GetLibrary 

    // Assert
    match fetchLibrary with
    | Success lib -> lib.Playlists.Length |> should equal 3
    | Failure e -> failwith e

[<Test>]
let ``It Should Parse PlayLists With Track Info``() = 
    // Arrange
    
    // Act
    let fetchLibrary = 
        xml
        |> ParsePList
        |> GetLibrary 

    // Assert
    match fetchLibrary with
    | Success lib -> 
        lib.Playlists
        |> Seq.collect (fun p -> p.Items)
        |> Seq.length |> should be (greaterThanOrEqualTo 1)
    | Failure e -> failwith e