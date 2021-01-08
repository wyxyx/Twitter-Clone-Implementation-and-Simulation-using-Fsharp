# Twitter-Clone-Implementation-and-Simulation-using-Fsharp
Implemented both web-based and websocket-based API for Twitter

### Part 1 (Project 4-1)
Implement a Twitter Clone and a client tester/simulator.

Specific things: 
•	 Implement a Twitter like engine with the following functionality:
  o	Register account
  o	Send tweet. Tweets can have hashtags (e.g. #COP5615isgreat) and mentions (@bestuser)
  o	Subscribe to user's tweets
  o	Re-tweets (so that your subscribers get an interesting tweet you got by other means)
  o	Allow querying tweets subscribed to, tweets with specific hashtags, tweets in which the user is mentioned (my mentions)
  o	If the user is connected, deliver the above types of tweets live (without querying)
•	Implement a tester/simulator to test the above
  o	Simulate as many users as you can
  o	Simulate periods of live connection and disconnection for users
  o	Simulate a Zipf distribution on the number of subscribers. For accounts with a lot of subscribers, increase the number of tweets. Make some of these messages re-tweets
•	Other considerations:
  o	The client part (send/receive tweets) and the engine (distribute tweets) have to be in separate processes. Preferably, you use multiple independent client processes that     simulate thousands of clients and a single engine process
  o	You need to measure various aspects of your simulator and report performance 
  o	More detail in lecture as the project progresses.
