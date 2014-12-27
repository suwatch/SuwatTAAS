module TAAS.Domain.Account

open System
open TAAS.Contract
open Types
open Commands
open Events
open Railway
open Helpers

type Account = 
    | Init
    | Active of AccountId*AccountName

let evolveOneAccount state event =
    match event with
    | AccountCreated(id, name) -> Success (Active(id, name))
    | _ -> Failure (InvalidStateTransition (sprintf "%s, %s" (state.GetType().Name) (event.GetType().Name)))

let evolveAccount = evolve evolveOneAccount

let getAccountState dependencies id = evolveAccount Init ((dependencies.readEvents id) |> (fun (_, e) -> e))

let handleAccount dependencies ac =
    let createAccount id accountName (version, state) =
        match state with
        | Init -> Success (id, version, [AccountCreated(AccountId id, accountName)])
        | _ -> Failure AccountAlreadyExist

    match ac with
    | CreateAccount(AccountId id, accountName) -> 
        getAccountState dependencies id >>= createAccount id accountName

