using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using claims_website.Repositories;
using claims_website.Models;
using System.Security.Claims;
using claims_website.Entities;
using Amazon.S3;
using Amazon.S3.Model;
using System.Text;

namespace claims_website.Controllers;

public class ClaimController : Controller
{
	private readonly ILogger<ClaimController> _logger;
	private readonly IUserRepository _userRepo;
	private readonly IPolicyRepository _policyRepo;
	private readonly IClaimRepository _claimRepo;
	private readonly IAmazonS3 _s3Client;

	public ClaimController(
		ILogger<ClaimController> logger,
		IUserRepository userRepo,
		IPolicyRepository policyRepo,
		IClaimRepository claimRepo,
		IAmazonS3 s3Client)
	{
		_logger = logger;
		_userRepo = userRepo;
		_policyRepo = policyRepo;
		_claimRepo = claimRepo;
		_s3Client = s3Client;
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
			ClaimNo = c.ClaimNo ?? "",
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
			ClaimNo = claim.ClaimNo ?? "",
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

	[Authorize]
	[HttpGet]
	public async Task<IActionResult> Create(string? policyNo)
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
		var model = new ClaimCreateViewModel
		{
			ActivePolicies = policies.Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
			{
				Value = p.PolicyNo,
				Text = $"{p.PolicyNo} - {p.Make} {p.Model} ({p.ModelYear})",
				Selected = policyNo != null && p.PolicyNo == policyNo
			}).ToList(),
			PolicyNo = policyNo
		};
		return View(model);
	}

	[Authorize]
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(ClaimCreateViewModel model)
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
		if (!ModelState.IsValid)
		{
			var customerPolicies = await _policyRepo.GetByCustomerIdAsync(user.CustomerId.Value.ToString());
			model.ActivePolicies = customerPolicies.Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
			{
				Value = p.PolicyNo,
				Text = $"{p.PolicyNo} - {p.Make} {p.Model} ({p.ModelYear})",
				Selected = model.PolicyNo != null && p.PolicyNo == model.PolicyNo
			}).ToList();
			return View(model);
		}
		int incidentHour = 0;
		if (DateTime.TryParse(model.IncidentTimeStr, out DateTime parsedTime))
		{
			incidentHour = parsedTime.Hour;
		}
		// 1. Find the specific policy
		var policies = await _policyRepo.GetByCustomerIdAsync(user.CustomerId.Value.ToString());
		var selectedPolicy = policies.FirstOrDefault(p => p.PolicyNo == model.PolicyNo);

		// 2. Calculate Months
		int monthsLoyal = 0;
		if (selectedPolicy != null && selectedPolicy.PolEffDate.HasValue) // Assuming 'PolEffDate' exists on Policy entity
		{
			var start = selectedPolicy.PolEffDate.Value;
			var now = DateTime.Now;
			// Simple math: ((Year Diff) * 12) + Month Diff
			monthsLoyal = ((now.Year - start.Year) * 12) + now.Month - start.Month;
			if (monthsLoyal < 0) monthsLoyal = 0;
		}

		// Auto generate a new claim number
		var generatedClaimNo = Guid.NewGuid().ToString();
		var newClaim = new InsuranceClaim
		{
			ClaimNo = generatedClaimNo,
			PolicyNo = model.PolicyNo!,
			IncidentDate = model.IncidentDate.HasValue ? DateTime.SpecifyKind(model.IncidentDate.Value, DateTimeKind.Utc) : null,
			IncidentType = model.IncidentType,
			IncidentHour = incidentHour,
			IncidentSeverity = model.IncidentSeverity,
			CollisionType = model.CollisionType,
			InsuredRelationship = model.InsuredRelationship,
			DriverAge = model.DriverAge,
			NumberOfWitnesses = model.Witnesses,
			LicenseIssueDate = model.LicenseInputDate.HasValue ? DateTime.SpecifyKind(model.LicenseInputDate.Value, DateTimeKind.Utc) : null,
			NumberOfVehiclesInvolved = model.VehiclesInvolved,
			MonthsAsCustomer = monthsLoyal,
			Injury = 0,
			Property = 0,
			Vehicle = 0,
			Total = 0,
			SuspiciousActivity = false,
			// Set ClaimDate to now (when claim is created)
			ClaimDate = DateTime.UtcNow
		};
		await _claimRepo.AddAsync(newClaim);

		if (model.ClaimPhotos != null && model.ClaimPhotos.Count > 0)
		{
			try
			{
				// A. Get Chassis Number
				var userPolicies = await _policyRepo.GetByCustomerIdAsync(user.CustomerId.Value.ToString());
				var policy = userPolicies.FirstOrDefault(p => p.PolicyNo == model.PolicyNo);
				string chassisNo = policy?.ChassisNo ?? "UNKNOWN";
				string bucketName = "car-smart-claims";

				// B. Prepare the CSV Builder (Header Row)
				var csvBuilder = new StringBuilder();
				csvBuilder.AppendLine("image_name,image_id,claim_no,chassis_no");

				// C. Loop through images
				foreach (var file in model.ClaimPhotos)
				{
					if (file.Length > 0)
					{
						var imageId = Guid.NewGuid().ToString();
						var safeFileName = Path.GetFileName(file.FileName);

						// --- UPLOAD 1: The Image ---
						var imageKey = $"landing/customer/claims/images/{generatedClaimNo}_{imageId}_{safeFileName}";
						using (var stream = file.OpenReadStream())
						{
							var putRequest = new PutObjectRequest
							{
								BucketName = bucketName,
								Key = imageKey,
								InputStream = stream,
								ContentType = file.ContentType
							};
							await _s3Client.PutObjectAsync(putRequest);
						}

						// --- COLLECT DATA (Don't upload CSV yet) ---
						// Append the row to our builder
						csvBuilder.AppendLine($"{safeFileName},{imageId},{generatedClaimNo},{chassisNo}");
					}
				}

				// --- UPLOAD 2: The MASTER Metadata CSV (Once) ---
				// Name it using the Claim ID so it's easy to find
				var metadataKey = $"landing/customer/claims/metadata/{generatedClaimNo}_metadata.csv";
				var metadataBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());

				using (var metaStream = new MemoryStream(metadataBytes))
				{
					var putMetaRequest = new PutObjectRequest
					{
						BucketName = bucketName,
						Key = metadataKey,
						InputStream = metaStream,
						ContentType = "text/csv"
					};
					await _s3Client.PutObjectAsync(putMetaRequest);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error uploading photos/metadata to S3.");
			}
		}

		return RedirectToAction("Claim");
	}
}
