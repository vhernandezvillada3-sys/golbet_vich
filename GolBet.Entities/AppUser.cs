// GolBet.Entities/AppUser.cs
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GolBet.Entities;

public class AppUser : IdentityUser
{
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = null!;

    public decimal Balance { get; set; }

    // Relación: Un usuario tiene muchas apuestas
    public ICollection<Bet> Bets { get; set; } = new List<Bet>();
}