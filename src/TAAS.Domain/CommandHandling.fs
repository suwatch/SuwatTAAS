module TAAS.Domain.CommandHandling

open TAAS.Contract
open Commands
open Types

open Account
open User

type Dependencies = {Hasher: (Password -> PasswordHash)}

let handle deps c state =
    match c with
    | Command.AccountCommand(ac) -> handleAccount state ac
    | Command.UserCommand(uc) -> handleUser deps.Hasher state uc
