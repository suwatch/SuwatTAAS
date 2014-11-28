module TAAS.Application.Builder

open TAAS.Contract
open Types
open Commands

open TAAS.Domain
open CommandHandling
open EventHandling

let toStreamId id = sprintf "Data-%O" id

let createApplication readStream appendStream = 
    let aggId command = 
        match command with
        | AccountCommand(CreateAccount(AccountId id, _)) -> id
        | UserCommand(AddUserToAccount(UserId id, _, _, _)) -> id

    let loadState command id = 
        let streamId = toStreamId id
        let (version, events) = readStream streamId
        let state = evolve command events
        (version, state)
        
    let save id expectedVersion newEvents =
        let streamId = toStreamId id
        appendStream streamId expectedVersion newEvents

    let getPerson (x:string) (y:Fødselsnummer) = {Navn = {Fornavn = "Tomas"; Mellomnavn = None; Etternavn = "Jansson"}; Adresse = {Linjer = "Linje 1"}}

    let defaultDependencies = {
        Hasher = (fun (Password x) -> PasswordHash x) 
    }

    fun command ->
        let aggregateId = aggId command
        let state = loadState command aggregateId 
        state ||> fun version state -> (version, handle defaultDependencies command state) ||> save aggregateId 
