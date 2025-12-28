using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using claims_website.Models;
using claims_website.Repositories;
using claims_website.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Identity;

namespace claims_website.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly IPasswordHasher<object> _passwordHasher;
    private readonly IUserRepository _userRepo;
    private readonly ICustomerRepository _customerRepo;
    private readonly IPolicyRepository _policyRepo;
    private readonly IClaimRepository _claimRepo;

    public AccountController(
        ILogger<AccountController> logger,
        IPasswordHasher<object> passwordHasher,
        IUserRepository userRepo,
        ICustomerRepository customerRepo,
        IPolicyRepository policyRepo,
        IClaimRepository claimRepo)
    {
        _logger = logger;
        _passwordHasher = passwordHasher;
        _userRepo = userRepo;
        _customerRepo = customerRepo;
        _policyRepo = policyRepo;
        _claimRepo = claimRepo;
    }

    public IActionResult Login()
    {
        return View();
    }

    public IActionResult Register()
    {
        return View();
    }

    [Authorize] // Ensure only logged-in users can hit this
    [HttpGet]
    public async Task<IActionResult> AccountDetails()
    {
        // 1. Get UserId safely from the cookie
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return RedirectToAction("Login");
        }

        // 2. Get User Entity
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        // 3. Get Customer Entity (SAFELY)
        // We create a variable for customer and default it to null.
        claims_website.Entities.Customer? customer = null;

        // Check if the User actually has a linked CustomerId in the DB
        // (Assuming your User entity has: public int? CustomerId { get; set; })
        if (user.CustomerId.HasValue)
        {
            customer = await _customerRepo.GetByIdAsync(user.CustomerId.Value);
        }

        // 4. Map to ViewModel
        var model = new AccountDetailsViewModel
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            Phone = user.Phone ?? string.Empty,
            CreatedAt = user.CreatedAt,

            // --- Safe Mapping with Null Coalescing ---

            // If customer is null, ID is 0
            CustomerId = customer?.Id ?? 0,

            // If customer is null, DateOfBirth is null
            DateOfBirth = customer?.DateOfBirth,

            // If customer is null OR column is null, use empty string
            Borough = customer?.Borough ?? string.Empty,
            Neighborhood = customer?.Neighborhood ?? string.Empty,
            ZipCode = customer?.ZipCode ?? string.Empty,
            Name = customer?.Name ?? string.Empty
        };

        return View(model);
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
        var existingUserByPhone = await _userRepo.GetByPhoneAsync(model.Phone);
        if (existingUserByPhone != null)
        {
            ModelState.AddModelError("Phone", "This phone number is already registered.");
            return View("Register", model);
        }

        string hashedPassword = _passwordHasher.HashPassword(model.Email, model.Password);

        // 1. Create a new Customer with empty columns
        var newCustomer = new Customer
        {
            // All fields left as default/null/empty
        };
        await _customerRepo.AddAsync(newCustomer);

        // 2. Create the new User and assign the CustomerId
        var newUser = new User
        {
            Email = model.Email,
            Phone = model.Phone,
            HashedPassword = hashedPassword,
            CreatedAt = DateTime.UtcNow,
            CustomerId = newCustomer.Id // assign the new Customer's ID
        };

        await _userRepo.AddAsync(newUser);

        _logger.LogInformation("User registered successfully via Repository.");

        return RedirectToAction("Index", "Home");
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