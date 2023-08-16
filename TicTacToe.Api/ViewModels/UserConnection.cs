using System.Net.WebSockets;

namespace TicTacToe.Api.ViewModels;

public class UserConnection
{
    public required User User { get; init; }
    public required WebSocket Connection { get; init; }
}