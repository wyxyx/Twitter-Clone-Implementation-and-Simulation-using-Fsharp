module ClientServerTest1.WebSocketServer
open WebSharper
open WebSharper.AspNetCore.WebSocket.Server

[<Rpc>]
let ShowLogOut input =
    let R (s: string) = System.String((s+" has logged out!").ToCharArray())
    async {
        return R input
    }
[<Rpc>]
let ShowError input =
    let R (s: string) = System.String("Wrong Command!".ToCharArray())
    async {
        return R input
    }
[<Rpc>]
let ShowWrongOrder input =
    let R (s: string) = System.String("Please log in first!".ToCharArray())
    async {
        return R input
    }
[<Rpc>]
let DuplicateLogin input =
    let R (s: string) = System.String("Please log out first!".ToCharArray())
    async {
        return R input
    }
[<Rpc>]
let SubYourself input =
    let R (s: string) = System.String("Cannot subscribe to yourself!".ToCharArray())
    async {
        return R input
    }
type [<JavaScript; NamedUnionCases>]
    C2SMessage =
    | RequestRegister of username0:string * psw0:string  //username,password
    | RequestLogIn of username1:string * psw1:string  //become active
    | RequestLogOut of username2:string  //become inactive
    | WantSubscribe of username3:string * subscribeto3:string
    | WantSendTweet of username4:string * tweetcontent4:string
    | WantReTweet of username5:string * tweetcontent5:string
    | WantQueryByTag of username6:string * tag:string
    | WantQueryByMentioned of username7:string * mentioned:string
    | WantQueryByUsername of username8:string * queriedusername:string

and [<JavaScript; NamedUnionCases "type">]
    S2CMessage =
    | RegisterSuccess of username9:string * psw9:string  //username,password
    | RegisterFailed of username19:string
    | LogInSuccess of username10:string * psw10:string  //become active
    | LogInFailed
    | LogOutSuccess of username11:string  //become inactive
    | Subscribing of username12:string* subscribeto12:string
    | SubscribeFailed of username20:string * subscribeto20:string
    | BeSubscribed of subscribeto13:string * username13:string
    | ResponseSendTweet of username14:string * tweetcontent14:string * followers14: string list
    | ResponseReTweet of username15:string * tweetcontent15:string
    | QueryByTag of username16:string * tag16:string
    | QueryByMentioned of username17:string * mentioned17:string
    | QueryByUsername of username18:string * queriedusername18:string
//map of user and password
let mutable passwdMap = Map.empty<string,string>
//map of user and followers
let mutable followersMap = Map.empty<string,string list>

let mutable i=0
let Start() : StatefulAgent<S2CMessage, C2SMessage, int> =
    // print to debug output and stdout
    let dprintfn x =
        Printf.ksprintf (fun s ->
            System.Diagnostics.Debug.WriteLine s
            stdout.WriteLine s
        ) x

    fun client -> async {
        let clientIp = client.Connection.Context.Connection.RemoteIpAddress.ToString()
        return 0, fun state msg -> async {
            dprintfn "Received message #%i from %s" state clientIp
            match msg with
            | Message data -> 
                match data with
                | RequestRegister(username,psw) ->
                    if passwdMap.ContainsKey(username) then
                        do! client.PostAsync (RegisterFailed (username))
                        dprintfn "Register Failed!"
                    else
                        do! client.PostAsync (RegisterSuccess (username,psw))
                        passwdMap <- passwdMap.Add(username, psw)
                        followersMap <- followersMap.Add(username, [])
                | RequestLogIn(username,psw) ->
                    if passwdMap.ContainsKey(username) && psw = passwdMap.[username] then
                        do! client.PostAsync (LogInSuccess (username,psw))
                    else
                        do! client.PostAsync (LogInFailed)
                        dprintfn "Login failed. Invalid username or password."
                | RequestLogOut(username) ->
                    do! client.PostAsync (LogOutSuccess (username))
                    dprintfn "%A Logout successfully." username
                | WantSubscribe(username, subscribeto) ->
                    if passwdMap.ContainsKey(subscribeto) then
                        let mutable temp_flag = 0
                        for follower in followersMap.[subscribeto] do
                            if follower = username then
                                temp_flag <- 1
                        if temp_flag = 0 then
                            do! client.PostAsync (Subscribing(username,subscribeto))
                            do! client.PostAsync (BeSubscribed(subscribeto,username))

                            let mutable follower = followersMap.[subscribeto]
                            follower <- username :: follower
                            followersMap<-followersMap.Remove(subscribeto)
                            followersMap<-followersMap.Add(subscribeto, follower)
                        else
                            do! client.PostAsync (SubscribeFailed(username, subscribeto))
                    else
                        do! client.PostAsync (SubscribeFailed(username, subscribeto))
                | WantSendTweet(username, tweetcontent) ->
                    do! client.PostAsync (ResponseSendTweet(username, tweetcontent,followersMap.[username]))
                | WantReTweet(username, tweetcontent) ->
                    do! client.PostAsync (ResponseReTweet(username,tweetcontent))
                | WantQueryByTag(username, tag) ->
                    do! client.PostAsync (QueryByTag(username, tag))
                | WantQueryByMentioned(username, mentioned) ->
                    do! client.PostAsync (QueryByMentioned(username, mentioned))
                | WantQueryByUsername(username, queriedusername) ->
                    do! client.PostAsync (QueryByUsername(username, queriedusername))
                       
                return state + 1
            | Error exn -> 
                eprintfn "Error in WebSocket server connected to %s: %s" clientIp exn.Message
                //do! client.PostAsync (Response1 ("Error: " + exn.Message))
                return state
            | Close ->
                dprintfn "Closed connection to %s" clientIp
                return state
        }
    }
