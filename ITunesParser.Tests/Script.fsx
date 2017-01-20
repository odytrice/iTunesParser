// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
#load "Scripts/load-references-debug.fsx"

open System.IO
open ITunesParser.Parser
open ITunesParser.Types
open System.Collections.Generic

// string
let path = Path.Combine(__SOURCE_DIRECTORY__, "Library.xml")
let xml = File.OpenText(path).ReadToEnd()
let library = parsePListXML xml
let log value = printfn "%A" value

type Track = 
    { TrackID : int64
      Name : string
      Artist : string
      AlbumArtist : string
      Composer : string
      Location : string
      Year : int64
      Genre : string }

type PlayList = 
    { PlayListID : int64 }

type Library = 
    { MusicFolder : string
      Tracks : Track list
      Playlists : PlayList list }

let getString key (dict : IDictionary<string, Value>) = 
    match dict.TryGetValue(key) with
    | (true, String s) -> s
    | _ -> ""

let getInt key (dict : IDictionary<string, Value>) = 
    match dict.TryGetValue(key) with
    | (true, Integer i) -> i
    | _ -> 0L

let fetchTracks value = 
    let asTrack (dict : IDictionary<string, Value>) = 
        { TrackID = dict |> getInt "Track ID"
          Name = dict |> getString "Name"
          Artist = dict |> getString "Artist"
          AlbumArtist = dict |> getString "Album Artist"
          Composer = dict |> getString "Composer"
          Location = dict |> getString "Location"
          Year = dict |> getInt "Year"
          Genre = dict |> getString "Genre" }
    
    let fetchTracks (values : seq<Value>) = 
        seq { 
            for value in values do
                match value with
                | Dict dict -> yield dict |> asTrack
                | _ -> ()
        }
    
    match value with
    | Dict dict -> dict.Values |> fetchTracks
    | _ -> Seq.empty

let fetchPlayLists value = [ { PlayListID = 1L } ]

let getLibrary library = 
    match library with
    | Dict dict -> 
        let tracks = dict.["Tracks"] |> fetchTracks |> List.ofSeq
        let playLists = dict.["Playlists"] |> fetchPlayLists |> List.ofSeq

        Some { Playlists = playLists
               Tracks = tracks
               MusicFolder = dict |> getString "Music Folder" }
    | _ -> None

getLibrary library
