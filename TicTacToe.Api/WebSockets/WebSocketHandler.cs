using System.Net.WebSockets;
using System.Text;

namespace TicTacToe.Api.WebSockets;

public class WebSocketHandler
{
    public List<WebSocket> Connections { get; } = new();
    
    public async Task Echo(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!receiveResult.CloseStatus.HasValue)
        {
            await webSocket.SendAsync(
                new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                receiveResult.MessageType,
                receiveResult.EndOfMessage,
                CancellationToken.None);
        
            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        await webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);
    }

    public async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
    {
        var buffer = new byte[1024 * 4];
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(
                new ArraySegment<byte>(buffer),
                CancellationToken.None);
            handleMessage(result, buffer);
        }
        
        
    }

    public async Task Broadcast(string message)
    {
        var bytes = Encoding.UTF8.GetBytes(message);
        foreach (var connection in Connections)
        {
            if (connection.State != WebSocketState.Open) continue;
            var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
            await connection.SendAsync(arraySegment,
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
    }
}