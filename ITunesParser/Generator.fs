module ITunesParser.Generator

open System.Collections.Generic
open ITunesParser.Types

let getString key (dict : IDictionary<string, Value>) = 
    match dict.TryGetValue(key) with
    | (true, String s) -> s
    | _ -> ""

let getInt key (dict : IDictionary<string, Value>) = 
    match dict.TryGetValue(key) with
    | (true, Integer i) -> i
    | _ -> 0L

let fetchTracks value = 
    let extract (values : seq<Value>) = 
        seq { 
            for value in values do
                match value with
                | Dict dict -> 
                    yield { TrackID = dict |> getInt "Track ID"
                            Name = dict |> getString "Name"
                            Artist = dict |> getString "Artist"
                            Album = dict |> getString "Album"
                            AlbumArtist = dict |> getString "Album Artist"
                            Composer = dict |> getString "Composer"
                            Location = dict |> getString "Location"
                            Year = dict |> getInt "Year"
                            Genre = dict |> getString "Genre" }
                | _ -> ()
        }
    match value with
    | Dict dict -> dict.Values |> extract
    | _ -> Seq.empty

let fetchPlayLists (tracks : seq<Track>) (playlists : seq<Value>) = 
    let getTracks key (dict : IDictionary<string, Value>) = 
        match dict.TryGetValue(key) with
        | (true, Array trackValues) -> 
            seq { 
                for trackValue in trackValues do
                    match trackValue with
                    | Dict playIdDict -> 
                        let id = playIdDict |> getInt "Track ID"
                        yield tracks
                              |> Seq.filter (fun t -> t.TrackID = id)
                              |> Seq.tryHead
                    | _ -> ()
            }
        | _ -> Seq.empty
    seq { 
        for playlist in playlists do
            match playlist with
            | Dict dict -> 
                yield { PlayListID = dict |> getInt "Playlist ID"
                        Items = 
                            dict
                            |> getTracks "Playlist Items"
                            |> Seq.filter (function 
                                   | Some i -> true
                                   | None -> false)
                            |> Seq.map (fun o -> o.Value)
                            |> Seq.toArray
                        Name = dict |> getString "Name"
                        Description = dict |> getString "Description" }
            | _ -> ()
    }

let GetLibrary library = 
    try 
        match library with
        | Dict dict -> 
            let tracks = dict.["Tracks"] |> fetchTracks
            
            let playLists = 
                match dict.["Playlists"] with
                | Array playlists -> playlists |> fetchPlayLists tracks
                | _ -> Seq.empty

            Success { Playlists = playLists |> List.ofSeq
                      Tracks = tracks |> List.ofSeq
                      MusicFolder = dict |> getString "Music Folder" }

        | _ -> Failure "Library is not a Dictionary"
    with ex -> Failure ex.Message
