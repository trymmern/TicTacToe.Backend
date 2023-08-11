using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.WebSockets;

namespace TicTacToe.Api.Controllers;

public class WebSocketController : ControllerBase
{
    private WebSocketHandler WsHandler { get; }
    
    public WebSocketController(WebSocketHandler wsHandler)
    {
        WsHandler = wsHandler;
    }
    
    [Route("/ws")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await WsHandler.Echo(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
    
    [Route("/ws/connect")]
    public async Task Connect()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            var curName = HttpContext.Request.Query["name"];
            using var ws = await HttpContext.WebSockets.AcceptWebSocketAsync();
            if (!WsHandler.Connections.Contains(ws))
            {
                WsHandler.Connections.Add(ws);
            }

            var stream = new StreamReader(HttpContext.Request.Body);
            var body = await stream.ReadToEndAsync();
            await WsHandler.Broadcast($"{curName} joined the room");
            await WsHandler.Broadcast($"{WsHandler.Connections.Count} users connected");
            await WsHandler.ReceiveMessage(ws,
                async (result, buffer) =>
                {
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        await WsHandler.Broadcast($"{curName}: {message}");
                    }
                    else if (result.MessageType == WebSocketMessageType.Close || ws.State == WebSocketState.Aborted)
                    {
                        WsHandler.Connections.Remove(ws);
                        await WsHandler.Broadcast($"{curName} left the room");
                        await WsHandler.Broadcast($"{WsHandler.Connections.Count} users connected");
                        await ws.CloseAsync(result.CloseStatus.Value, 
                            result.CloseStatusDescription,
                            CancellationToken.None);
                    }
                });
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}