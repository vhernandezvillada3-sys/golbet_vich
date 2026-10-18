using GolBet.Entities;
using GolBet.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GolBet.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // --- REGISTRO ---

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = new AppUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName,
            Balance = 100000m // Regla de negocio: 100,000 FutCoins de bienvenida
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            // Asignamos el rol de apostador
            await _userManager.AddToRoleAsync(user, "betor");

            // Lo logueamos automáticamente después de registrarse
            await _signInManager.SignInAsync(user, isPersistent: false);

            TempData["Success"] = $"¡Bienvenido a GolBet, {user.FullName}! Tienes 100.000 FutCoins de regalo.";
            return RedirectToAction("Index", "Matches");
        }

        // Si hubo errores (ej. el correo ya existe), los mostramos en la vista
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    // --- LOGIN ---

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Matches");
        }

        ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
        return View(model);
    }

    // --- LOGOUT ---

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}