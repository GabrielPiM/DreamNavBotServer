using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DibraServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Runtime;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Channels;
using System.Net.Sockets;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

BotServer.print("Made by discord: VivoDibra#1182");
BotServer.print("Starting...");
var wsOptions = new WebSocketOptions { KeepAliveInterval = TimeSpan.FromSeconds(120) };
app.UseWebSockets(wsOptions);

var socketMananger = new SocketMananger();

app.Use(async (HttpContext context, Func<Task> next) =>
{
    try
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            using (WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync())
            {
                await SendToAll(webSocket);
            }
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }

        await next();
    }
    catch (Exception)
    {
        //silence...
    }

});

async Task SendToAll(WebSocket webSocket)
{
    var buffer = new byte[1024 * 4];
    WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
    string name = "";
    string channel = "";

    while (!result.CloseStatus.HasValue)
    {
        try
        {
            string message = Encoding.UTF8.GetString(new ArraySegment<byte>(buffer, 0, result.Count));
            var myObject = JsonConvert.DeserializeAnonymousType(message, new { type = "", name = "", channel = "", topic = "" });

            if (myObject != null)
            {
                //Initialize
                if (myObject.type == "init")
                {
                    name = myObject.name;
                    channel = myObject.channel;
                    socketMananger.addSocket(webSocket, channel);
                    BotServer.print(name + " has connected to channel: " + channel);
                }
                //Already initialized
                else if (name != "")
                {
                    // Its a ping, ping just that connection
                    if (string.Equals(myObject.type, "ping"))
                    {
                        JObject pingMens = new JObject
                        {
                            { "_keepAlive_", "" }
                        };

                        await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(pingMens))), result.MessageType, result.EndOfMessage, CancellationToken.None);

                    }
                    //Broadcast message to all connections in the channel.
                    else
                    {
                        JObject jsonObj = JsonConvert.DeserializeObject<JObject>(message);
                        jsonObj.Add("name", name);
                        var tasks = new List<Task>();
                        foreach (var socket in socketMananger.getChannelConnections(channel))
                        {
                            tasks.Add(socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jsonObj))), result.MessageType, result.EndOfMessage, CancellationToken.None));
                        }
                        await Task.WhenAll(tasks);
                    }
                }
            }

            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        }
        catch (Exception)
        {
            //silence...
        }
    }

    BotServer.print(name + " has desconnected.");
    socketMananger.removeSocket(webSocket, channel);
    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);

}

BotServer.print("Bot Server is running!");

app.Run();