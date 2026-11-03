using GolBet.Entities;
using GolBet.Entities.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GolBet.Repositories.Data;

public static class DbSeeder
{
    // Constants to avoid typos in roles
    public const string AdminRole = "Admin";
    public const string BettorRole = "Apostador";

    public static async Task SeedAsync(
        AppDbContext context,
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // Apply pending migrations
        await context.Database.MigrateAsync();

        // 1. Seed Identity (Roles and Admin User)
        await SeedIdentityAsync(userManager, roleManager);

        // 2. Seed Domain (Teams and Matches) - Idempotence guard
        if (await context.Teams.AnyAsync()) return;

        // ---- Teams ----
        var teams = new List<Team>
        {
            new() { Name = "Atlético Nacional",       City = "Medellín",     CrestUrl = "https://placehold.co/80x80/006633/ffffff?text=NAC" },
            new() { Name = "Independiente Medellín",  City = "Medellín",     CrestUrl = "https://placehold.co/80x80/cc0000/ffffff?text=DIM" },
            new() { Name = "Millonarios",             City = "Bogotá",       CrestUrl = "https://placehold.co/80x80/003399/ffffff?text=MIL" },
            new() { Name = "Independiente Santa Fe",  City = "Bogotá",       CrestUrl = "https://placehold.co/80x80/cc0000/ffffff?text=SFE" },
            new() { Name = "América de Cali",         City = "Cali",         CrestUrl = "https://placehold.co/80x80/e60000/ffffff?text=AME" },
            new() { Name = "Deportivo Cali",          City = "Cali",         CrestUrl = "https://placehold.co/80x80/00794d/ffffff?text=CAL" },
            new() { Name = "Junior de Barranquilla",  City = "Barranquilla", CrestUrl = "https://placehold.co/80x80/d40026/ffffff?text=JUN" },
            new() { Name = "Once Caldas",             City = "Manizales",    CrestUrl = "https://placehold.co/80x80/ffffff/000000?text=ONC" }
        };

        context.Teams.AddRange(teams);
        await context.SaveChangesAsync();

        // ---- Matches ----
        var today = DateTime.UtcNow.Date;
        var matches = new List<Match>
        {
            new() { HomeTeamId = teams[0].Id, AwayTeamId = teams[1].Id, Date = today.AddDays(3).AddHours(20), Status = MatchStatus.Scheduled, HomeOdds = 2.10m, DrawOdds = 3.20m, AwayOdds = 3.60m },
            new() { HomeTeamId = teams[2].Id, AwayTeamId = teams[3].Id, Date = today.AddDays(5).AddHours(18), Status = MatchStatus.Scheduled, HomeOdds = 2.45m, DrawOdds = 3.00m, AwayOdds = 2.95m },
            new() { HomeTeamId = teams[4].Id, AwayTeamId = teams[5].Id, Date = today.AddDays(7).AddHours(19), Status = MatchStatus.Scheduled, HomeOdds = 2.30m, DrawOdds = 3.10m, AwayOdds = 3.15m },
            new() { HomeTeamId = teams[6].Id, AwayTeamId = teams[7].Id, Date = today.AddDays(10).AddHours(16), Status = MatchStatus.Scheduled, HomeOdds = 1.85m, DrawOdds = 3.40m, AwayOdds = 4.20m },
            new() { HomeTeamId = teams[3].Id, AwayTeamId = teams[6].Id, Date = DateTime.UtcNow.AddHours(-1), Status = MatchStatus.InProgress, HomeOdds = 2.60m, DrawOdds = 3.05m, AwayOdds = 2.80m },
            new() { HomeTeamId = teams[1].Id, AwayTeamId = teams[2].Id, Date = today.AddDays(-4).AddHours(20), Status = MatchStatus.Finished, HomeGoals = 2, AwayGoals = 1, HomeOdds = 2.75m, DrawOdds = 3.10m, AwayOdds = 2.70m }
        };

        context.Matches.AddRange(matches);
        await context.SaveChangesAsync();
    }

    private static async Task SeedIdentityAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // 1. Create roles if they don't exist
        string[] roleNames = { AdminRole, BettorRole };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // 2. Create Admin User
        string adminEmail = "admin@golbet.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        // If user doesn't exist, create it
        if (adminUser == null)
        {
            adminUser = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Administrador del Sistema",
                Balance = 0m
            };

            await userManager.CreateAsync(adminUser, "Admin123");
        }

        // CRITICAL FIX: Always check if the user has the role. If not, assign it.
        // This fixes the issue where the user was created but the role assignment failed previously.
        if (!await userManager.IsInRoleAsync(adminUser, AdminRole))
        {
            await userManager.AddToRoleAsync(adminUser, AdminRole);
        }
    }
}