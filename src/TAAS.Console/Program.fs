// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open System
open TAAS.Contract
open Types
open Commands
open Events

open EventStore

//open EventStore.DummyEventStore
open TAAS.EventStore.EventStore

open TAAS.Domain
open DomainBuilder
open CommandHandling

open Newtonsoft.Json

module Test = 
    let doStuff = 
        let es = connect()
//        let application = createApplication
//
//        let application = createApplication readStream appendStream

        let id = Guid.NewGuid()
        let personId = AccountId(id)
        let c1 = AccountCommand(CreateAccount(personId, "jansson"))
        c1
//        let result = application c1
        
//        printfn "%A" result

//        let (version, (events: Event list)) = readFromStream es (toStreamId id)

//        let serialized = JsonConvert.SerializeObject(events)
//        printfn "serialized: %s" serialized

//        let deserialized = JsonConvert.DeserializeObject<Event list>(serialized)
//        printfn "deserialized: %A" deserialized
////        printfn "deserialized: %O" deserialized
//
//        let testId = sprintf "test-%O" id
//        appendToStream es testId -1 events |> ignore
//        let (version, (readEvents: Event list)) = readFromStream es "test"
//        printf "Read: %A, Version: %d" readEvents version

open Test
[<EntryPoint>]
let main argv = 
    doStuff
    let s = Console.ReadLine()
    0 // return an integer exit code
