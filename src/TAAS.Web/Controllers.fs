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
open EventStore.DummyEventStore

open TAAS.Domain
open CommandHandling
open EventHandling

open TAAS.Application.Builder

module WebStart =
    let es = create()
 
    let app command = 
        let appendStream = appendToStream es
        let readStream = readFromStream es
        let app = createApplication readStream appendStream
        app command

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
        member this.Post (command:AccountCommand) = 
            match command with
            | CreateAccount(_) -> app (AccountCommand(command)) |> ignore
            | _ -> raise InvalidCommand
            this.Request.CreateResponse(HttpStatusCode.OK, command)

    [<RoutePrefix("api/events/{id}")>]
    type EventsController() = 
        inherit ApiController()
        [<Route>]
        member this.Get(id:Guid) =
            let streamId = toStreamId id
            let events = readFromStream es streamId
            this.Request.CreateResponse(HttpStatusCode.OK, events)