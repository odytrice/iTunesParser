namespace ITunesDecoder.Types

open System
open System.Collections.Generic

type PList = 
| Integer of int64
| Date of DateTime
| String of String
| Dict of IDictionary<string,PList>
| Array of PList list
| Data of string
| Key of string
| None