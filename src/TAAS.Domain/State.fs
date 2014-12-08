module State

open TAAS.Contract
open Types
open Events
open System

type Dependencies = {Hasher: (Password -> PasswordHash); ReadEvents: Guid -> (int*Event list)}

type AccountState = {Id:AccountId; AccountName:AccountName} 
let initAccount = {Id = AccountId(Guid.Empty); AccountName = ""}

type UserState = {Id:UserId}
let initUser = {Id = UserId(Guid.Empty)}

let defaultVersion = -1

type State =
    | Init
    | Account of AccountState
    | User of UserState

let evolve evolveOne initState events =
    List.fold (fun (v,s) e -> (v + 1, (evolveOne s e))) (defaultVersion,initState) events