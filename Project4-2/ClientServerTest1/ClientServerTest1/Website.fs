module ClientServerTest1.Website

open Microsoft.Extensions.Logging
open WebSharper
open WebSharper.AspNetCore
open WebSharper.JavaScript
open WebSharper.Sitelets
open WebSharper.UI
open WebSharper.UI.Html
open WebSharper.UI.Templating

type IndexTemplate = Template<"Main.html", clientLoad = ClientLoad.FromDocument>

[<AbstractClass>]
type RpcUserSession() =
    [<Rpc>]
    abstract GetLogin : unit -> Async<option<string>>
    [<Rpc>]
    abstract Login : name: string -> Async<unit>
    [<Rpc>]
    abstract Logout : unit -> Async<unit>

type EndPoint =
    | [<EndPoint "/">] Home
    | [<EndPoint "/about">] About
    | [<EndPoint "POST /post">] Post
    | [<EndPoint "POST /formdata"; FormData "x">] FormData of x: string 

[<JavaScript>]
[<Require(typeof<Resources.BaseResource>, "//maxcdn.bootstrapcdn.com/bootstrap/3.3.5/css/bootstrap.min.css")>]
module Client =
    let Main (aboutPageLink: string) wsep =
        IndexTemplate.Body()
            .WebSocketTest(WebSocketClient.WebSocketTest wsep)
            .Doc()
open WebSharper.UI.Server

type MyWebsite(logger: ILogger<MyWebsite>) =
    inherit SiteletService<EndPoint>()

    override this.Sitelet = Application.MultiPage(fun (ctx: Context<_>) (ep: EndPoint) ->
        let readBody() =
            let i = ctx.Request.Body 
            if not (isNull i) then 
                // We need to copy the stream because else StreamReader would close it.
                use m =
                    if i.CanSeek then
                        new System.IO.MemoryStream(int i.Length)
                    else
                        new System.IO.MemoryStream()
                i.CopyTo m
                if i.CanSeek then
                    i.Seek(0L, System.IO.SeekOrigin.Begin) |> ignore
                m.Seek(0L, System.IO.SeekOrigin.Begin) |> ignore
                use reader = new System.IO.StreamReader(m)
                reader.ReadToEnd()
            else "Request body not found"
        logger.LogInformation("Serving {0}", ep)
        match ep with
        | Home ->
            let aboutPageLink = ctx.Link About
            let wsep = WebSocketClient.MyEndPoint (ctx.RequestUri.ToString())
            printfn "print:%A" (ctx.RequestUri.ToString())
            //call the function: Client.Main
            IndexTemplate()
                .Main(client <@ Client.Main aboutPageLink wsep @>)
                .Doc()
            |> Content.Page
        | About ->
            Content.Text "This is a test project for WebSharper.AspNetCore"
        | FormData i ->
            Content.Text i
        | Post ->
            Content.Text ctx.Request.BodyText
    )
