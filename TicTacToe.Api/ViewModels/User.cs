using System.Text.Json.Serialization;

namespace TicTacToe.Api.ViewModels;

public class User
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    [JsonPropertyName("name")]
    public required string Name { get; set; }
}