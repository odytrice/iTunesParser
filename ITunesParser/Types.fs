module ITunesParser.Types

open System
open System.Collections.Generic

type Value = 
    | Integer of int64
    | String of string
    | Date of DateTime
    | Data of string
    | Bool of bool
    | Dict of IDictionary<string, Value>
    | Array of list<Value>