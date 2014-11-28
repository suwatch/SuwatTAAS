module TAAS.Tests.Specification

open Xunit
open FsUnit.Xunit

open TAAS.Contract
open Events
open Commands
open Types

open TAAS.Domain
open EventHandling
open CommandHandling

type TestSpec = {PreCondition: (Event list * Dependencies option); Action: Command; PostCondition: Event list}

let defaultDependencies = {
    Hasher = (fun (Password x) -> PasswordHash x) 
}

let Given (events: Event list, dependencies: Dependencies option) = events, dependencies
let When (command: Command) (events: Event list, dependencies: Dependencies option) = events, dependencies, command
let Expect (expectedEvents:Event list) (events:Event list, dependencies: Dependencies option, command: Command) = 
    let deps = match dependencies with
                | Some x -> x
                | None -> defaultDependencies

    printfn "Given: %A" events
    printfn "When: %A" command
    printfn "Expects: %A" expectedEvents
    evolve command events |> handle deps command |> should equal expectedEvents
