module TAAS.Contract.Types
open System

type AccountId = AccountId of Guid
type AccountName = string

type UserId = UserId of Guid
type UserName = string

type Password = Password of string
type PasswordHash = PasswordHash of string
