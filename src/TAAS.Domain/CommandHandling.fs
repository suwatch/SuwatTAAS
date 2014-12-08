module TAAS.Domain.CommandHandling

open TAAS.Contract
open Commands
open Types

open Account
open User

let handle dependencies c =
    match c with
    | Command.AccountCommand(ac) -> handleAccount dependencies ac
    | Command.UserCommand(uc) -> handleUser dependencies uc
