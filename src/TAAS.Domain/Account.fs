module TAAS.Domain.Account

open System
open TAAS.Contract
open Types
open Commands
open Events
open State

let handleAccount state ac =
    match ac with
    | CreateAccount(accountId, accountName) -> [AccountCreated(accountId, accountName)]

let evolveAccount p event =
    match event with
    | AccountCreated(id, name) -> State.Account({Id = id; AccountName = name} )
