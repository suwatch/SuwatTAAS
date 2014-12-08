﻿namespace TAAS.Web
open Owin
open Newtonsoft.Json

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

type OwinAppSetup() =
    member this.Configuration (app:IAppBuilder) = 
        let config = 
            let config = new HttpConfiguration()
            config.MapHttpAttributeRoutes()
            config.Formatters.JsonFormatter.SerializerSettings <- new JsonSerializerSettings()
            config.Formatters.JsonFormatter.SerializerSettings.ConstructorHandling <- ConstructorHandling.AllowNonPublicDefaultConstructor
            config
        app.UseWebApi config |> ignore
        app.Run(fun c -> c.Response.WriteAsync("Hello TAAS! Why haven't you handled this?"))
