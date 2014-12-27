namespace TAAS.Domain
open System
open TAAS.Contract.Events
open TAAS.Contract.Types

type Dependencies = {readEvents: Guid -> (int*Event list); hasher: (Password -> PasswordHash)}

module Railway = 
    type Error = 
        | InvalidState of string
        | InvalidStateTransition of string
        | NotSupportedCommand of string
        | UnmatchedDto of string
        | ValidationError of string
        | AccountAlreadyExist
        | UserAccountIsMissing of Guid
        | WrongExpectedVersion

    type Result<'T> =
        | Success of 'T
        | Failure of Error

    let bind switchFunction = function
        | Success s -> switchFunction s
        | Failure f -> Failure f
   
    let (>>=) input switchFunction = bind switchFunction input

