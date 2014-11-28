module TAAS.Domain.User

open TAAS.Contract
open Commands
open Events

let handleUser hasher state (uc:UserCommand) =
    match uc with
    | AddUserToAccount(userId, userName, password, accountId) -> 
        [UserAddedToAccount(userId, userName, (hasher password), accountId)]

let evolveUser p event =
    match event with
    | UserAddedToAccount(id, _, _, _) -> State.User({Id = id} )

