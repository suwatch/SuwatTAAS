module TAAS.Domain.Account

open System
open TAAS.Contract
open Types
open Commands
open Events
open State

exception AccountAlreadyExistsException
exception InvalidStateException

let handleAccount state ac =
    let accountState = match state with
                        | Account s -> s
                        |_ -> raise InvalidStateException
    match ac with
    | CreateAccount(accountId, accountName) -> 
        if accountState <> initAccount then raise AccountAlreadyExistsException
        [AccountCreated(accountId, accountName)]

let evolveAccount p event =
    match event with
    | AccountCreated(id, name) -> State.Account({Id = id; AccountName = name} )
