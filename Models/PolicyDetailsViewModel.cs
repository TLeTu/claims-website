using System.ComponentModel.DataAnnotations;

namespace claims_website.Models;

public class PolicyDetailsViewModel
{
    // --- Identifiers ---
    public string PolicyNo { get; set; } = string.Empty;

    // --- The Car (The "Hero" of the card) ---
    [Display(Name = "Vehicle")]
    public string VehicleDescription { get; set; } = string.Empty; // e.g. "2018 Toyota Corolla"
    public string ChassisNo { get; set; } = string.Empty;

    // --- Coverage Info ---
    [Display(Name = "Plan Type")]
    public string Product { get; set; } = string.Empty; // e.g. "Standard Comprehensive"

    [DataType(DataType.Currency)]
    public decimal SumInsured { get; set; }

    [DataType(DataType.Currency)]
    public decimal Premium { get; set; }

    // --- Dates & Status ---
    [DataType(DataType.Date)]
    public DateTime? StartDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }

    // Helper to determine if policy is active
    public bool IsActive => DateTime.Now >= StartDate && DateTime.Now <= EndDate;

    // Helper for CSS badge class
    public string StatusBadgeClass => IsActive ? "bg-success" : "bg-secondary";
    public string StatusText => IsActive ? "Active" : "Expired";

    // --- Detailed Financials ---
    [Display(Name = "Deductible (Excess)")]
    [DataType(DataType.Currency)]
    public decimal? Deductible { get; set; } // e.g., $1,000

    [Display(Name = "Usage Type")]
    public string? Usage { get; set; } // "Private" or "Commercial"

    [Display(Name = "Date Issued")]
    public DateTime? IssueDate { get; set; }

    // --- Related Data ---
    public List<ClaimSummaryViewModel> ClaimsHistory { get; set; } = new();

    // --- Helper for "Download" section ---
    public string PolicyDocumentUrl => $"/documents/{PolicyNo}.pdf";
}