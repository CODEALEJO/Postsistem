using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Postsistem.Data;
using Postsistem.Models;
using System.Security.Claims;

[Authorize]
public class AccountController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<AccountController> _logger;
    private readonly ApplicationDbContext _context;

    public AccountController(
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        ILogger<AccountController> logger,
        ApplicationDbContext context)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
        _context = context;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _signInManager.PasswordSignInAsync(
            model.Email, model.Password, model.RememberMe, false);

        if (result.Succeeded)
            return RedirectToAction("Index", "Inventario");

        ModelState.AddModelError("", "Credenciales inválidas");
        return View(model);
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Register()
    {
        ViewBag.Locales = new SelectList(_context.Locales, "Id", "Nombre");
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Locales = new SelectList(_context.Locales, "Id", "Nombre");
            return View(model);
        }

        var user = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            ViewBag.Locales = new SelectList(_context.Locales, "Id", "Nombre");
            return View(model);
        }

        // RELACIÓN USUARIO - LOCAL
        _context.UsuarioLocales.Add(new UsuarioLocal
        {
            UserId = user.Id,
            LocalId = model.LocalId
        });

        await _context.SaveChangesAsync();

        await _signInManager.SignInAsync(user, false);
        return RedirectToAction("Index", "Inventario");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }

    // MÉTODO CLAVE
    protected int ObtenerLocalDelUsuario()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return _context.UsuarioLocales
            .Where(u => u.UserId == userId)
            .Select(u => u.LocalId)
            .First();
    }
}
