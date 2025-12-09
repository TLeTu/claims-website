using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using claims_website.Models;
using claims_website.Repositories;
using claims_website.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using Microsoft.AspNetCore.Identity;

namespace claims_website.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly IPasswordHasher<object> _passwordHasher;
    private readonly IUserRepository _userRepo;

    public AccountController(
        ILogger<AccountController> logger,
        IPasswordHasher<object> passwordHasher,
        IUserRepository userRepo)
    {
        _logger = logger;
        _passwordHasher = passwordHasher;
        _userRepo = userRepo;
    }

    public IActionResult Login()
    {
        return View();
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RegisterPost(RegisterViewModel model)
    {
        _logger.LogInformation("RegisterPost hit. Email: {Email}", model.Email);

        if (!ModelState.IsValid)
        {
            return View("Register", model);
        }

        var existingUser = await _userRepo.GetByEmailAsync(model.Email);
        if (existingUser != null)
        {
            ModelState.AddModelError("Email", "This email is already registered.");
            return View("Register", model);
        }

        string hashedPassword = _passwordHasher.HashPassword(model.Email, model.Password);

        var newUser = new User
        {
            Email = model.Email,
            Phone = model.Phone,
            HashedPassword = hashedPassword,
            CreatedAt = DateTime.UtcNow,
        };

        await _userRepo.AddAsync(newUser);

        _logger.LogInformation("User registered successfully via Repository.");

        return RedirectToAction("Home", "Index");
    }

    [HttpPost]
    public async Task<IActionResult> LoginPost(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View("Login", model);

        var user = await _userRepo.GetByEmailAsync(model.Email);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View("Login", model);
        }

        var result = _passwordHasher.VerifyHashedPassword(model.Email, user.HashedPassword, model.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View("Login", model);
        }

        // 3. Create the "Claims" (The data inside the wristband)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email), // Defines HttpContext.User.Identity.Name
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("UserId", user.Id.ToString()) // Custom claim for ID
            // new Claim(ClaimTypes.Role, "Admin") // You can add roles here later
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true // Keep user logged in even after closing browser
        };

        // 4. Sign In (This generates the encrypted cookie)
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        _logger.LogInformation("User {Email} logged in.", user.Email);
        

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}