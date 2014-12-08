module TAAS.Infrastructure.ApplicationBuilder
open Railroad

let toStreamId id = sprintf "TICKETING-%O" id

let bind switchFunction = 
    fun input -> match input with
                    | Success s -> switchFunction s
                    | Failure s -> Failure s

let (>>=) input switchFunction = bind switchFunction input

let buildApplication save handler c = 
    (handler c) >>= save

