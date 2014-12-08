module TAAS.Domain.User

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
    | AddUserToAccount(UserId id, userName, password, accountId) -> 
        let version, state = getUserState dependencies id
        Success (id, version, [UserAddedToAccount(UserId id, userName, (dependencies.Hasher password), accountId)])
