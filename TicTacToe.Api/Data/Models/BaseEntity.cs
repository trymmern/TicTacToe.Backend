using System.ComponentModel.DataAnnotations;

namespace TicTacToe.Api.Data.Models;

public interface IBaseEntity
{
    public int Id { get; init; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
}
public abstract class BaseEntity : IBaseEntity
{
    [Key]
    public int Id { get; init; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = "";
    public string UpdatedBy { get; set; } = "";
}