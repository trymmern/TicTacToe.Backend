using System.Text.Json;
using TicTacToe.Api.Data;
using TicTacToe.Api.Data.Models;

namespace TicTacToe.Api.Repository;

public interface IGameRepository
{
    Game? Get(int id);
    IEnumerable<Game> Get();
    Game CreateGame();
    Game UpdateGame(int id, char?[] state);
}

public class GameRepository : IGameRepository
{
    private readonly TicTacToeContext _context;
    
    public GameRepository(TicTacToeContext context)
    {
        _context = context;
    }

    public Game? Get(int id)
    {
        return _context.Games.FirstOrDefault(g => g.Id == id);
    }

    public IEnumerable<Game> Get()
    {
        return _context.Games.ToList();
    }

    public Game CreateGame()
    {
        var id = 0;
        var games = _context.Games.ToList();
        if (games.Count > 0) id = games.Max(g => g.Id) + 1;
        
        var game = new Game
        {
            Id = id,
            States = new List<State> { new() },
            Winner = null
        };
        
        _context.Games.Add(game);
        _context.SaveChanges();

        return game;
    }

    public Game UpdateGame(int id, char?[] state)
    {
        var game = _context.Games
            .ToList()
            .FirstOrDefault(g => g.Id == id);
        
        if (game == null)
        {
            throw new ArgumentException("Game not found");
        }
        
        if (_context.States.ToList().Any(s => s.GameId == id && s.Compare(state)))
        {
            return game;
        }
        
        game.States.Add(new State(state));
        _context.SaveChanges();

        return game;
    }
}