using System.ComponentModel.DataAnnotations;

namespace claims_website.Models;

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

public class ClaimCreateViewModel
{
	[Required]
	[Display(Name = "Policy Number")]
	public string? PolicyNo { get; set; }

	public List<SelectListItem> ActivePolicies { get; set; } = new List<SelectListItem>();

	[Required]
	[DataType(DataType.Date)]
	[Display(Name = "Incident Date")]
	public DateTime? IncidentDate { get; set; }

	[Required]
	[Display(Name = "Time of Incident")]
    public string IncidentTimeStr { get; set; } = "12:00";

	[Required]
	[Display(Name = "Type of Incident")]
	public string? IncidentType { get; set; }

	[Required]
	[Display(Name = "Severity")]
	public string? IncidentSeverity { get; set; }
	[Display(Name = "Collision Type")]
	public string? CollisionType { get; set; }

	[Required]
	[Display(Name = "Driver Relationship")]
	public string? InsuredRelationship { get; set; }
	[Required]
	[Range(16, 99)]
	[Display(Name = "Driver Age")]
	public int? DriverAge { get; set; }

	[Display(Name = "Witnesses")]
	[Range(0, 99)]
	public int? Witnesses { get; set; }

	[Display(Name = "License Input Date")]
	[DataType(DataType.Date)]
	public DateTime? LicenseInputDate { get; set; }

	[Display(Name = "Vehicles Involved")]
	[Range(1, 99)]
	public int? VehiclesInvolved { get; set; }

	[Display(Name = "Upload Photos")]
    public List<IFormFile> ClaimPhotos { get; set; } = new List<IFormFile>();
}