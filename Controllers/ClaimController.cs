using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using claims_website.Repositories;
using claims_website.Models;
using System.Security.Claims;

namespace claims_website.Controllers;

public class ClaimController : Controller
{
	private readonly ILogger<ClaimController> _logger;
	private readonly IUserRepository _userRepo;
	private readonly IPolicyRepository _policyRepo;
	private readonly IClaimRepository _claimRepo;

	public ClaimController(
		ILogger<ClaimController> logger,
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
	public async Task<IActionResult> Claim()
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

		var claims = await _claimRepo.GetByCustomerIdAsync(user.CustomerId.Value.ToString());

		var claimViewModels = claims.Select(c => new ClaimViewModel
		{
			ClaimNo = c.ClaimNo,
			PolicyNo = c.PolicyNo,
			VehicleDescription = "",
			IncidentDate = c.IncidentDate ?? DateTime.MinValue,
			IncidentType = c.CollisionType,
			TotalAmount = c.Total ?? 0,
			Status = "Pending"
		}).ToList();

		return View(claimViewModels);
	}

	[Authorize]
	[HttpGet]
	public async Task<IActionResult> ClaimDetails(string id)
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

		var claim = await _claimRepo.GetByClaimNoAsync(id);
		if (claim == null)
		{
			return NotFound();
		}

		var policy = await _policyRepo.GetByPolicyNoAsync(claim.PolicyNo);
		if (policy == null || policy.CustId != user.CustomerId.Value.ToString())
		{
			return Forbid();
		}

		var model = new ClaimDetailsViewModel
		{
			ClaimNo = claim.ClaimNo,
			IncidentType = claim.IncidentType,
			ClaimDate = claim.ClaimDate ?? DateTime.MinValue,
			TotalAmount = claim.Total ?? 0,
			VehicleCost = claim.Vehicle ?? 0,
			InjuryCost = claim.Injury ?? 0,
			PropertyCost = claim.Property ?? 0,
			IncidentDate = claim.IncidentDate ?? DateTime.MinValue,
			IncidentHour = claim.IncidentHour ?? 0,
			CollisionType = claim.CollisionType,
			DriverRelationship = claim.InsuredRelationship,
			DriverAge = claim.DriverAge ?? 0,
			VehiclesInvolved = claim.NumberOfVehiclesInvolved ?? 0,
			Witnesses = claim.NumberOfWitnesses ?? 0,
			PolicyNo = claim.PolicyNo
		};

		return View(model);
	}

	public async Task<IActionResult> Create()
	{
		return View();
	}
}
