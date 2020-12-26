(function()
{
 "use strict";
 var Global,ClientServerTest1,WebSocketServer,S2CMessage,WebSocketClient,Website,Client,ClientServerTest1_Templates,ClientServerTest1_JsonEncoder,ClientServerTest1_JsonDecoder,WebSharper,UI,Var$1,Submitter,View,Remoting,AjaxRemotingProvider,Arrays,Concurrency,Doc,AspNetCore,WebSocket,Client$1,WithEncoding,JSON,Utils,IntelliFactory,Runtime,AttrProxy,Templating,Runtime$1,Server,ProviderBuilder,Handler,TemplateInstance,Client$2,Templates,ClientSideJson,Provider;
 Global=self;
 ClientServerTest1=Global.ClientServerTest1=Global.ClientServerTest1||{};
 WebSocketServer=ClientServerTest1.WebSocketServer=ClientServerTest1.WebSocketServer||{};
 S2CMessage=WebSocketServer.S2CMessage=WebSocketServer.S2CMessage||{};
 WebSocketClient=ClientServerTest1.WebSocketClient=ClientServerTest1.WebSocketClient||{};
 Website=ClientServerTest1.Website=ClientServerTest1.Website||{};
 Client=Website.Client=Website.Client||{};
 ClientServerTest1_Templates=Global.ClientServerTest1_Templates=Global.ClientServerTest1_Templates||{};
 ClientServerTest1_JsonEncoder=Global.ClientServerTest1_JsonEncoder=Global.ClientServerTest1_JsonEncoder||{};
 ClientServerTest1_JsonDecoder=Global.ClientServerTest1_JsonDecoder=Global.ClientServerTest1_JsonDecoder||{};
 WebSharper=Global.WebSharper;
 UI=WebSharper&&WebSharper.UI;
 Var$1=UI&&UI.Var$1;
 Submitter=UI&&UI.Submitter;
 View=UI&&UI.View;
 Remoting=WebSharper&&WebSharper.Remoting;
 AjaxRemotingProvider=Remoting&&Remoting.AjaxRemotingProvider;
 Arrays=WebSharper&&WebSharper.Arrays;
 Concurrency=WebSharper&&WebSharper.Concurrency;
 Doc=UI&&UI.Doc;
 AspNetCore=WebSharper&&WebSharper.AspNetCore;
 WebSocket=AspNetCore&&AspNetCore.WebSocket;
 Client$1=WebSocket&&WebSocket.Client;
 WithEncoding=Client$1&&Client$1.WithEncoding;
 JSON=Global.JSON;
 Utils=WebSharper&&WebSharper.Utils;
 IntelliFactory=Global.IntelliFactory;
 Runtime=IntelliFactory&&IntelliFactory.Runtime;
 AttrProxy=UI&&UI.AttrProxy;
 Templating=UI&&UI.Templating;
 Runtime$1=Templating&&Templating.Runtime;
 Server=Runtime$1&&Runtime$1.Server;
 ProviderBuilder=Server&&Server.ProviderBuilder;
 Handler=Server&&Server.Handler;
 TemplateInstance=Server&&Server.TemplateInstance;
 Client$2=UI&&UI.Client;
 Templates=Client$2&&Client$2.Templates;
 ClientSideJson=WebSharper&&WebSharper.ClientSideJson;
 Provider=ClientSideJson&&ClientSideJson.Provider;
 S2CMessage.LogInFailed={
  $:3
 };
 WebSocketClient.WebSocketTest=function(endpoint)
 {
  var cmdInput,flag,arg1,arg2,user,vInput,submit,vTextView,container,b;
  function writen(fmt)
  {
   return fmt(function(s)
   {
    var x;
    x=self.document.createTextNode(s+"\n");
    container.elt.appendChild(x);
   });
  }
  cmdInput="";
  flag=0;
  arg1="";
  arg2="";
  user="";
  vInput=Var$1.Create$1("");
  submit=Submitter.CreateOption(vInput.get_View());
  vTextView=View.MapAsync(function(a)
  {
   var input,cmd,b$1,b$2,b$3,b$4,b$5,b$6,b$7,b$8,b$9;
   return a!=null&&a.$==1?(input=a.$0,(cmd=(new AjaxRemotingProvider.New()).Sync("ClientServerTest1:ClientServerTest1.WebSocketClient+ActorModule.SplitByComma:1982470384",[input]),Arrays.get(cmd,0)==="register"?Arrays.length(cmd)===3?(flag=1,cmdInput=Arrays.get(cmd,0),arg1=Arrays.get(cmd,1),arg2=Arrays.get(cmd,2),b$1=null,Concurrency.Delay(function()
   {
    return Concurrency.Return("");
   })):(flag=0,(new AjaxRemotingProvider.New()).Async("ClientServerTest1:ClientServerTest1.WebSocketServer.ShowError:-1146015191",[input])):Arrays.get(cmd,0)==="login"?Arrays.length(cmd)===3?user===""?(flag=1,cmdInput=Arrays.get(cmd,0),user=Arrays.get(cmd,1),arg1=Arrays.get(cmd,1),arg2=Arrays.get(cmd,2),b$2=null,Concurrency.Delay(function()
   {
    return Concurrency.Return("");
   })):(flag=0,(new AjaxRemotingProvider.New()).Async("ClientServerTest1:ClientServerTest1.WebSocketServer.DuplicateLogin:-1146015191",[input])):(flag=0,(new AjaxRemotingProvider.New()).Async("ClientServerTest1:ClientServerTest1.WebSocketServer.ShowError:-1146015191",[input])):Arrays.get(cmd,0)==="logout"?Arrays.length(cmd)===1?(flag=1,cmdInput=Arrays.get(cmd,0),user!==""?(new AjaxRemotingProvider.New()).Async("ClientServerTest1:ClientServerTest1.WebSocketServer.ShowLogOut:-1146015191",[user]):(flag=0,(new AjaxRemotingProvider.New()).Async("ClientServerTest1:ClientServerTest1.WebSocketServer.ShowWrongOrder:-1146015191",[input]))):(flag=0,(new AjaxRemotingProvider.New()).Async("ClientServerTest1:ClientServerTest1.WebSocketServer.ShowError:-1146015191",[input])):Arrays.get(cmd,0)==="subscribe"?Arrays.length(cmd)===2?(flag=1,cmdInput=Arrays.get(cmd,0),arg1=Arrays.get(cmd,1),user===""?(flag=0,(new AjaxRemotingProvider.New()).Async("ClientServerTest1:ClientServerTest1.WebSocketServer.ShowWrongOrder:-1146015191",[input])):user===arg1?(flag=0,(new AjaxRemotingProvider.New()).Async("ClientServerTest1:ClientServerTest1.WebSocketServer.SubYourself:-1146015191",[input])):(b$3=null,Concurrency.Delay(function()
   {
    return Concurrency.Return("");
   }))):(flag=0,(new AjaxRemotingProvider.New()).Async("ClientServerTest1:ClientServerTest1.WebSocketServer.ShowError:-1146015191",[input])):Arrays.get(cmd,0)==="send"?Arrays.length(cmd)===2?(flag=1,cmdInput=Arrays.get(cmd,0),arg1=Arrays.get(cmd,1),user===""?(flag=0,(new AjaxRemotingProvider.New()).Async("ClientServerTest1:ClientServerTest1.WebSocketServer.ShowWrongOrder:-1146015191",[input])):(b$4=null,Concurrency.Delay(function()
   {
    return Concurrency.Return("");
   }))):(flag=0,(new AjaxRemotingProvider.New()).Async("ClientServerTest1:ClientServerTest1.WebSocketServer.ShowError:-1146015191",[input])):Arrays.get(cmd,0)==="retweet"?Arrays.length(cmd)===2?(flag=1,cmdInput=Arrays.get(cmd,0),arg1=Arrays.get(cmd,1),user===""?(flag=0,(new AjaxRemotingProvider.New()).Async("ClientServerTest1:ClientServerTest1.WebSocketServer.ShowWrongOrder:-1146015191",[input])):(b$5=null,Concurrency.Delay(function()
   {
    return Concurrency.Return("");
   }))):(flag=0,(new AjaxRemotingProvider.New()).Async("ClientServerTest1:ClientServerTest1.WebSocketServer.ShowError:-1146015191",[input])):Arrays.get(cmd,0)==="query"?Arrays.length(cmd)===3?Arrays.get(cmd,1)==="tag"?(flag=1,cmdInput=Arrays.get(cmd,0),arg1=Arrays.get(cmd,1),arg2=Arrays.get(cmd,2),user===""?(flag=0,(new AjaxRemotingProvider.New()).Async("ClientServerTest1:ClientServerTest1.WebSocketServer.ShowWrongOrder:-1146015191",[input])):(b$6=null,Concurrency.Delay(function()
   {
    return Concurrency.Return("");
   }))):Arrays.get(cmd,1)==="mentioned"?(flag=1,cmdInput=Arrays.get(cmd,0),arg1=Arrays.get(cmd,1),arg2=Arrays.get(cmd,2),user===""?(flag=0,(new AjaxRemotingProvider.New()).Async("ClientServerTest1:ClientServerTest1.WebSocketServer.ShowWrongOrder:-1146015191",[input])):(b$7=null,Concurrency.Delay(function()
   {
    return Concurrency.Return("");
   }))):Arrays.get(cmd,1)==="username"?(flag=1,cmdInput=Arrays.get(cmd,0),arg1=Arrays.get(cmd,1),arg2=Arrays.get(cmd,2),user===""?(flag=0,(new AjaxRemotingProvider.New()).Async("ClientServerTest1:ClientServerTest1.WebSocketServer.ShowWrongOrder:-1146015191",[input])):(b$8=null,Concurrency.Delay(function()
   {
    return Concurrency.Return("");
   }))):(flag=0,(new AjaxRemotingProvider.New()).Async("ClientServerTest1:ClientServerTest1.WebSocketServer.ShowError:-1146015191",[input])):(flag=0,(new AjaxRemotingProvider.New()).Async("ClientServerTest1:ClientServerTest1.WebSocketServer.ShowError:-1146015191",[input])):(flag=0,(new AjaxRemotingProvider.New()).Async("ClientServerTest1:ClientServerTest1.WebSocketServer.ShowError:-1146015191",[input])))):(flag=0,b$9=null,Concurrency.Delay(function()
   {
    return Concurrency.Return("");
   }));
  },submit.view);
  container=Doc.Element("pre",[],[]);
  Concurrency.Start((b=null,Concurrency.Delay(function()
  {
   return Concurrency.Bind(WithEncoding.ConnectStateful(function(a)
   {
    return JSON.stringify((ClientServerTest1_JsonEncoder.j())(a));
   },function(a)
   {
    return(ClientServerTest1_JsonDecoder.j())(JSON.parse(a));
   },endpoint,function()
   {
    var b$1;
    b$1=null;
    return Concurrency.Delay(function()
    {
     return Concurrency.Return([0,function(state)
     {
      return function(msg)
      {
       var b$2;
       b$2=null;
       return Concurrency.Delay(function()
       {
        var data,username,username$1,username$2,subscribeto,subscribeto$1,username$3,tweetcontent,username$4,username$5;
        return msg.$==3?(writen(function($1)
        {
         return $1("WebSocket connection closed.");
        }),Concurrency.Return(state)):msg.$==2?(writen(function($1)
        {
         return $1("WebSocket connection open.");
        }),Concurrency.Return(state)):msg.$==1?(writen(function($1)
        {
         return $1("WebSocket connection error!");
        }),Concurrency.Return(state)):(data=msg.$0,Concurrency.Combine(data.$==1?((writen(function($1)
        {
         return function($2)
         {
          return $1("Registed failed, "+Utils.toSafe($2)+" has already been registed!");
         };
        }))(data.$0),Concurrency.Zero()):data.$==2?(username=data.$0,((writen(function($1)
        {
         return function($2)
         {
          return $1(Utils.toSafe($2)+" logged in.");
         };
        }))(username),(writen(function($1)
        {
         return function($2)
         {
          return $1("Welcome, "+Utils.toSafe($2)+"!");
         };
        }))(username),(writen(function($1)
        {
         return function($2)
         {
          return $1("----Current User: "+Utils.toSafe($2)+"----");
         };
        }))(username),Concurrency.Bind((new AjaxRemotingProvider.New()).Async("ClientServerTest1:ClientServerTest1.WebSocketClient+ActorModule.LogInSuccess:1131399653",[username]),function()
        {
         return Concurrency.Return(null);
        }))):data.$==3?(user="",writen(function($1)
        {
         return $1("Invalid username or password!");
        }),Concurrency.Zero()):data.$==4?(username$1=data.$0,((writen(function($1)
        {
         return function($2)
         {
          return $1(Utils.toSafe($2)+" logged out.");
         };
        }))(username$1),Concurrency.Bind((new AjaxRemotingProvider.New()).Async("ClientServerTest1:ClientServerTest1.WebSocketClient+ActorModule.LogOutSuccess:1131399653",[username$1]),function()
        {
         return Concurrency.Return(null);
        }))):data.$==5?(username$2=data.$0,(subscribeto=data.$1,(((writen(Runtime.Curried3(function($1,$2,$3)
        {
         return $1(Utils.toSafe($2)+" subscribes "+Utils.toSafe($3)+" successfully.");
        })))(username$2))(subscribeto),Concurrency.Bind((new AjaxRemotingProvider.New()).Async("ClientServerTest1:ClientServerTest1.WebSocketClient+ActorModule.Subscribing:-685279371",[username$2,subscribeto]),function()
        {
         return Concurrency.Return(null);
        })))):data.$==6?(subscribeto$1=data.$1,((((writen(Runtime.Curried(function($1,$2,$3,$4)
        {
         return $1(Utils.toSafe($2)+" does not exist or "+Utils.toSafe($3)+" has already subscribed to "+Utils.toSafe($4)+".");
        },4)))(subscribeto$1))(data.$0))(subscribeto$1),Concurrency.Zero())):data.$==7?Concurrency.Bind((new AjaxRemotingProvider.New()).Async("ClientServerTest1:ClientServerTest1.WebSocketClient+ActorModule.BeSubscribed:-685279371",[data.$0,data.$1]),function()
        {
         return Concurrency.Return(null);
        }):data.$==8?(username$3=data.$0,(tweetcontent=data.$1,Concurrency.Bind((new AjaxRemotingProvider.New()).Async("ClientServerTest1:ClientServerTest1.WebSocketClient+ActorModule.ResponseSendTweet:-47597098",[username$3,tweetcontent,data.$2]),function()
        {
         ((writen(Runtime.Curried3(function($1,$2,$3)
         {
          return $1(Utils.toSafe($2)+" sends a tweet: "+Utils.toSafe($3));
         })))(username$3))(tweetcontent);
         return Concurrency.Zero();
        }))):data.$==9?(username$4=data.$0,Concurrency.Bind((new AjaxRemotingProvider.New()).Async("ClientServerTest1:ClientServerTest1.WebSocketClient+ActorModule.ResponseReTweet:-685279371",[username$4,data.$1]),function()
        {
         (writen(function($1)
         {
          return function($2)
          {
           return $1(Utils.toSafe($2)+" retweets successfully.");
          };
         }))(username$4);
         return Concurrency.Zero();
        })):data.$==10?Concurrency.Bind((new AjaxRemotingProvider.New()).Async("ClientServerTest1:ClientServerTest1.WebSocketClient+ActorModule.QueryByTag:-685279371",[data.$0,data.$1]),function()
        {
         writen(function($1)
         {
          return $1("Query By Tag, please check console for details.");
         });
         return Concurrency.Zero();
        }):data.$==11?Concurrency.Bind((new AjaxRemotingProvider.New()).Async("ClientServerTest1:ClientServerTest1.WebSocketClient+ActorModule.QueryByMentioned:-685279371",[data.$0,data.$1]),function()
        {
         writen(function($1)
         {
          return $1("Query By Mentioned, please check console for details.");
         });
         return Concurrency.Zero();
        }):data.$==12?Concurrency.Bind((new AjaxRemotingProvider.New()).Async("ClientServerTest1:ClientServerTest1.WebSocketClient+ActorModule.QueryByUsername:-685279371",[data.$0,data.$1]),function()
        {
         writen(function($1)
         {
          return $1("Query By Username, please check console for details.");
         });
         return Concurrency.Zero();
        }):(username$5=data.$0,((writen(function($1)
        {
         return function($2)
         {
          return $1(Utils.toSafe($2)+" registed successfully, password: *** ");
         };
        }))(username$5),Concurrency.Bind((new AjaxRemotingProvider.New()).Async("ClientServerTest1:ClientServerTest1.WebSocketClient+ActorModule.RegisterSuccess:-685279371",[username$5,data.$1]),function()
        {
         return Concurrency.Return(null);
        }))),Concurrency.Delay(function()
        {
         return Concurrency.Return(state+1);
        })));
       });
      };
     }]);
    });
   }),function(a)
   {
    return Concurrency.While(function()
    {
     return true;
    },Concurrency.Delay(function()
    {
     return flag===1?(flag=0,cmdInput==="register"?Concurrency.Bind(Concurrency.Sleep(500),function()
     {
      a.Post({
       $:0,
       $0:arg1,
       $1:arg2
      });
      return Concurrency.Zero();
     }):cmdInput==="login"?Concurrency.Bind(Concurrency.Sleep(500),function()
     {
      a.Post({
       $:1,
       $0:arg1,
       $1:arg2
      });
      return Concurrency.Zero();
     }):cmdInput==="logout"?Concurrency.Bind(Concurrency.Sleep(500),function()
     {
      a.Post({
       $:2,
       $0:user
      });
      user="";
      return Concurrency.Zero();
     }):cmdInput==="subscribe"?Concurrency.Bind(Concurrency.Sleep(500),function()
     {
      a.Post({
       $:3,
       $0:user,
       $1:arg1
      });
      return Concurrency.Zero();
     }):cmdInput==="send"?Concurrency.Bind(Concurrency.Sleep(500),function()
     {
      a.Post({
       $:4,
       $0:user,
       $1:arg1
      });
      return Concurrency.Zero();
     }):cmdInput==="retweet"?Concurrency.Bind(Concurrency.Sleep(500),function()
     {
      a.Post({
       $:5,
       $0:user,
       $1:arg1
      });
      return Concurrency.Zero();
     }):cmdInput==="query"?arg1==="tag"?Concurrency.Bind(Concurrency.Sleep(500),function()
     {
      a.Post({
       $:6,
       $0:user,
       $1:arg2
      });
      return Concurrency.Zero();
     }):arg1==="mentioned"?Concurrency.Bind(Concurrency.Sleep(500),function()
     {
      a.Post({
       $:7,
       $0:user,
       $1:arg2
      });
      return Concurrency.Zero();
     }):arg1==="username"?Concurrency.Bind(Concurrency.Sleep(500),function()
     {
      a.Post({
       $:8,
       $0:user,
       $1:arg2
      });
      return Concurrency.Zero();
     }):Concurrency.Zero():Concurrency.Zero()):flag===0?Concurrency.Bind(Concurrency.Sleep(500),function()
     {
      return Concurrency.Return(null);
     }):Concurrency.Zero();
    }));
   });
  })),null);
  return Doc.Element("div",[],[Doc.Input([],vInput),Doc.Button("Send",[],function()
  {
   submit.Trigger();
  }),Doc.Element("hr",[],[]),Doc.Element("h4",[AttrProxy.Create("class","text-muted")],[Doc.TextNode("The server responded:")]),Doc.Element("div",[AttrProxy.Create("class","jumbotron")],[Doc.Element("h4",[],[Doc.TextView(vTextView)])]),container]);
 };
 Client.Main=function(aboutPageLink,wsep)
 {
  var b,W,_this,p,i;
  return(b=(W=WebSocketClient.WebSocketTest(wsep),(_this=new ProviderBuilder.New$1(),(_this.h.push({
   $:0,
   $0:"websockettest",
   $1:W
  }),_this))),(p=Handler.CompleteHoles(b.k,b.h,[]),(i=new TemplateInstance.New(p[1],ClientServerTest1_Templates.body(p[0])),b.i=i,i))).get_Doc();
 };
 ClientServerTest1_Templates.body=function(h)
 {
  Templates.LoadLocalTemplates("main");
  return h?Templates.NamedTemplate("main",{
   $:1,
   $0:"body"
  },h):void 0;
 };
 ClientServerTest1_JsonEncoder.j=function()
 {
  return ClientServerTest1_JsonEncoder._v?ClientServerTest1_JsonEncoder._v:ClientServerTest1_JsonEncoder._v=(Provider.EncodeUnion(void 0,{
   queriedusername:8,
   mentioned:7,
   tag:6,
   tweetcontent5:5,
   tweetcontent4:4,
   subscribeto3:3,
   username2:2,
   psw1:1,
   psw0:0
  },[["RequestRegister",[["$0","username0",Provider.Id(),0],["$1","psw0",Provider.Id(),0]]],["RequestLogIn",[["$0","username1",Provider.Id(),0],["$1","psw1",Provider.Id(),0]]],["RequestLogOut",[["$0","username2",Provider.Id(),0]]],["WantSubscribe",[["$0","username3",Provider.Id(),0],["$1","subscribeto3",Provider.Id(),0]]],["WantSendTweet",[["$0","username4",Provider.Id(),0],["$1","tweetcontent4",Provider.Id(),0]]],["WantReTweet",[["$0","username5",Provider.Id(),0],["$1","tweetcontent5",Provider.Id(),0]]],["WantQueryByTag",[["$0","username6",Provider.Id(),0],["$1","tag",Provider.Id(),0]]],["WantQueryByMentioned",[["$0","username7",Provider.Id(),0],["$1","mentioned",Provider.Id(),0]]],["WantQueryByUsername",[["$0","username8",Provider.Id(),0],["$1","queriedusername",Provider.Id(),0]]]]))();
 };
 ClientServerTest1_JsonDecoder.j=function()
 {
  return ClientServerTest1_JsonDecoder._v?ClientServerTest1_JsonDecoder._v:ClientServerTest1_JsonDecoder._v=(Provider.DecodeUnion(void 0,"type",[["RegisterSuccess",[["$0","username9",Provider.Id(),0],["$1","psw9",Provider.Id(),0]]],["RegisterFailed",[["$0","username19",Provider.Id(),0]]],["LogInSuccess",[["$0","username10",Provider.Id(),0],["$1","psw10",Provider.Id(),0]]],["LogInFailed",[]],["LogOutSuccess",[["$0","username11",Provider.Id(),0]]],["Subscribing",[["$0","username12",Provider.Id(),0],["$1","subscribeto12",Provider.Id(),0]]],["SubscribeFailed",[["$0","username20",Provider.Id(),0],["$1","subscribeto20",Provider.Id(),0]]],["BeSubscribed",[["$0","subscribeto13",Provider.Id(),0],["$1","username13",Provider.Id(),0]]],["ResponseSendTweet",[["$0","username14",Provider.Id(),0],["$1","tweetcontent14",Provider.Id(),0],["$2","followers14",Provider.DecodeList(Provider.Id()),0]]],["ResponseReTweet",[["$0","username15",Provider.Id(),0],["$1","tweetcontent15",Provider.Id(),0]]],["QueryByTag",[["$0","username16",Provider.Id(),0],["$1","tag16",Provider.Id(),0]]],["QueryByMentioned",[["$0","username17",Provider.Id(),0],["$1","mentioned17",Provider.Id(),0]]],["QueryByUsername",[["$0","username18",Provider.Id(),0],["$1","queriedusername18",Provider.Id(),0]]]]))();
 };
}());
