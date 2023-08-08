using Microsoft.EntityFrameworkCore;
using TicTacToe.Api.Data.Models;

namespace TicTacToe.Api.Data;

public class TicTacToeContext : DbContext
{
    public DbSet<Game> Games { get; set; } = null!;
    public DbSet<State> States { get; set; } = null!;

    public TicTacToeContext(DbContextOptions<TicTacToeContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<State>(entity =>
        {
            entity.HasOne(s => s.Game)
                .WithMany(g => g.States)
                .HasForeignKey(s => s.GameId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Status_Game");
        });
    }

}