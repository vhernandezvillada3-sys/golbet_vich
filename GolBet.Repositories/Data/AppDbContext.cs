// GolBet.Repositories/Data/AppDbContext.cs
using GolBet.Entities;
using GolBet.Entities.Common;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GolBet.Repositories.Data;

// AHORA HEREDA DE IdentityDbContext<AppUser>
public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<Bet> Bets => Set<Bet>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1. OBLIGATORIO: Llamar al padre primero para crear las tablas de Identity
        base.OnModelCreating(modelBuilder);

        // --- Configuración de Equipos (Módulo 2) ---
        modelBuilder.Entity<Team>()
            .Property(t => t.Name)
            .UseCollation("SQL_Latin1_General_CP1_CI_AI");

        modelBuilder.Entity<Team>()
            .HasIndex(t => t.Name)
            .IsUnique();

        // --- Configuración de Partidos (Módulo 2) ---
        modelBuilder.Entity<Match>()
            .HasOne(m => m.HomeTeam)
            .WithMany()
            .HasForeignKey(m => m.HomeTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Match>()
            .HasOne(m => m.AwayTeam)
            .WithMany()
            .HasForeignKey(m => m.AwayTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        // --- Configuración de Apuestas y Usuarios (Módulo 7) ---
        modelBuilder.Entity<Bet>()
            .HasOne(b => b.Match)
            .WithMany(m => m.Bets)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación con el Usuario
        modelBuilder.Entity<Bet>()
            .HasOne(b => b.User)
            .WithMany(u => u.Bets)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Regla de negocio: Un usuario no puede repetir apuesta en el mismo partido
        modelBuilder.Entity<Bet>()
            .HasIndex(b => new { b.MatchId, b.UserId, b.Pick })
            .IsUnique();

        // Precisión para el saldo del usuario
        modelBuilder.Entity<AppUser>()
            .Property(u => u.Balance)
            .HasPrecision(12, 2);
    }

    // ---- Automatic audit timestamps ----
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedDate = utcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.ModifiedDate = utcNow;
                    entry.Property(e => e.CreatedDate).IsModified = false;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}