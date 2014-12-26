namespace TAAS.Web
open Owin
open Newtonsoft.Json

open System
open System.Web.Http
open System.Net.Http
open System.Net

open FSharp.Configuration

open TAAS.Contract
open Commands
open Types

open TAAS.Infrastructure
open EventStore.DummyEventStore

open TAAS.Application.Builder
open Microsoft.Owin.Security.ActiveDirectory
open Microsoft.Owin.Security.OpenIdConnect
open Microsoft.Owin.Security.Cookies
open System.Configuration
open System.Collections.Generic
open System.Threading.Tasks

module OwinStart = 
    open Microsoft.Owin
    open Microsoft.Owin.Security

    type Settings = AppSettings<"../TAAS.WebBootstrap/Web.config">

    type AppFunc = Func<IDictionary<string, obj>, Task>
    
    type AuthenticationMiddleware(next: AppFunc) =
        inherit OwinMiddleware(null)
        let isAuthenticated (context:IOwinContext) =
            let user = context.Request.User
            user <> null && user.Identity <> null && user.Identity.IsAuthenticated

        let challenge (authentication:IAuthenticationManager) =
            let authenticationProperties = 
                let authenticationProperties = new AuthenticationProperties()
                authenticationProperties.RedirectUri <- "/"
                authenticationProperties
            authentication.Challenge(authenticationProperties, OpenIdConnectAuthenticationDefaults.AuthenticationType);

        let signout (authentication:IAuthenticationManager) =
            authentication.SignOut(OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType)

        let runAsTask (action) =
            let asyncBlock = 
                async {
                    action
                }
            asyncBlock |> Async.StartAsTask :> Task

        default x.Invoke(context: IOwinContext) =
            match context.Request.Path.Value = "/signout" with
            | true -> runAsTask (signout context.Authentication)
            | _ -> 
                match isAuthenticated context with
                | true -> next.Invoke(context.Environment)
                | false -> runAsTask (challenge context.Authentication)

    type OwinAppSetup() =
        let useWebApi (app:IAppBuilder) =
            let config = 
                let config = new HttpConfiguration()
                config.MapHttpAttributeRoutes()
                config.Formatters.JsonFormatter.SerializerSettings <- new JsonSerializerSettings()
                config.Formatters.JsonFormatter.SerializerSettings.ConstructorHandling <- ConstructorHandling.AllowNonPublicDefaultConstructor
                config
            app.UseWebApi config |> ignore
            app

        let adSetup (app:IAppBuilder) =
            let windowsAzureActiveDirectoryBearerAuthenticationExtensions = 
                let waadbae = new WindowsAzureActiveDirectoryBearerAuthenticationOptions()
                waadbae.Tenant <- ConfigurationManager.AppSettings.["Tenant"]
                waadbae.Audience <- ConfigurationManager.AppSettings.["Audience"]
                waadbae
            
            app.UseWindowsAzureActiveDirectoryBearerAuthentication(windowsAzureActiveDirectoryBearerAuthenticationExtensions) |> ignore
            app
        
        let defaultSetup (app:IAppBuilder) = 
            app.Run(fun c -> c.Response.WriteAsync("Hello TAAS! Why haven't you handled this?"))
            app

        let authenticationSetup (app:IAppBuilder) = 
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType)
            app.UseCookieAuthentication(new CookieAuthenticationOptions()) |> ignore

            let openIdConnectAuthenticationOptions = 
                let authority = sprintf "%s%s" (Settings.IdaAadiNstance.ToString()) (Settings.IdaTenant)
                let openIdConnectAuthenticationOptions = new OpenIdConnectAuthenticationOptions()
                openIdConnectAuthenticationOptions.ClientId <- Settings.IdaClientId
                openIdConnectAuthenticationOptions.Authority <- authority
                openIdConnectAuthenticationOptions.PostLogoutRedirectUri <- Settings.IdaPostLogoutRedirectUri.ToString()
                openIdConnectAuthenticationOptions

            app.UseOpenIdConnectAuthentication(openIdConnectAuthenticationOptions) |> ignore

            app.Use(typeof<AuthenticationMiddleware>)

        member this.Configuration (app:IAppBuilder) = 
            app
            |> authenticationSetup
            |> useWebApi
            |> adSetup
            |> defaultSetup
            |> ignore
