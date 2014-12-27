namespace TAAS.Domain

module DomainBuilder = 
    open System
    open TAAS.Contract.Events
    open TAAS.Contract.Types
    open TAAS.Contract.Commands
    open Railway
    open CommandHandling


    let validateCommand c = Success c
//        function
//        | Command.(CheckoutBasket(id, addr)) -> 
//            match addr.Street.Trim() with
//            | "" -> Failure (ValidationError "Invalid address")
//            | trimmed -> Success (BasketCommand(CheckoutBasket(id, {addr with Street = trimmed})))
//        | c -> Success c

    let buildDomainEntry save deps c = 
        (validateCommand c) >>= (handle deps) >>= save
