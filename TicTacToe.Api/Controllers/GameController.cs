using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Repository;
using TicTacToe.Api.ViewModels;

namespace TicTacToe.Api.Controllers;

[Controller]
public class GameController
{
    private readonly IGameRepository _gameRepository;
    
    public GameController(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    [HttpGet("Games")]
    public IResult Get()
    {
        var games = _gameRepository.Get();
        var gameVms = games.Select(game => new GameViewModel(game)).ToList().OrderBy(g => g.Id);

        return Results.Ok(JsonSerializer.Serialize(gameVms));
    }

    [HttpGet("Games/{id:int}")]
    public IResult Get(int id)
    {
        var game = _gameRepository.Get(id);
        
        return game != null ? Results.Ok(JsonSerializer.Serialize(new GameViewModel(game))) : Results.Empty;
    }
    
    [HttpPost("/Create")]
    public IResult CreateGame()
    {
        return Results.Ok(JsonSerializer.Serialize(new GameViewModel(_gameRepository.CreateGame())));
    }

    [HttpPost("/Update")]
    public IResult UpdateGame(int id, [FromBody] string?[] stateStr)
    {
        var state = new char?[9];
        if (stateStr.Length != 9)
        {
            return Results.BadRequest("State array needs to have a length of exactly 9");
        }

        for (var i = 0; i < state.Length; i++)
        {
            if (char.TryParse(stateStr[i], out var charVal)) state[i] = charVal;
            else state[i] = null;
        }
        return Results.Ok(new GameViewModel(_gameRepository.UpdateGame(id, state)));
    }
}