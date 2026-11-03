// GolBet.Web/Controllers/BetsController.cs
using GolBet.Entities;
using GolBet.Services.DTOs;
using GolBet.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GolBet.Web.Controllers;

[Authorize(Roles = "Apostador")] // Only bettors can access this controller
public class BetsController : Controller
{
    private readonly IBetService _betService;
    private readonly UserManager<AppUser> _userManager;

    public BetsController(IBetService betService, UserManager<AppUser> userManager)
    {
        _betService = betService;
        _userManager = userManager;
    }

    // POST /Bets/Place
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Place(BetFormDto dto)
    {
        // Identity from the cookie (never from the form!)
        var userId = _userManager.GetUserId(User)!;

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Revisa el monto de la apuesta.";
            return RedirectToAction("Detail", "Matches", new { id = dto.MatchId });
        }

        try
        {
            await _betService.PlaceBetAsync(userId, dto);
            TempData["Success"] = $"¡Apuesta registrada! {dto.Amount:N0} FutCoins en juego. Suerte 🍀";
        }
        catch (InvalidOperationException ex) // Business rule violated
        {
            TempData["Error"] = ex.Message;
        }

        // Always redirect back to the match detail (PRG pattern)
        return RedirectToAction("Detail", "Matches", new { id = dto.MatchId });
    }

    // GET /Bets/MyBets
    public async Task<IActionResult> MyBets()
    {
        var userId = _userManager.GetUserId(User)!;
        var bets = await _betService.GetMyBetsAsync(userId);
        return View(bets);
    }
}