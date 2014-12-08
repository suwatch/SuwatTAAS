module TAAS.Domain.Account

open System
open TAAS.Contract
open Types
open Commands
open Events
open State

open TAAS.Infrastructure.Railroad

let evolveOneAccount state event =
    match event with
    | AccountCreated(id, name) -> {Id = id; AccountName = name}

let evolveAccount = evolve evolveOneAccount

let getAccountState dependencies id = evolveAccount initAccount ((dependencies.ReadEvents id) |> (fun (_, e) -> e))

let handleAccount dependencies ac =
    match ac with
    | CreateAccount(AccountId id, accountName) -> 
        let (version, state) = getAccountState dependencies id
        if state <> initAccount then Failure AccountAlreadyExist
        else Success (id, version, [AccountCreated(AccountId id, accountName)])
