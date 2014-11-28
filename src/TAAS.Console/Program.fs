// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open System
open TAAS.Contract
open Types
open Commands
open Events

open TAAS.Infrastructure
open EventStore.DummyEventStore

open TAAS.Domain
open TAAS.Application
open Builder
open Newtonsoft.Json


module Test = 

    let doStuff = 
        let es = create()
        let appendStream = appendToStream es
        let readStream = readFromStream es
        let application = createApplication readStream appendStream

        let fnr = fødselsnummer "08080812345" |> Option.get
        let id = Guid.NewGuid()
        let personId = AccountId(id)
        let adresse = {Linjer = "hello"}
        let c1 = AccountCommand(CreateAccount(personId, "jansson"))
        application c1
        
        let (version, events) = readFromStream es (toStreamId id)

        let serialized = JsonConvert.SerializeObject(events)
        printfn "serialized: %s" serialized

        let deserialized = JsonConvert.DeserializeObject<Event list>(serialized)
        printfn "deserialized: %A" deserialized
        printfn "deserialized: %O" deserialized

        let es = create()
        appendToStream es "test" -1 events
        let (version, readEvents) = readFromStream es "test"
        printf "Read: %A, Version: %d" readEvents version

open Test
[<EntryPoint>]
let main argv = 
    doStuff
    let s = Console.ReadLine()
    0 // return an integer exit code
