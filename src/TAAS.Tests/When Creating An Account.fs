module TAAS.Tests.``When Creating An Account``
open System
open Xunit
open FsUnit.Xunit

open TAAS.Contract
open Events
open Commands
open Types

open TAAS.Domain
open EventHandling
open CommandHandling
open State

open Specification

[<Fact>]
let ``account should create account``() =
    let id = Guid.NewGuid()
    Given ([], None)
    |> When (AccountCommand(CreateAccount(AccountId(id), "AccountName")))
    |> Expect [AccountCreated(AccountId(id), "AccountName")]
