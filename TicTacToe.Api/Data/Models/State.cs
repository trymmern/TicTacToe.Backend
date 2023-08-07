using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace TicTacToe.Api.Data.Models;

[Table("States")]
public class State : BaseEntity
{
    public string Json { get; set; }
    public int GameId { get; set; }
    public virtual Game Game {get; set; }
    
    // Initialize a null-state (starting state)
    public State()
    {
        Json = JsonSerializer.Serialize(new char?[9]);
    }

    // Initialize a state with char array as input
    public State(char?[] state)
    {
        Json = JsonSerializer.Serialize(state);
    }
    
}