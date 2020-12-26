module ClientServerTest1.WebSocketClient
open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html
open WebSharper.AspNetCore.WebSocket
open WebSharper.AspNetCore.WebSocket.Client
open Akka.FSharp
open Akka.Actor
open WebSharper.Forms

module Server = WebSocketServer

//Data type in actors
type UserData = 
    | Initialize of username:string * psw:string
    | Join of tweetcontent:string * sender:string
    | BeSubscribed of username:string
    | Subscribing of username:string
    | SendTweets of tweetcontent:string
    | ReTweets of tweetcontent:string
    | QueryByTag of tag:string
    | QueryByMentioned of mentioned:string
    | QueryByUsername of username:string
    | LogInSuccess
    | LogOutSuccess

let UserActor (mailbox: Actor<_>) = 
    let mutable my_username = ""
    let mutable my_psw = ""
    let mutable status = 0
    let mutable tweet_with_username = Map.empty<string, string list>
    let mutable tweet_with_tag = Map.empty<string, string list>
    let mutable tweet_with_mention = Map.empty<string, string list>
    let mutable tweet_all : string list = []
    let mutable tweet_not_processed : string list = []
    let mutable followings : string list = []
    let mutable followers : string list = []
    let rec loop () = actor {
        let! msg = mailbox.Receive ()
        match msg with
        | Initialize(username,psw)->
            my_username <- username
            my_psw <- psw
            printfn "%A has been created successfully!" my_username
        | Join(tweetcontent, sender)->
            if status = 1 then
                printfn "%A received tweet from %A : %A" my_username sender tweetcontent
                tweet_all <- tweetcontent :: tweet_all
                if tweet_with_username.ContainsKey(sender) then
                    let mutable tweet = tweet_with_username.[sender]
                    tweet <- tweetcontent :: tweet
                    tweet_with_username <- tweet_with_username.Remove(sender)
                    tweet_with_username <- tweet_with_username.Add(sender, tweet)
                else
                    let tweet:string list = [tweetcontent]
                    tweet_with_username <- tweet_with_username.Add(sender, tweet)
                for i =0 to tweetcontent.Length-1 do
                    if tweetcontent.[i] = '#' then
                        let mutable ch = "#"
                        let mutable j = i+1
                        while j<tweetcontent.Length && tweetcontent.[j] <> ' ' do
                            ch <- ch + tweetcontent.[j].ToString()
                            j <- j+1
                        if tweet_with_tag.ContainsKey(ch) then
                            let mutable tweet = tweet_with_tag.[ch]
                        
                            tweet <- tweetcontent :: tweet
                            tweet_with_tag<-tweet_with_tag.Remove(ch)
                            tweet_with_tag<-tweet_with_tag.Add(ch, tweet)
                        else
                            let tweet:string list = [tweetcontent]
                            tweet_with_tag<-tweet_with_tag.Add(ch, tweet)
                    if tweetcontent.[i] = '@' then
                        let mutable ch = "@"
                        let mutable j = i+1
                        while  j<tweetcontent.Length && tweetcontent.[j] <> ' ' do
                            ch <- ch + tweetcontent.[j].ToString()
                            j <- j+1
                        if tweet_with_mention.ContainsKey(ch) then
                            let mutable tweet = tweet_with_mention.[ch]
                        
                            tweet <- tweetcontent :: tweet
                            tweet_with_mention<-tweet_with_mention.Remove(ch)
                            tweet_with_mention<-tweet_with_mention.Add(ch, tweet)
                        else
                            let tweet:string list = [tweetcontent]
                            tweet_with_mention<-tweet_with_mention.Add(ch, tweet)
            else
                tweet_not_processed <- sender+","+tweetcontent :: tweet_not_processed
        | BeSubscribed(username)->
            let mutable contained = false
            for follower in followers do
                if follower = username then
                    contained <- true
            if contained = false then
                followers <- username :: followers
        | Subscribing(username)->
            let mutable contained = false
            for following in followings do
                if following = username then
                    contained <- true
            if contained = false then
                followings <- username :: followings
        | SendTweets(tweetcontent)->
            if status = 1 then
                tweet_all <- tweetcontent :: tweet_all
                if tweet_with_username.ContainsKey(my_username) then
                    let mutable tweet = tweet_with_username.[my_username]
                    tweet <- tweetcontent :: tweet
                    tweet_with_username <- tweet_with_username.Remove(my_username)
                    tweet_with_username <- tweet_with_username.Add(my_username, tweet)
                else
                    let tweet:string list = [tweetcontent]
                    tweet_with_username <- tweet_with_username.Add(my_username, tweet)
                printfn "%A send tweet : %A" my_username tweetcontent
                for i =0 to tweetcontent.Length-1 do
                    if tweetcontent.[i] = '#' then
                        let mutable ch = "#"
                        let mutable j = i+1
                        while j<tweetcontent.Length && tweetcontent.[j] <> ' ' do
                            ch <- ch + tweetcontent.[j].ToString()
                            j <- j+1
                        if tweet_with_tag.ContainsKey(ch) then
                            let mutable tweet = tweet_with_tag.[ch]
                        
                            tweet <- tweetcontent :: tweet
                            tweet_with_tag<-tweet_with_tag.Remove(ch)
                            tweet_with_tag<-tweet_with_tag.Add(ch, tweet)
                        else
                            let tweet:string list = [tweetcontent]
                            tweet_with_tag<-tweet_with_tag.Add(ch, tweet)
                    if tweetcontent.[i] = '@' then
                        let mutable ch = "@"
                        let mutable j = i+1
                        while  j<tweetcontent.Length && tweetcontent.[j] <> ' ' do
                            ch <- ch + tweetcontent.[j].ToString()
                            j <- j+1
                        if tweet_with_mention.ContainsKey(ch) then
                            let mutable tweet = tweet_with_mention.[ch]
                        
                            tweet <- tweetcontent :: tweet
                            tweet_with_mention<-tweet_with_mention.Remove(ch)
                            tweet_with_mention<-tweet_with_mention.Add(ch, tweet)
                        else
                            let tweet:string list = [tweetcontent]
                            tweet_with_mention<-tweet_with_mention.Add(ch, tweet)
                //tell engine that send tweets to followers
                //Transit <! [["STF"];[tweetcontent]; followers; [my_username]]
            else
                printfn "Send tweet failed. Please log in first!"
        | ReTweets(tweetcontent) ->
            if status = 1 then
                tweet_all<-tweetcontent::tweet_all
                if tweet_with_username.ContainsKey(my_username) then
                    let mutable tweet = tweet_with_username.[my_username]
                    tweet <- tweetcontent :: tweet
                    tweet_with_username <- tweet_with_username.Remove(my_username)
                    tweet_with_username <- tweet_with_username.Add(my_username, tweet)
                else
                    let tweet:string list = [tweetcontent]
                    tweet_with_username <- tweet_with_username.Add(my_username, tweet)
                printfn "%A re-tweet : %A" my_username tweetcontent
                for i =0 to tweetcontent.Length-1 do
                    if tweetcontent.[i] = '#' then
                        let mutable ch = "#"
                        let mutable j = i+1
                        while j<tweetcontent.Length && tweetcontent.[j] <> ' ' do
                            ch <- ch + tweetcontent.[j].ToString()
                            j <- j+1
                        if tweet_with_tag.ContainsKey(ch) then
                            let mutable tweet = tweet_with_tag.[ch]
                        
                            tweet <- tweetcontent :: tweet
                            tweet_with_tag<-tweet_with_tag.Remove(ch)
                            tweet_with_tag<-tweet_with_tag.Add(ch, tweet)
                        else
                            let tweet:string list = [tweetcontent]
                            tweet_with_tag<-tweet_with_tag.Add(ch, tweet)
                    if tweetcontent.[i] = '@' then
                        let mutable ch = "@"
                        let mutable j = i+1
                        while  j<tweetcontent.Length && tweetcontent.[j] <> ' ' do
                            ch <- ch + tweetcontent.[j].ToString()
                            j <- j+1
                        if tweet_with_mention.ContainsKey(ch) then
                            let mutable tweet = tweet_with_mention.[ch]
                        
                            tweet <- tweetcontent :: tweet
                            tweet_with_mention<-tweet_with_mention.Remove(ch)
                            tweet_with_mention<-tweet_with_mention.Add(ch, tweet)
                        else
                            let tweet:string list = [tweetcontent]
                            tweet_with_mention<-tweet_with_mention.Add(ch, tweet)
                //Transit <! [["STF"];[tweetcontent]; followers; [my_username]]
            else
                printfn "Retweet failed! Please log in first!"
        | QueryByTag (tag)->
            if tweet_with_tag.ContainsKey(tag) then
                for tweet in tweet_with_tag.[tag] do
                    printfn "Query tweets(key:%A) : %A" tag tweet
            else
                printfn "No tweet found using this key!"
        | QueryByMentioned (mention)->
            if tweet_with_mention.ContainsKey(mention) then
                for tweet in tweet_with_mention.[mention] do
                    printfn "Query tweets(key:%A) : %A" mention tweet
            else
                printfn "No tweet found using this key!"
        | QueryByUsername (username)->
            if tweet_with_username.ContainsKey(username) then
                for tweet in tweet_with_username.[username] do
                    printfn "Query tweets(key:%A) : %A" username tweet
            else
                printfn "No tweet found using this key!"
        | LogInSuccess ->
            status <- 1
            printfn "%A login succeed!" my_username
            for tweet in tweet_not_processed do
                let index_i = tweet.IndexOf(',')
                //let mutable index = index_i-1
                let sender = tweet.[0..index_i-1]
                //index <- index_i+1
                let tweetcontent = tweet.[(index_i+1)..]
                tweet_all<-tweetcontent::tweet_all
                if tweet_with_username.ContainsKey(sender) then
                    let mutable tweet = tweet_with_username.[sender]
                    tweet <- tweetcontent :: tweet
                    tweet_with_username <- tweet_with_username.Remove(sender)
                    tweet_with_username <- tweet_with_username.Add(sender, tweet)
                else
                    let tweet:string list = [tweetcontent]
                    tweet_with_username <- tweet_with_username.Add(sender, tweet)
                printfn "%A received tweet from %A : %A" my_username sender tweetcontent
                for i =0 to tweetcontent.Length-1 do
                    if tweetcontent.[i] = '#' then
                        let mutable ch = "#"
                        let mutable j = i+1
                        while j<tweetcontent.Length && tweetcontent.[j] <> ' ' do
                            ch <- ch + tweetcontent.[j].ToString()
                            j <- j+1
                        if tweet_with_tag.ContainsKey(ch) then
                            let mutable tweet = tweet_with_tag.[ch]
                            
                            tweet <- tweetcontent :: tweet
                            tweet_with_tag<-tweet_with_tag.Remove(ch)
                            tweet_with_tag<-tweet_with_tag.Add(ch, tweet)
                        else
                            let tweet:string list = [tweetcontent]
                            tweet_with_tag<-tweet_with_tag.Add(ch, tweet)
                    if tweetcontent.[i] = '@' then
                        let mutable ch = "@"
                        let mutable j = i+1
                        while  j<tweetcontent.Length && tweetcontent.[j] <> ' ' do
                            ch <- ch + tweetcontent.[j].ToString()
                            j <- j+1
                        if tweet_with_mention.ContainsKey(ch) then
                            let mutable tweet = tweet_with_mention.[ch]
                            
                            tweet <- tweetcontent :: tweet
                            tweet_with_mention<-tweet_with_mention.Remove(ch)
                            tweet_with_mention<-tweet_with_mention.Add(ch, tweet)
                        else
                            let tweet:string list = [tweetcontent]
                            tweet_with_mention<-tweet_with_mention.Add(ch, tweet)
            tweet_not_processed <- []
        | LogOutSuccess ->
            status <- 0
            printfn "%A logout succeed!" my_username
        return! loop ()
    }
    loop ()

let mutable usermap = Map.empty<string,IActorRef>
let system = ActorSystem.Create "System"

module ActorModule = 
    [<Remote>]
    let RegisterSuccess(username,psw) = async {
        let User = spawn system username UserActor
        User <! Initialize(username,psw)
        usermap <- usermap.Add(username, User)
    }
    [<Remote>]
    let LogInSuccess(username) = async {
        usermap.[username] <! LogInSuccess
    }
    [<Remote>]
    let LogOutSuccess(username) = async {
        usermap.[username] <! LogOutSuccess
    }
    [<Remote>]
    let Subscribing(username,subscribeto) = async {
        usermap.[username] <! Subscribing(subscribeto)
    }
    [<Remote>]
    let BeSubscribed(subscribeto,username) = async {
        usermap.[subscribeto] <! BeSubscribed(username)
    }
    [<Remote>]
    let ResponseSendTweet(username,tweetcontent,followers:string list) = async {
        //Let one user send tweets 
        //usermap.[username] <! LogOutSuccess
        usermap.[username] <! SendTweets(tweetcontent)
        //Let followers of the user listen to the tweets
        for follower in followers do
            usermap.[follower] <! Join(tweetcontent,username)
    }
    [<Remote>]
    let ResponseReTweet(username,tweetcontent) = async {
        usermap.[username] <! ReTweets(tweetcontent)
    }
    [<Remote>]
    let QueryByTag(username,tag) = async {
        usermap.[username] <! QueryByTag(tag)
    }
    [<Remote>]
    let QueryByMentioned(username,mentioned) = async {
        usermap.[username] <! QueryByMentioned(mentioned)
    }
    [<Remote>]
    let QueryByUsername(username,queriedusername) = async { 
        usermap.[username] <! QueryByUsername(queriedusername)
    }
    [<Remote>]
    let SplitByComma(input:string) =
        let cmd = input.Split ","
        cmd

[<JavaScript>]
let WebSocketTest (endpoint : WebSocketEndpoint<Server.S2CMessage, Server.C2SMessage>) =

    let mutable cmdInput = ""
    //if the command is correct,flag = 1;otherwise flag = 0
    let mutable flag = 0
    //the first argument in command, which represents username
    let mutable arg1 = ""
    let mutable arg2 = ""
    let mutable user = ""
    //Command Format:
    //register,username,psw
    //login,username,psw
    //logout
    //subscribe,username
    //send,content
    //retweet,content
    //query,tag,tag_content
    //query,mentioned,mentioned_content
    //query,username,username_content

    let vInput = Var.Create ""
    let submit = Submitter.CreateOption vInput.View
    let vTextView =
        submit.View.MapAsync(function
            | None -> 
                flag <- 0
                async { 
                    return "" 
                }
            | Some input -> 
                let cmd = ActorModule.SplitByComma(input)
                if cmd.[0] = "register" then
                    if cmd.Length = 3 then
                        flag <- 1
                        cmdInput <- cmd.[0]
                        arg1 <- cmd.[1]
                        arg2 <- cmd.[2]
                        async { return "" }
                    else
                        flag <- 0
                        Server.ShowError input
                elif cmd.[0] = "login" then
                    if cmd.Length = 3 then
                        if user = "" then
                            flag <- 1
                            cmdInput <- cmd.[0]
                            user <- cmd.[1]
                            arg1 <- cmd.[1]
                            arg2 <- cmd.[2]
                            async { return "" }
                            //Server.ShowLogIn user
                        else
                            flag <- 0
                            Server.DuplicateLogin input
                    else
                        flag <- 0
                        Server.ShowError input
                elif cmd.[0] = "logout" then
                    if cmd.Length = 1 then
                        flag <- 1
                        cmdInput <- cmd.[0]
                        if user <> "" then
                            Server.ShowLogOut user
                        else
                            flag <- 0
                            Server.ShowWrongOrder input
                    else
                        flag <- 0
                        Server.ShowError input
                elif cmd.[0] = "subscribe" then
                    if cmd.Length = 2 then
                        flag <- 1
                        cmdInput <- cmd.[0]
                        arg1 <- cmd.[1]
                        if user = "" then
                            flag <- 0
                            Server.ShowWrongOrder input
                        elif user = arg1 then
                            flag <- 0
                            Server.SubYourself input
                        else
                            async { return "" }
                        //Server.ShowRegister "subscribe"
                    else
                        flag <- 0
                        Server.ShowError input
                    
                elif cmd.[0] = "send" then
                    if cmd.Length = 2 then
                        flag <- 1
                        cmdInput <- cmd.[0]
                        arg1 <- cmd.[1]
                        if user = "" then
                            flag <- 0
                            Server.ShowWrongOrder input
                        else
                            async { return "" }
                        //Server.ShowRegister "send"
                    else
                        flag <- 0
                        Server.ShowError input
                elif cmd.[0] = "retweet" then
                    if cmd.Length = 2 then
                        flag <- 1
                        cmdInput <- cmd.[0]
                        arg1 <- cmd.[1]
                        if user = "" then
                            flag <- 0
                            Server.ShowWrongOrder input
                        else
                            async { return "" }
                    else
                        flag <- 0
                        Server.ShowError input
                elif cmd.[0] = "query" then
                    if cmd.Length = 3 then
                        if cmd.[1] = "tag" then
                            flag <- 1
                            cmdInput <- cmd.[0]
                            arg1 <- cmd.[1]
                            arg2 <- cmd.[2]
                            if user = "" then
                                flag <- 0
                                Server.ShowWrongOrder input
                            else
                                async { return "" }
                        elif cmd.[1] = "mentioned" then
                            flag <- 1
                            cmdInput <- cmd.[0]
                            arg1 <- cmd.[1]
                            arg2 <- cmd.[2]
                            if user = "" then
                                flag <- 0
                                Server.ShowWrongOrder input
                            else
                                async { return "" }
                        elif cmd.[1] = "username" then
                            flag <- 1
                            cmdInput <- cmd.[0]
                            arg1 <- cmd.[1]
                            arg2 <- cmd.[2]
                            if user = "" then
                                flag <- 0
                                Server.ShowWrongOrder input
                            else
                                async { return "" }
                        else
                            flag <- 0
                            Server.ShowError input
                    else
                        flag <- 0
                        Server.ShowError input
                else
                    flag <- 0
                    Server.ShowError input
                
        )

    let container = Elt.pre [] []
    let writen fmt =
        Printf.ksprintf (fun s ->
            JS.Document.CreateTextNode(s + "\n")
            |> container.Dom.AppendChild
            |> ignore
        ) fmt
    async {
        let! server =
            ConnectStateful endpoint <| fun server -> async {
                return 0, fun state msg -> async {
                    match msg with
                    | Message data ->
                        match data with
                        | Server.RegisterSuccess(username,psw) ->  
                            writen "%s registed successfully, password: *** " username

                            do! ActorModule.RegisterSuccess(username,psw)
                        | Server.RegisterFailed(username) ->
                            writen "Registed failed, %s has already been registed!" username
                        | Server.LogInSuccess (username,psw) ->
                            writen "%s logged in." username
                            writen "Welcome, %s!" username
                            writen "----Current User: %s----" username
                            do! ActorModule.LogInSuccess(username)
                        | Server.LogInFailed ->
                            user <- ""
                            writen "Invalid username or password!"
                        | Server.LogOutSuccess (username) ->
                            writen "%s logged out." username
                            do! ActorModule.LogOutSuccess(username)
                        | Server.Subscribing(username,subscribeto) ->
                            writen "%s subscribes %s successfully." username subscribeto
                            do! ActorModule.Subscribing(username,subscribeto)
                        | Server.SubscribeFailed(username, subscribeto) ->
                            writen "%s does not exist or %s has already subscribed to %s." subscribeto username subscribeto
                        | Server.BeSubscribed(subscribeto,username) ->
                            //writen "BeSubscribed"
                            do! ActorModule.BeSubscribed(subscribeto,username)
                            
                        | Server.ResponseSendTweet(username,tweetcontent,followers) ->
                            do! ActorModule.ResponseSendTweet(username,tweetcontent,followers)
                            writen "%s sends a tweet: %s" username tweetcontent
                        | Server.ResponseReTweet(username,tweetcontent) ->
                            do! ActorModule.ResponseReTweet(username,tweetcontent)
                            writen "%s retweets successfully." username
                        | Server.QueryByTag(username,tag)->
                            do! ActorModule.QueryByTag(username,tag)
                            writen "Query By Tag, please check console for details."
                        | Server.QueryByMentioned(username,mentioned)->
                            do! ActorModule.QueryByMentioned(username,mentioned)
                            writen "Query By Mentioned, please check console for details."
                        | Server.QueryByUsername(username,queriedusername)->
                            do! ActorModule.QueryByUsername(username,queriedusername)
                            writen "Query By Username, please check console for details."
                        return (state + 1)
                    | Close ->
                        writen "WebSocket connection closed."
                        return state
                    | Open ->
                        writen "WebSocket connection open."
                        return state
                    | Error ->
                        writen "WebSocket connection error!"
                        return state
                }
            }
        //run command
        while true do
            if flag = 1 then
                flag <- 0
                if cmdInput = "register" then 
                    do! Async.Sleep 500
                    server.Post (Server.RequestRegister (arg1,arg2))
                elif cmdInput = "login" then 
                    do! Async.Sleep 500
                    server.Post (Server.RequestLogIn (arg1,arg2))
                elif cmdInput = "logout" then 
                    do! Async.Sleep 500
                    server.Post (Server.RequestLogOut (user))
                    user <- ""
                elif cmdInput = "subscribe" then 
                    do! Async.Sleep 500
                    server.Post (Server.WantSubscribe (user,arg1))
                elif cmdInput = "send" then 
                    do! Async.Sleep 500
                    server.Post (Server.WantSendTweet (user,arg1))
                elif cmdInput = "retweet" then 
                    do! Async.Sleep 500
                    server.Post (Server.WantReTweet (user,arg1))
                elif cmdInput = "query" then 
                    if arg1 = "tag" then
                        do! Async.Sleep 500
                        server.Post (Server.WantQueryByTag (user,arg2))
                    elif arg1 = "mentioned" then
                        do! Async.Sleep 500
                        server.Post (Server.WantQueryByMentioned (user,arg2))
                    elif arg1 = "username" then
                        do! Async.Sleep 500
                        server.Post (Server.WantQueryByUsername (user,arg2))
             
            elif flag = 0 then
                do! Async.Sleep 500
    }
    |> Async.Start

    div [] [
        
        Doc.Input [] vInput
        Doc.Button "Send" [] submit.Trigger
        hr [] []
        h4 [attr.``class`` "text-muted"] [text "The server responded:"]
        div [attr.``class`` "jumbotron"] [h4 [] [textView vTextView]]
        
        container
        
    ]

let MyEndPoint (url: string) : WebSharper.AspNetCore.WebSocket.WebSocketEndpoint<Server.S2CMessage, Server.C2SMessage> = 
    WebSocketEndpoint.Create(url, "/ws", JsonEncoding.Readable)