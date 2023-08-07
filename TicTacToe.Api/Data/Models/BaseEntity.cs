using System.ComponentModel.DataAnnotations;

namespace TicTacToe.Api.Data.Models;

public abstract class BaseEntity
{
    [Key]
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = "";
    public string UpdatedBy { get; set; } = "";
}