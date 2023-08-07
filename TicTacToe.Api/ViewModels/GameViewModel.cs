using System.Text.Json;
using TicTacToe.Api.Data.Models;

namespace TicTacToe.Api.ViewModels;

public class GameViewModel
{
    public GameViewModel(Game game)
    {
        Id = game.Id;
        Winner = game.Winner;
        States = ToCharList(game.States);
    }
    public int Id { get; set; }
    public char ?Winner { get; set; }
    public List<char?[]> States { get; set; }

    private static List<char?[]> ToCharList(IEnumerable<State> states)
    {
        return states.Select(state => JsonSerializer.Deserialize<char?[]>(state.Json) ?? new char?[9]).ToList();
    }
}