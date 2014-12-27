namespace TAAS.Web

open TAAS.EventStore.EventStore
open System.Web.Http
open System.Net.Http
open System.Net
open TAAS.Domain
open Railway
open TAAS.Infrastructure.Security
open TAAS.Contract.Types
open Dtos

module WebStart =
    open System
    open TAAS.Domain.DomainBuilder

    let toStreamId (id:Guid) = sprintf "TAAS-%O" id
    let es = connect()

    let map f = 
        match f with 
        | AccountAlreadyExist -> "Account already exists"
        | WrongExpectedVersion -> "Version error"
        | UnmatchedDto s -> sprintf "Failed to match %s" s

    let matchToResult (controller:'T when 'T :> ApiController) res =        
        match res with
        | Success events -> controller.Request.CreateResponse(HttpStatusCode.Accepted, events)
        | Failure f -> controller.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, (map f))

//    let app controller dto =
//        dto |> toCommand >>= createApplication |> (matchToResult controller)


    let app controller dto =
        let appendStream = appendToStream es
        let readStream id = readFromStream es (toStreamId id)
        let save (id, version, events) = 
            appendStream (toStreamId id) version events |> ignore
            Success events
        let passwordToString (Password s) = s
        let toPasswordHash s = PasswordHash s
        let deps = {readEvents = readStream; hasher = passwordToString >> hasher >> toPasswordHash}

        dto |> toCommand >>= (buildDomainEntry save deps) |> (matchToResult controller)

//            let passwordToString (Password s) = s
//    let toPasswordHash s = PasswordHash s
//    let dependencies = {
//        Hasher = passwordToString >> hasher >> toPasswordHash
//        ReadEvents = (fun id -> readStream (toStreamId id))
//    }


