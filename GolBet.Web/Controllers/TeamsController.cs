// GolBet.Web/Controllers/TeamsController.cs
using GolBet.Services.DTOs;
using GolBet.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GolBet.Web.Controllers;

[Authorize(Roles = "admin")] // Restricts access to users with the 'admin' role
public class TeamsController : Controller
{
    private readonly ITeamService _teamService;

    public TeamsController(ITeamService teamService)
        => _teamService = teamService;

    // GET /Teams
    public async Task<IActionResult> Index()
    {
        var teams = await _teamService.GetAllAsync();
        return View(teams);
    }

    // GET /Teams/Create
    public IActionResult Create()
        => View(new TeamFormDto());

    // POST /Teams/Create
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TeamFormDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto); // If validation fails, return the form with errors

        await _teamService.CreateAsync(dto);

        TempData["Success"] = $"Equipo «{dto.Name}» creado correctamente.";

        return RedirectToAction(nameof(Index)); // PRG: Redirect to the list to prevent duplicate submissions
    }

    // GET /Teams/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _teamService.GetForEditAsync(id);
        if (dto is null) return NotFound();

        return View(dto);
    }

    // POST /Teams/Edit
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TeamFormDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        await _teamService.UpdateAsync(dto);

        TempData["Success"] = $"Equipo «{dto.Name}» actualizado.";

        return RedirectToAction(nameof(Index));
    }

    // POST /Teams/Deactivate/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(int id)
    {
        await _teamService.DeactivateAsync(id);

        TempData["Success"] = "Equipo desactivado.";

        return RedirectToAction(nameof(Index));
    }
}