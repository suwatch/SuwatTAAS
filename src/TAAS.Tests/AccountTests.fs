module TAAS.Tests.AccountTests
open System
open Xunit
open FsUnit.Xunit

open TAAS.Contract
open Events
open Commands
open Types

open TAAS.Domain
open CommandHandling

open Specification

module ``When Creating An Account`` =
    [<Fact>]
    let ``account should create account``() =
        let id = Guid.NewGuid()
        Given ([], None)
        |> When (AccountCommand(CreateAccount(AccountId(id), "AccountName")))
        |> Expect [AccountCreated(AccountId(id), "AccountName")]
