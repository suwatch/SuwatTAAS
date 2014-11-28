namespace TAAS.Web
open Owin

open System
open System.Web.Http
open System.Net.Http
open System.Net

open TAAS.Contract
open Commands
open Types

open TAAS.Infrastructure
open EventStore.DummyEventStore

open TAAS.Application.Builder

module WebStart = 
    let es = create()
    let appendStream = appendToStream es
    let readStream = readFromStream es

    let app = createApplication readStream appendStream

type OwinAppSetup() =
    member this.Configuration (app:IAppBuilder) = 
        let config = 
            let config = new HttpConfiguration()
            config.MapHttpAttributeRoutes()
            config
        app.UseWebApi config |> ignore
        app.Run(fun c -> c.Response.WriteAsync("Hello TAAS! Why haven't you handled this?"))

module controllers = 
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
    //        let es = create()
    //        let appendStream = appendToStream es
    //        let readStream = readFromStream es
    //
    //        let app = createApplication readStream appendStream
    //        let app c = this.Request.CreateResponse(HttpStatusCode.OK, c)
            match command with
            | CreateAccount(_) -> app (AccountCommand(command))
            | _ -> raise InvalidCommand
            this.Request.CreateResponse(HttpStatusCode.OK, command)