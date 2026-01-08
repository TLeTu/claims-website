using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using claims_website.Repositories;
using claims_website.Models;
using System.Security.Claims;

namespace claims_website.Controllers;

public class PolicyController : Controller
{
	private readonly ILogger<PolicyController> _logger;
	private readonly IUserRepository _userRepo;
	private readonly IPolicyRepository _policyRepo;
	private readonly IClaimRepository _claimRepo;

	public PolicyController(
		ILogger<PolicyController> logger,
		IUserRepository userRepo,
		IPolicyRepository policyRepo,
		IClaimRepository claimRepo)
	{
		_logger = logger;
		_userRepo = userRepo;
		_policyRepo = policyRepo;
		_claimRepo = claimRepo;
	}

	[Authorize]
	[HttpGet]
	public async Task<IActionResult> Policy()
	{
		var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
		if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
		{
			return RedirectToAction("Login", "Account");
		}

		var user = await _userRepo.GetByIdAsync(userId);
		if (user == null || !user.CustomerId.HasValue)
		{
			return RedirectToAction("Login", "Account");
		}

		var policies = await _policyRepo.GetByCustomerIdAsync(user.CustomerId.Value.ToString());

		var policyViewModels = policies.Select(p => new PolicyViewModel
		{
			PolicyNo = p.PolicyNo,
			VehicleDescription = $"{(p.ModelYear?.ToString() ?? "").Trim()} {(p.Make ?? "").Trim()} {(p.Model ?? "").Trim()}".Trim(),
			ChassisNo = p.ChassisNo ?? string.Empty,
			Product = p.Product ?? string.Empty,
			SumInsured = p.SumInsured ?? 0,
			Premium = p.Premium ?? 0,
			StartDate = p.PolEffDate,
			EndDate = p.PolExpiryDate,
		})
		.OrderByDescending(vm => vm.IsActive)
		.ThenBy(vm => vm.StartDate)
		.ToList();

		return View(policyViewModels);
	}

	[Authorize]
	[HttpGet]
	public async Task<IActionResult> PolicyDetails(string id)
	{
		var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
		if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
		{
			return RedirectToAction("Login", "Account");
		}

		var user = await _userRepo.GetByIdAsync(userId);
		if (user == null || !user.CustomerId.HasValue)
		{
			return RedirectToAction("Login", "Account");
		}

		var policy = await _policyRepo.GetByPolicyNoAsync(id);
		if (policy == null)
		{
			return NotFound();
		}

		if (policy.CustId != user.CustomerId.Value.ToString())
		{
			return Forbid();
		}

		var claims = await _claimRepo.GetByPolicyNoAsync(policy.PolicyNo);

		var model = new PolicyDetailsViewModel
		{
			PolicyNo = policy.PolicyNo,
			VehicleDescription = $"{(policy.ModelYear?.ToString() ?? "").Trim()} {(policy.Make ?? "").Trim()} {(policy.Model ?? "").Trim()}".Trim(),
			ChassisNo = policy.ChassisNo ?? string.Empty,
			Product = policy.Product ?? string.Empty,
			SumInsured = policy.SumInsured ?? 0,
			Premium = policy.Premium ?? 0,
			StartDate = policy.PolEffDate,
			EndDate = policy.PolExpiryDate,
			Deductible = policy.Deductable,
			Usage = policy.UseOfVehicle,
			IssueDate = policy.PolIssueDate,
			ClaimsHistory = claims.Select(c => new ClaimSummaryViewModel
			{
				ClaimNo = c.ClaimNo ?? 	string.Empty,
				IncidentDate = c.IncidentDate,
				Type = c.CollisionType,
				Severity = c.IncidentSeverity,
				TotalAmount = c.Total
			}).ToList()
		};

		return View(model);
	}
}
