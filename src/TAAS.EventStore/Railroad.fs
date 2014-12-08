module TAAS.Infrastructure.Railroad

type Error = 
    | InvalidState of string
    | NotSupportedCommand of string
    | AccountAlreadyExist

type Result<'T> =
    | Success of 'T
    | Failure of Error
