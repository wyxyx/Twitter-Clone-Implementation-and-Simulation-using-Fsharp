# Twitter-Clone-Implementation-and-Simulation-using-Fsharp
Implemented both web-based and websocket-based API for Twitter

### •	 Part 1 (Project 4-1)
Implement a Twitter Clone and a client tester/simulator.  
#### •	 Implement a Twitter like engine with the following functionality:  
1.	Register account  
2.	Send tweet. Tweets can have hashtags (e.g. #COP5615isgreat) and mentions (@bestuser)  
3.	Subscribe to user's tweets  
4.	Re-tweets (so that your subscribers get an interesting tweet you got by other means)  
5.	Allow querying tweets subscribed to, tweets with specific hashtags, tweets in which the user is mentioned (my mentions)  
6.	If the user is connected, deliver the above types of tweets live (without querying)  
#### •	Implement a tester/simulator to test the above  
1.	Simulate as many users as you can  
2.	Simulate periods of live connection and disconnection for users  
3.	Simulate a Zipf distribution on the number of subscribers. For accounts with a lot of subscribers, increase the number of tweets. Make some of these messages re-tweets  
#### •	Other considerations:  
The client part (send/receive tweets) and the engine (distribute tweets) have to be in separate processes.  
### •	 Part 2 (Project 4-2)  
Use WebSharper web framework to implement a WebSocket interface to your part I implementation. That means that, even though the F#  implementation (Part I) you could use AKKA messaging to allow client-server implementation, you now need to design and use a proper WebSocket interface. Specifically:  
1.	You need to design a JSON based API that  represents all messages and their replies (including errors).  
2.	You need to re-write parts of your engine using WebSharper to implement the WebSocket interface.  
3.	You need to re-write parts of your client to use WebSockets.  
