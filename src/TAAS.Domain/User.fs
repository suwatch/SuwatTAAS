module TAAS.Domain.User

open Account

open TAAS.Contract
open Commands
open Events
open State
open Types

open TAAS.Infrastructure.Railroad

let evolveOneUser state event =
    match event with
    | UserAddedToAccount(id, _, _, _) -> {Id = id}

let evolveUser = evolve evolveOneUser

let getUserState dependencies id = evolveUser initUser ((dependencies.ReadEvents id) |> (fun (_, e) -> e))

let handleUser dependencies (uc:UserCommand) =
    match uc with
    | AddUserToAccount(UserId id, userName, password, (AccountId accountId)) -> 
        let version, state = getUserState dependencies id
        let accountVersion, _ = getAccountState dependencies accountId
        if accountVersion = defaultVersion then Failure (UserAccountIsMissing accountId)
        else Success (id, version, [UserAddedToAccount(UserId id, userName, (dependencies.Hasher password), (AccountId accountId))])
