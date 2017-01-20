module ITunesParser.Tests.Parser

open NUnit.Framework
open FsUnit
open ITunesParser.Parser
open ITunesParser.Types

[<Test>]
let ``It Should Parse Dictionaries``() = 
    //Arrange
    let xml = """<?xml version="1.0" encoding="UTF-8"?>
                 <!DOCTYPE plist PUBLIC "-//Apple Computer//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
                 <plist version="1.0">
                    <dict>
			            <key>Track ID</key><integer>1470</integer>
		            </dict> 
                </plist>"""
    //Act
    let result = parsePListXML xml
    let expected = Value.Dict(dict [ ("Track ID", Integer 1470L) ])
    result |> should equal expected
