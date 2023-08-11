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

    public override int SaveChanges()
    {
        ChangeTracker.DetectChanges();
        var added = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added)
            .Select(e => e.Entity)
            .ToArray();
        foreach (var e in added)
        {
            if (e is not IBaseEntity entity) continue;
            entity.CreatedDate = DateTime.UtcNow;
            entity.CreatedBy = Environment.UserName;
        }

        var modified = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified)
            .Select(e => e.Entity)
            .ToArray();
        foreach (var e in modified)
        {
            if (e is not IBaseEntity entity) continue;
            entity.UpdatedDate = DateTime.UtcNow;
            entity.UpdatedBy = Environment.UserName;
        }

        return base.SaveChanges();
    }
}