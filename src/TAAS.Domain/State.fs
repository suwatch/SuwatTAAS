module State

open TAAS.Contract
open Types
open System

type AccountState = {Id:AccountId; AccountName:AccountName} 
let initAccount = {Id = AccountId(Guid.Empty); AccountName = ""}

type UserState = {Id:UserId}
let initUser = {Id = UserId(Guid.Empty)}

type State =
    | Init
    | Account of AccountState
    | User of UserState
