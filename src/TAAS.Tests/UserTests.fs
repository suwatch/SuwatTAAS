module TAAS.Tests.UserTests
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

open TAAS.Infrastructure.Railroad

open Specification

module ``When Adding User To An Account`` = 
    [<Fact>]
    let ``user should be added if account exists``() =
        let id = Guid.NewGuid()
        let accountId = Guid.NewGuid()
        let encryptedPassword = "secretStuff"
        let hasher (Password x) = PasswordHash encryptedPassword
        Given ([(accountId, [AccountCreated(AccountId accountId, "")])], Some {defaultDependencies with Hasher = hasher})
        |> When (Command.UserCommand(AddUserToAccount(UserId(id), "Name", Password("Password"), AccountId(accountId))))
        |> Expect [UserAddedToAccount(UserId(id), "Name", (PasswordHash encryptedPassword), AccountId(accountId))]

    [<Fact>]
    let ``should fail if account doesn't exist``() =
        let id = Guid.NewGuid()
        let accountId = Guid.NewGuid()
        let encryptedPassword = "secretStuff"
        let hasher (Password x) = PasswordHash encryptedPassword
        Given ([], Some {defaultDependencies with Hasher = hasher})
        |> When (Command.UserCommand(AddUserToAccount(UserId(id), "Name", Password("Password"), AccountId(accountId))))
        |> ExpectFail (UserAccountIsMissing accountId)
