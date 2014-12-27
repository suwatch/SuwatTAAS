module TAAS.Web.Dtos
open System
open TAAS.Contract
open Commands
open Types

open TAAS.Domain.Railway

type CreateAccountDto = {AccountName: string}
type AddUserToAccountDto = { UserName: string; Password: string; AccountId: Guid}


let guid = (Guid.NewGuid())

let toCommand (dto: obj) =
    match dto with 
    | :? CreateAccountDto as caDto -> Success (AccountCommand(CreateAccount(AccountId (guid), caDto.AccountName)))
    | :? AddUserToAccountDto as autaDto -> Success (UserCommand(AddUserToAccount(UserId (guid), autaDto.UserName, Password autaDto.Password, AccountId (autaDto.AccountId))))
    | _ -> Failure (UnmatchedDto (dto.GetType().Name))

