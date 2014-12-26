namespace TAAS.Web
open TAAS.Infrastructure
open Railroad
open EventStore
open EventStore.EventStore
open System.Web.Http
open System.Net.Http
open System.Net
open TAAS.Domain
open TAAS.Infrastructure
open Railroad
open Dtos
open TAAS.Application.Builder

module WebStart =
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

    let app controller dto =
        dto |> toCommand >>= createApplication |> (matchToResult controller)


