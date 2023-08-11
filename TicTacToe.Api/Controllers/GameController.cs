using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Repository;
using TicTacToe.Api.ViewModels;

namespace TicTacToe.Api.Controllers;

[Controller]
[Route("Games")]
public class GameController
{
    private readonly IGameRepository _gameRepository;
    
    public GameController(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    [HttpGet("")]
    public IResult Get()
    {
        var games = _gameRepository.Get();
        var gameVms = games.Select(game => new GameViewModel(game)).ToList().OrderBy(g => g.Id);

        return Results.Ok(JsonSerializer.Serialize(gameVms));
    }

    [HttpGet("{id:int}")]
    public IResult Get(int id)
    {
        var game = _gameRepository.Get(id);
        
        return game != null ? Results.Ok(JsonSerializer.Serialize(new GameViewModel(game))) : Results.Empty;
    }
    
    [HttpPost("")]
    public IResult CreateGame()
    {
        return Results.Ok(JsonSerializer.Serialize(new GameViewModel(_gameRepository.CreateGame())));
    }

    [HttpPost("Update")]
    public IResult UpdateGame(int id, [FromBody] UpdateModel body)
    {
        var state = new char?[9];
        if (body.State.Length != 9)
        {
            return Results.BadRequest("State array needs to have a length of exactly 9");
        }

        for (var i = 0; i < state.Length; i++)
        {
            if (char.TryParse(body.State[i], out var charVal)) state[i] = charVal;
            else state[i] = null;
        }

        char? winnerChar = null;
        if (body.Winner != null)
        {
            winnerChar = char.Parse(body.Winner);
        }
        return Results.Ok(new GameViewModel(_gameRepository.UpdateGame(id, state, winnerChar)));
        
    }

    [HttpDelete("")]
    public IResult Delete(int id)
    {
        var games = _gameRepository.Delete(id);
        var gameVms = games.Select(game => new GameViewModel(game)).ToList().OrderBy(g => g.Id);
        return Results.Ok(JsonSerializer.Serialize(gameVms));
    }
}