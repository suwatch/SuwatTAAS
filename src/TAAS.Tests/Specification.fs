﻿module TAAS.Tests.Specification
open System
open Xunit
open FsUnit.Xunit

open TAAS.Contract
open Events
open Commands
open Types

open TAAS.Domain
open CommandHandling
open State

open TAAS.Infrastructure.EventStore.DummyEventStore
open TAAS.Infrastructure.ApplicationBuilder
open TAAS.Infrastructure.Railroad

let defaultDependencies = {
    Hasher = (fun (Password x) -> PasswordHash x); 
    ReadEvents = (fun x -> 0,[])
}

let createTestApplication dependencies events = 
    let es = create()
    let toStreamId (id:Guid) = sprintf "%O" id
    let readStream id = readFromStream es (toStreamId id)
    events |> List.map (fun (id, evts) -> appendToStream es (toStreamId id) -1 evts) |> ignore
    let deps = match dependencies with
        | Some d -> {d with ReadEvents = readStream}
        | None -> {defaultDependencies with ReadEvents = readStream}

    let save res = Success res
    let handler = handle deps
    buildApplication save handler

let Given (events, dependencies) = events, dependencies
let When command (events, dependencies) = events, dependencies, command
let Expect expectedEvents (events, dependencies, command) = 
    printfn "Given: %A" events
    printfn "When: %A" command
    printfn "Expects: %A" expectedEvents
    command 
    |> (createTestApplication dependencies events) 
    |> (fun (Success (id, version, events)) -> events)
    |> should equal expectedEvents

let ExpectFail failure (events, dependencies, command) =
    printfn "Given: %A" events
    printfn "When: %A" command
    printfn "Should fail with: %A" failure

    command 
    |> (createTestApplication dependencies events) 
    |> (fun r -> r = Failure failure)
    |> should equal true
