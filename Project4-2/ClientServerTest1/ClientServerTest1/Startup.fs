namespace ClientServerTest1

open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open WebSharper.AspNetCore   
open WebSharper.AspNetCore.WebSocket


type Startup() =

    member this.ConfigureServices(services: IServiceCollection) =
        services.AddSitelet<Website.MyWebsite>()
                .AddAuthentication("WebSharper")
                .AddCookie("WebSharper", fun options -> ())
        |> ignore

    member this.Configure(app: IApplicationBuilder, env: IWebHostEnvironment, cfg: IConfiguration) =
        if env.IsDevelopment() then app.UseDeveloperExceptionPage() |> ignore

        app.UseAuthentication()
            .UseWebSockets()
            .UseWebSharper(fun ws ->
                ws.UseWebSocket("ws", fun wsws -> 
                    wsws.Use(WebSocketServer.Start())
                        .JsonEncoding(JsonEncoding.Readable)
                    |> ignore
                )
                |> ignore
            )
            .UseStaticFiles()
            .Run(fun context ->
                context.Response.StatusCode <- 404
                context.Response.WriteAsync("Fell through :("))

module Program =
    let BuildWebHost args =
        WebHost
            .CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .Build()

    [<EntryPoint>]
    let main args =
        BuildWebHost(args).Run()
        0
