module TAAS.Tests.Specification
open System
open Xunit
open FsUnit.Xunit

open TAAS.Contract.Events
open TAAS.Contract.Commands
open TAAS.Contract.Types

open TAAS.Domain
open TAAS.Domain.CommandHandling
open TAAS.Domain.DomainBuilder
open TAAS.Domain.Railway

open TAAS.EventStore.DummyEventStore

let (defaultDependencies) = {
    readEvents = (fun x -> 0,[]);
    hasher = (fun (Password x) -> PasswordHash x)
}

let createTestApplication dependencies events = 
    let es = create()
    let toStreamId (id:Guid) = sprintf "%O" id
    let readStream id = readFromStream es (toStreamId id)
    events |> List.map (fun (id, evts) -> appendToStream es (toStreamId id) -1 evts) |> ignore
    let deps = match dependencies with
        | Some d -> {d with readEvents = readStream}
        | None -> {defaultDependencies with readEvents = readStream}

    let save res = Success res
    buildDomainEntry save deps

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
