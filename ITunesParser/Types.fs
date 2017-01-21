namespace ITunesParser.Types

open System
open System.Collections.Generic

type Result<'TSuccess, 'TFailure> = 
    | Success of 'TSuccess
    | Failure of 'TFailure

type Value = 
    | Integer of int64
    | String of string
    | Date of DateTime
    | Data of string
    | Bool of bool
    | Dict of IDictionary<string, Value>
    | Array of list<Value>

type Track = 
    { TrackID : int64
      Name : string
      Artist : string
      AlbumArtist : string
      Album: string
      Composer : string
      Location : string
      Year : int64
      Genre : string }

type PlayList = 
    { PlayListID : int64
      Name : string
      Description : string
      Items : Track [] }

type Library = 
    { MusicFolder : string
      Tracks : Track list
      Playlists : PlayList list }