using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using TicTacToe.Api.ViewModels;

namespace TicTacToe.Api.WebSockets;

public class WebSocketHandler
{
    public Dictionary<string, UserConnection> Connections { get; } = new();
    private List<UserActivity> UserActivity { get; } = new();

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
        foreach (var (_, connection) in Connections)
        {
            if (connection.Connection.State != WebSocketState.Open) continue;
            var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
            await connection.Connection.SendAsync(arraySegment,
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
    }

    public async Task HandleUserEvent(JsonDocument json, WebSocket connection)
    {
        var userProp = json.RootElement.GetProperty("user");
        var user = JsonSerializer.Deserialize<User>(userProp.GetString() ??
                                                    throw new InvalidOperationException(
                                                        "User object needs to be present"));

        if (user == null) throw new InvalidOperationException("Invalid user object in message");

        var userConnection = new UserConnection { Connection = connection, User = user };

        if (Connections.TryAdd(user.Id, userConnection))
        {
            AddJoinActivity(user);

            var joinData = JsonSerializer.Serialize(new
                { userActivity = UserActivity, eventType = EventTypes.ActivityEvent });
            await Broadcast(joinData);
        }
        else
        {
            throw new InvalidOperationException("User already connected or could not add for some reason");
        }
    }

    public GameViewModel HandleGameEvent(JsonDocument json)
    {
        throw new NotImplementedException();
    }

    private void AddJoinActivity(User user)
    {
        var (joinMsg, countMsg) = ("joined the room", $"{Connections.Count} users connected");
        UserActivity.AddRange(new []
        {
            new UserActivity { UserId = user.Id, Username = user.Name, Message = joinMsg }, 
            new UserActivity { Message = countMsg }
        });
    }
}