using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TicTacToe.Api.Data;
using TicTacToe.Api.Data.Models;

namespace TicTacToe.Api.Repository;

public interface IGameRepository
{
    Game? Get(int id);
    IEnumerable<Game> Get();
    Game CreateGame();
    Game UpdateGame(int id, char?[] state, char? winner);
    IEnumerable<Game> Delete(int id);
}

public class GameRepository : IGameRepository
{
    private readonly TicTacToeContext _db;
    
    public GameRepository(TicTacToeContext db)
    {
        _db = db;
    }

    public Game? Get(int id)
    {
        return _db.Games
            .Include(g => g.States)
            .FirstOrDefault(g => g.Id == id);
    }

    public IEnumerable<Game> Get()
    {
        return _db.Games
            .Include(g => g.States.OrderBy(s => s.Id))
            .OrderBy(g => g.Id)
            .ToList();
    }

    public Game CreateGame()
    {
        var id = 0;
        var games = _db.Games.ToList();
        if (games.Count > 0) id = games.Max(g => g.Id) + 1;
        
        var game = new Game
        {
            Id = id,
            States = new List<State> { new() },
            Winner = null
        };
        
        _db.Games.Add(game);
        _db.SaveChanges();

        return game;
    }

    public Game UpdateGame(int id, char?[] state, char? winner)
    {
        var game = _db.Games
            .ToList()
            .FirstOrDefault(g => g.Id == id);
        
        if (game == null)
        {
            throw new ArgumentException("Game not found");
        }
        
        if (_db.States.ToList().Any(s => s.GameId == id && s.Compare(state)))
        {
            return game;
        }
        
        game.States.Add(new State(state));
        game.Winner = winner;
        
        _db.SaveChanges();

        return game;
    }

    public IEnumerable<Game> Delete(int id)
    {
        var toDelete = _db.Games.FirstOrDefault(g => g.Id == id);

        if (toDelete == null)
        {
            throw new InvalidOperationException($"Game with id {id} does not exist. Cannot delete it");
        }

        var statesToDelete = _db.States.Where(s => s.GameId == id).ToList();
        foreach (var state in statesToDelete)
        {
            _db.States.Remove(state);
        }

        _db.Games.Remove(toDelete);

        return _db.Games
            .Include(g => g.States)
            .ToList();
    }
}