module TAAS.Domain.User

open TAAS.Contract.Commands
open TAAS.Contract.Events
open TAAS.Contract.Types

open Account
open Railway
open Helpers

type User = 
    | Init
    | Active of UserId*UserName

let evolveOneUser state event =
    match event with
    | UserAddedToAccount(id, name, _, _) -> Success (Active(id,name))

let evolveUser = evolve evolveOneUser

let getUserState dependencies id = evolveUser Init ((dependencies.readEvents id) |> (fun (_, e) -> e))

let handleUser dependencies (uc:UserCommand) =
    let addUserToAccount id userName password accountId (version, state) =
        match state with
        | Init -> 
            match getAccountState dependencies accountId with
            | Success (_,Account.Active(_, _)) ->
                Success (id, version, [UserAddedToAccount(UserId id, userName, (dependencies.hasher password), (AccountId accountId))])
            | _ -> Failure (UserAccountIsMissing accountId)
        | _ -> Failure (AccountAlreadyExist)

    match uc with
    | AddUserToAccount(UserId id, userName, password, (AccountId accountId)) -> 
        getUserState dependencies id >>= addUserToAccount id userName password accountId
//
//        let version, state = getUserState dependencies id
//        let accountVersion, _ = getAccountState dependencies accountId
//        if accountVersion = defaultVersion then Failure (UserAccountIsMissing accountId)
//        else Success (id, version, [UserAddedToAccount(UserId id, userName, (dependencies.hasher password), (AccountId accountId))])
