open Akka.Actor
open Akka.FSharp
open System.Threading
open System
open Akka.Configuration

let configuration = 
    ConfigurationFactory.ParseString(
        @"akka {
            log-config-on-start : on
            stdout-loglevel : DEBUG
            loglevel : ERROR
            actor {
                provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
            }
            remote {
                helios.tcp {
                    port = 8777
                    hostname = localhost
                }
            }
        }")

let system = ActorSystem.Create("System", configuration)
let mutable Transit = null

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
    | SimulateReTweets
    | SimQueryByUsername

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
                tweet_all<-tweetcontent::tweet_all
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
                Transit <! [["STF"];[tweetcontent]; followers; [my_username]]
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
                Transit <! [["STF"];[tweetcontent]; followers; [my_username]]
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
        | SimulateReTweets ->
            let len = tweet_all.Length
            if len <> 0 then
                let rndInt = abs(System.Random().Next()%len)
                //Engine <! WantReTweet(my_username, tweet_all.[rndInt])
                Transit <! [["WRT"]; [my_username]; [tweet_all.[rndInt]]]
            else
                printfn "No tweet can be retweeted!"
        | SimQueryByUsername ->
            let mutable temp_list:string list = []
            for pair in tweet_with_username do
                temp_list <- pair.Key :: temp_list
            let len = temp_list.Length
            if len <> 0 then
                let mutable rndInt = abs(System.Random().Next()%len)
                let username = temp_list.[rndInt]
                for tweet in tweet_with_username.[username] do
                    printfn "Query tweets(key:%A) : %A" username tweet
            else
                printfn "No tweet can be queried!"
        return! loop ()
    }
    loop ()

[<EntryPoint>]
let main argv =
    //stores username and actor
    let mutable usermap = Map.empty<string,IActorRef>
    let transit_other = system.ActorSelection("akka.tcp://System@localhost:9001/user/Transit")
    Transit <- 
        spawn system "Transit"
        <| fun mailbox ->
            let rec loop() =
                actor {
                    let! (message: string list list) = mailbox.Receive()
                    //let sender = mailbox.Sender()
                    match message with
                    | _ -> 
                        match message.[0].[0] with
                        | "STF" ->
                            transit_other <! message
                        | "WRT" ->
                            transit_other <! message
                        | "ITL" ->
                            let username = message.[1].[0]
                            let psw = message.[2].[0]
                            let User = spawn system username UserActor
                            User <! Initialize(username,psw)
                            usermap <- usermap.Add(username, User)
                        | "SSB" ->
                            usermap.[message.[1].[0]] <! Subscribing(message.[2].[0])
                        | "BSS" ->
                            usermap.[message.[1].[0]] <! BeSubscribed(message.[2].[0])
                        | "Join" ->
                            usermap.[message.[1].[0]] <! Join(message.[2].[0],message.[3].[0])
                        | "LIS" ->
                            usermap.[message.[1].[0]] <! LogInSuccess
                        | "LOS" ->
                            usermap.[message.[1].[0]] <! LogOutSuccess
                        | "STT" ->
                            usermap.[message.[1].[0]] <! SendTweets(message.[2].[0])
                        | "RTT" ->
                            usermap.[message.[1].[0]] <! ReTweets(message.[2].[0])
                        | "QBT" ->
                            usermap.[message.[1].[0]] <! QueryByTag(message.[2].[0])
                        | "QBM" ->
                            usermap.[message.[1].[0]] <! QueryByMentioned(message.[2].[0])
                        | "QBU" ->
                            usermap.[message.[1].[0]] <! QueryByUsername(message.[2].[0])
                        | "SQBU" ->
                            usermap.[message.[1].[0]] <! SimQueryByUsername
                        | "SRT" ->
                            usermap.[message.[1].[0]] <! SimulateReTweets
                        | _ ->
                            printfn "Invalid message!"

                    return! loop()
                } 
            loop()
    let line = System.Console.ReadLine()
    0 // return an integer exit code
