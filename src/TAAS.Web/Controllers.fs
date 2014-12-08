namespace TAAS.Web
open Owin

open System
open System.Web.Http
open System.Net.Http
open System.Net

open TAAS.Contract
open Commands
open Events
open Types

open TAAS.Infrastructure
open Railroad
open EventStore
open EventStore.EventStore

open TAAS.Domain
open State
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

module Controllers = 
    open WebStart

    [<RoutePrefix("api")>]
    type HomeController() =
        inherit ApiController()
        [<Route>]
        member this.Get() = 
            this.Request.CreateResponse(
                HttpStatusCode.OK,
                (CreateAccount(AccountId(Guid.NewGuid()), "Hello world")))

    exception InvalidCommand
    [<RoutePrefix("api/account")>]
    type AccountController() = 
        inherit ApiController()
        [<Route>]
        member this.Post (commandDto:CreateAccountDto) = 
            commandDto |> app this

    [<RoutePrefix("api/account/user")>]
    type UserController() =
        inherit ApiController()
        [<Route>] 
        member this.Post (commandDto:AddUserToAccountDto) = 
            commandDto |> app this

    [<RoutePrefix("api/events/{id}")>]
    type EventsController() = 
        inherit ApiController()
        [<Route>]
        member this.Get(id:Guid) =
            let streamId = toStreamId id
            let events = readFromStream es streamId
            this.Request.CreateResponse(HttpStatusCode.OK, events)