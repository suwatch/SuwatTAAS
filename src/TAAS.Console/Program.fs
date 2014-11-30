﻿// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open System
open TAAS.Contract
open Types
open Commands
open Events

open TAAS.Infrastructure
//open EventStore.DummyEventStore
open EventStore.EventStore

open TAAS.Domain
open TAAS.Application
open Builder
open Newtonsoft.Json


module Test = 

    let doStuff = 
        let es = connect()
        let appendStream = appendToStream es
        let readStream = readFromStream es
        let application = createApplication readStream appendStream

        let id = Guid.NewGuid()
        let personId = AccountId(id)
        let c1 = AccountCommand(CreateAccount(personId, "jansson"))
        application c1
        
        let (version, (events: Event list)) = readFromStream es (toStreamId id)

        let serialized = JsonConvert.SerializeObject(events)
        printfn "serialized: %s" serialized

        let deserialized = JsonConvert.DeserializeObject<Event list>(serialized)
        printfn "deserialized: %A" deserialized
//        printfn "deserialized: %O" deserialized

        let testId = sprintf "test-%O" id
        appendToStream es testId -1 events |> ignore
        let (version, (readEvents: Event list)) = readFromStream es "test"
        printf "Read: %A, Version: %d" readEvents version

open Test
[<EntryPoint>]
let main argv = 
    doStuff
    let s = Console.ReadLine()
    0 // return an integer exit code
