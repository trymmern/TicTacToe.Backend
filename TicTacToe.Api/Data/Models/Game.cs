using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicTacToe.Api.Data.Models;

[Table("Games")]
public sealed class Game : BaseEntity
{
    public Game()
    {
        States = new List<State>();
    }
    public char ?Winner { get; set; }
    public List<State> States { get; set; }
}