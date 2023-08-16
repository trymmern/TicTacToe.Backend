using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
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
    
    [Route("/ws/connect")]
    public async Task Connect()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            string? userId = HttpContext.Request.Query["userId"];
            if (userId == null)
            {
                throw new ArgumentException("A user id must be present in the query string");
            }
            
            using var ws = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await WsHandler.ReceiveMessage(ws,
                async (result, buffer) =>
                {
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var jsonMsg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        var document = JsonDocument.Parse(jsonMsg);
                        var evt = document.RootElement.GetProperty("type").ToString();

                        if (evt == EventTypes.UserEvent) await WsHandler.HandleUserEvent(document, ws);
                        else if (evt == EventTypes.GameEvent) WsHandler.HandleGameEvent(document);
                        else {
                            Console.WriteLine($"Unknown event {evt}");
                            //TODO: reply to client that sent the message, and only to them, that the event was not recognized
                        }
                    }
                    else if (result.MessageType == WebSocketMessageType.Close || ws.State == WebSocketState.Aborted)
                    {
                        if (WsHandler.Connections.TryGetValue(userId, out var userConnection))
                        {
                            
                            WsHandler.Connections.Remove(userId);
                            await WsHandler.Broadcast($"{userConnection.User.Name} left the room");
                            await WsHandler.Broadcast($"{WsHandler.Connections.Count} users connected");
                        }

                        if (result.CloseStatus != null)
                            await ws.CloseAsync(result.CloseStatus.Value,
                                result.CloseStatusDescription,
                                CancellationToken.None);
                    }
                }
            );
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}