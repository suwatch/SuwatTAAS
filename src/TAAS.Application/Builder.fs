module TAAS.Application.Builder

open EventStore.ClientAPI

open TAAS.Contract
open Types
open Commands

open TAAS.Domain
open State
open CommandHandling
open EventHandling

open TAAS.Infrastructure
open ApplicationBuilder
open Railroad
open EventStore
open EventStore.EventStore

let createApplication = 
    let es = connect()
    let appendStream = appendToStream es
    let readStream = readFromStream es

    let dependencies = {
        Hasher = (fun (Password x) -> PasswordHash x);
        ReadEvents = (fun id -> readStream (toStreamId id))
    }

    let handler = handle dependencies
    let save (id, version, events) = appendStream (toStreamId id) version events

    buildApplication save handler

//
//let toStreamId id = sprintf "Data-%O" id
//
//let createApplication readStream appendStream = 
//    let aggId command = 
//        match command with
//        | AccountCommand(CreateAccount(AccountId id, _)) -> id
//        | UserCommand(AddUserToAccount(UserId id, _, _, _)) -> id
//
////    let loadState command id = 
////        let streamId = toStreamId id
////        let (version, events) = readStream streamId
////        let state = evolve command events
////        (version, state)
//        
//    let save id expectedVersion newEvents =
//        let streamId = toStreamId id
//        appendStream streamId expectedVersion newEvents
//
//    let defaultDependencies = {
//        Hasher = (fun (Password x) -> PasswordHash x);
//        ReadEvents = 
//    }
//
//    fun command ->
//        let aggregateId = aggId command
//        let state = loadState command aggregateId 
//        state ||> fun version state -> (version, handle defaultDependencies command state) ||> save aggregateId 
//    fun command -> command