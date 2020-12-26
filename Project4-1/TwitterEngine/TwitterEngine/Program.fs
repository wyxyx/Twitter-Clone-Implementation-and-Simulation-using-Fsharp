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
                    port = 9001
                    hostname = localhost
                }
            }
        }")

let system = ActorSystem.Create("System", configuration)
let mutable Engine = null
let mutable tweet_amount = 0
let mutable Transit = null

type Data = 
    | Register of username:string * psw:string  //username,password
    | SimulateRegister of num:int* nums:int list
    | SendTweetToFollowers of tweetcontent:string * followers:string list* sender:string
    | LogIn of username:string * psw:string  //become active
    | LogOut of username:string  //become inactive
    | WantSubscribe of username:string * subscribeto:string
    | WantSendTweet of username:string * tweetcontent:string
    | WantReTweet of username:string * tweetcontent:string
    | WantQueryByTag of username:string * tag:string
    | WantQueryByMentioned of username:string * mentioned:string
    | WantQueryByUsername of username:string * queriedusername:string
    | WantSimQueryByUsername of username:string
    | WantSimulateReTweet of username:string

let Shuffle (org:_[]) = 
    let arr = Array.copy org
    let max = (arr.Length - 1)
    let randomSwap (arr:_[]) i =
        let pos = (new Random()).Next(max)
        let tmp = arr.[pos]
        arr.[pos] <- arr.[i]
        arr.[i] <- tmp
        arr
   
    [|0..max|] |> Array.fold randomSwap arr

let EngineActor (mailbox: Actor<_>) = 
    
    //stores username and password
    let mutable passwdmap = Map.empty<string,string>

    let mutable startIndex = 0

    let rec loop () = actor {
        let! msg = mailbox.Receive ()
        match msg with
        | Register(username,psw)->
            Transit <! [["ITL"];[username];[psw]]
            passwdmap <- passwdmap.Add(username, psw)

            //if there exisit username with same name, register is not allowed

        | SimulateRegister(num,nums)->
            for i = 0 to num-1 do
                Transit <! [["ITL"];["user" + startIndex.ToString()];["123"]]
                passwdmap <- passwdmap.Add("user" + startIndex.ToString(), "123")
                startIndex <- startIndex + 1
            printfn "Create new users done!"
            printfn "Start creating subscribing relationship..."

            for i=0 to num-1 do
                if i = int num/4 then
                    printfn "25 percent done"
                elif i = int num/2 then
                    printfn "50 percent done"
                elif i = int num*3/4 then
                    printfn "75 percent done"
                elif i= int num*9/10 then
                    printfn "90 percent done"
                
                let random_subscribing = Shuffle([|0..(num-1)|])
                for k=0 to nums.[i]-1 do
                    Transit <! [["SSB"];["user"+i.ToString()];["user"+random_subscribing.[k].ToString()]]
                    Transit <! [["BSS"];["user"+random_subscribing.[k].ToString()];["user"+i.ToString()]]
            printfn "Create subscribing relationship done!"
            printfn "Waiting for users setting subscribing relationship..."

        | SendTweetToFollowers(tweetcontent,followers,sender)->
            for follower in followers do
                tweet_amount <- tweet_amount + 1
                Transit <! [["Join"];[follower];[tweetcontent];[sender]]
                
        | LogIn(username,psw) ->
            if passwdmap.ContainsKey(username) && psw = passwdmap.[username] then
                Transit <! [["LIS"];[username]]
            else
                printfn "Login failed. Invalid username or password."
        | LogOut(username) ->
            Transit <! [["LOS"];[username]]
        | WantSubscribe(username, subscribeto) ->
            Transit <! [["SSB"];[username];[subscribeto]]
            Transit <! [["BSS"];[subscribeto];[username]]
        | WantSendTweet(username, tweetcontent) ->
            tweet_amount <- tweet_amount + 1
            Transit <! [["STT"];[username];[tweetcontent]]
        | WantReTweet(username, tweetcontent) ->
            tweet_amount <- tweet_amount + 1
            Transit <! [["RTT"];[username];[tweetcontent]]
        | WantQueryByTag(username, tag) ->
            Transit <! [["QBT"];[username];[tag]]
        | WantQueryByMentioned(username, mentioned) ->
            Transit <! [["QBM"];[username];[mentioned]]
        | WantQueryByUsername(username, queriedusername) ->
            Transit <! [["QBU"];[username];[queriedusername]]
        | WantSimQueryByUsername(username) ->
            Transit <! [["SQBU"];[username]]
        | WantSimulateReTweet(username) ->
            tweet_amount <- tweet_amount + 1
            Transit <! [["SRT"];[username]]
        return! loop ()
    }
    loop ()

let simulator(number : int) =
    let randomTag = ["#COP5615Tag";"#AgoodTag";"#AbadTag";"#SomeTag"]
    //create some users
    printfn "Begin simulating..."

    let mutable sum = 0
    let mutable listOfNums : int list = []
    let mutable intervalList : int list = []
    for i=0 to number-1 do
        let userNum = abs(System.Random().Next()%(int number/3))
        listOfNums <- userNum :: listOfNums
    listOfNums <- List.sortDescending listOfNums
    //printfn "listOfNums:%A" listOfNums

    for i=0 to number-1 do
        intervalList <- sum :: intervalList
        sum <- sum + listOfNums.[i]
    intervalList <- sum :: intervalList
    intervalList <- List.sort intervalList
    
    Engine <! SimulateRegister(number,listOfNums)
    
    //------time change with amount of users
    Thread.Sleep(number*number/60)

    printfn "Initializing logging in..."
    for i=0 to int number/10 do
        let userNum = abs(System.Random().Next()%number)
        //printfn "%A" userNum
        Engine <! LogIn("user"+userNum.ToString(), "123")
        Thread.Sleep(20)
    printfn "logging in finished!"

    printfn "Start simulating commands by users..."
    let mutable EndLoop = false
    let stopWatch = System.Diagnostics.Stopwatch.StartNew()
    let mutable timeNum = 0.0
    let mutable sumcommand = 0
    let mutable tweetsendcommand = 0
    let mutable retweetcommand = 0
    let mutable qbytag = 0
    let mutable qbymen = 0
    let mutable qbyuser = 0
    let mutable logincommand = 0
    let mutable logoutcommand = 0
    while EndLoop = false do
        sumcommand <- sumcommand + 1
        let mutable userNum = abs(System.Random().Next()%number)
        let questionNum = abs(System.Random().Next()%8)
        //0 or 1:want send tweet ; 2:want retweet ; 3:want query by tag ; 4:want query by mention ; 5:want query by username ; 6:want login ; 7:want logout
        if questionNum = 0 || questionNum = 1 then 
            tweetsendcommand <- tweetsendcommand + 1
            let intervalNum = abs(System.Random().Next()%sum)
            let mutable Break = 0
            for i=0 to number do
                if intervalNum < intervalList.[i] && Break = 0 then
                    userNum <- i-1
                    Break <- 1
            let len = randomTag.Length
            let NumTag = abs(System.Random().Next()%len)
            let NumMention = abs(System.Random().Next()%number)
            let tweet = "COP5615 is great! " + randomTag.[NumTag] + " @user" + NumMention.ToString()
            Engine <! WantSendTweet("user"+userNum.ToString(), tweet)
        elif questionNum = 2 then
            retweetcommand <- retweetcommand + 1
            Engine <! WantSimulateReTweet("user"+userNum.ToString())
        elif questionNum = 3 then
            qbytag <- qbytag + 1
            let NumTag = abs(System.Random().Next() % randomTag.Length)
            Engine <! WantQueryByTag("user"+userNum.ToString(), randomTag.[NumTag])
        elif questionNum = 4 then
            qbymen <- qbymen + 1
            let NumMention = abs(System.Random().Next()%number)
            Engine <! WantQueryByMentioned("user"+userNum.ToString(), "@user" + NumMention.ToString())
        elif questionNum = 5 then
            qbyuser <- qbyuser + 1
            Engine <! WantSimQueryByUsername("user"+userNum.ToString())
        elif questionNum = 6 then
            logincommand <- logincommand + 1
            Engine <! LogIn("user"+userNum.ToString(), "123")
        elif questionNum = 7 then
            logoutcommand <- logoutcommand + 1
            Engine <! LogOut("user"+userNum.ToString())

        timeNum<-stopWatch.Elapsed.TotalMilliseconds
        if timeNum > 180000.0 then
            EndLoop <- true
        Thread.Sleep(5)
    stopWatch.Stop()
    printfn "The amount of commands that have been done is %A" sumcommand
    printfn "The amount of send tweet commands that have been done is %A" tweetsendcommand
    printfn "The amount of retweet commands that have been done is %A" retweetcommand
    printfn "The amount of query by tag commands that have been done is %A" qbytag
    printfn "The amount of query by mention commands that have been done is %A" qbymen
    printfn "The amount of query by subscribed commands that have been done is %A" qbyuser
    printfn "The amount of log in commands that have been done is %A" logincommand
    printfn "The amount of log out commands that have been done is %A" logoutcommand

[<EntryPoint>]
let main argv =

    Engine <- spawn system "engine" EngineActor
    printfn "Please input the amount of users in Twitter Simulator:"
    let line = System.Console.ReadLine()
    let num = int line

    let transit_other = system.ActorSelection("akka.tcp://System@localhost:8777/user/Transit")
    Transit <- 
        spawn system "Transit"
        <| fun mailbox ->
            let rec loop() =
                actor {
                    let! (message: string list list) = mailbox.Receive()
                    match message with
                    | _ ->
                        match message.[0].[0] with
                        | "ITL" ->
                            transit_other <! message
                        | "SSB" ->
                            transit_other <! message
                        | "BSS" ->
                            transit_other <! message
                        | "Join" ->
                            transit_other <! message
                        | "LIS" ->
                            transit_other <! message
                        | "LOS" ->
                            transit_other <! message
                        | "STT" ->
                            transit_other <! message
                        | "RTT" ->
                            transit_other <! message
                        | "QBT" ->
                            transit_other <! message
                        | "QBM" ->
                            transit_other <! message
                        | "QBU" ->
                            transit_other <! message
                        | "SQBU" ->
                            transit_other <! message
                        | "SRT" ->
                            transit_other <! message
                        | "STF" ->
                            Engine <! SendTweetToFollowers(message.[1].[0],message.[2],message.[3].[0])
                        | "WRT" ->
                            Engine <! WantReTweet(message.[1].[0], message.[2].[0])
                        | _ ->
                            printfn "Invalid message!"

                    return! loop()
                } 
            loop()


    let a = simulator(num) 
    printfn "The amount of tweets is %A" tweet_amount
    0 // return an integer exit code
