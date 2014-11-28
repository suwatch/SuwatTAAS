module TAAS.Contract.Commands
open Types

type Command = 
    | AccountCommand of AccountCommand
    | UserCommand of UserCommand
and AccountCommand =
    | CreateAccount of AccountId * AccountName 
and UserCommand = 
    | AddUserToAccount of UserId * UserName * Password * AccountId
