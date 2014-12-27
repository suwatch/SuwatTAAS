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

open TAAS.EventStore.EventStore
open TAAS.Domain

open Dtos

module ApiControllers = 
    open WebStart

    [<RoutePrefix("api")>]
    [<Authorize>]
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