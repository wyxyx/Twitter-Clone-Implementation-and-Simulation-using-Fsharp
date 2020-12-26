Project 4 Part 2

The link of our running program is: https://recordit.co/5VoWFv1hyh

1. Team members
Name: Zixun Wang             UFID: 3725-9823
Name: Yixin Wei              UFID: 5114-6181

2. Implementation

In part I, we used one engine process and one user process to simulate the twitter. 
In part II, we used WebSharper web framework to implement a WebSocket interface to our part I implementation.
We implemented part II with Websharper framework, which supports remote procedure calls from the client (JavaScript environment) to the server (ASP.NET). 
This project used a JSON based API that represents all messages and their replies.

In our project, we have 4 .fs files, including WebSocketServer.fs, WebSocketClient.fs, Website.fs and Startup.fs and some html related files.
We implemented functionalities of server in WebSocketServer.fs, which processes the messages from client and records each user's information.
We implemented functionalities of client in WebSocketClient.fs, which processes the messages from server and distributes tasks to corresponding actors.

In detail, we use type C2SMessage and type S2CMessage to exchange messages in server and client. And in client, we build multiple actors to execute different tasks, 
including sign up, send tweets, search, subscribe, retweet and query.

Use the functions in the WebSharper.UI.Html module, construct a simple HTML with a text input box and a "send" button. We can input commands to simulate
 the circumstance that an user uses the twitter simulator.
 
3. Instructions to run the program

Run the project directly. And a web page ("https://localhost:5001/") will pop up, you need to input commands in the web page.

Input commands in the text input box and then click the "send" button with the following formats.

a. Firstly, use command "register,[username],[password]" to register an account. (Ex: register,user1,12345)
b. Use command "login,[username],[password]" to log in an existing account. (Ex: login,user1,12345) Now you can send tweet and subscribe.
c. Use command "subscribe,[username]" to subscribe another user. (Ex: subscribe,user2)
d. Use command "send,[content of tweet]" to send tweet to the current user's followers. (Ex: send,This is a tweet #tag1 @user3)
e. User command "retweet,[content of tweet]" to retweet to the current user's followers.(Ex: retweet,This is a tweet #tag1 @user3)
f. User command "query,tag,[tag of tweet]" to query tweets by the content of tag. (Ex: query,tag,#tag1)
g. User command "query,mentioned,[mentioned of tweet]" to query tweets by the content of mentioned. (Ex: query,mentioned,@user3)
h. User command "query,username,[the user who sent tweet]" to query tweets by the sender of the tweet. (Ex: query,username,user1)
i. And you can use command "logout" at any time to disconnect the current user. (Ex: logout)

Here are some screenshots of our running program in Project4_Part2_Report.pdf.

In the video, we open two pages to simulate the circumstance when two users are using the twitter simulator. 
Fistly, create accounts "user1" and "user2" separately. We test wrong inputs at the same time, for example, 
we register the same username and the our program prints "Register failed". 
Then, log in the two users. And let "user1" subscribes "user2". 
And if "user2" sends a tweet, "user1" would receive it.
Then log out "user1". And let "user2" sends tweets during the time when "user1" is disconnected.
When "user1" logs in again, it would receive all the tweets from its followings("user2") during the time when it is disconnected.
And we can use query commands in page of "user1" to search tweets by tag, mentioned and username.
Finally, we test retweet command.

To sum up, our program can realize the above functions successfully. 
