﻿module TAAS.Infrastructure.Railroad
open System

type Error = 
    | InvalidState of string
    | NotSupportedCommand of string
    | AccountAlreadyExist
    | WrongExpectedVersion
    | UserAccountIsMissing of Guid
    | UnmatchedDto of string

type Result<'T> =
    | Success of 'T
    | Failure of Error

let bind switchFunction = 
    fun input -> match input with
                    | Success s -> switchFunction s
                    | Failure s -> Failure s

let (>>=) input switchFunction = bind switchFunction input
