using System.ComponentModel.DataAnnotations;

namespace claims_website.Models;

public class ClaimSummaryViewModel 
{
    public required string ClaimNo { get; set; }
    public DateTime? IncidentDate { get; set; }
    public string? Type { get; set; } // e.g., "Side Collision"
    public string? Severity { get; set; } // e.g., "Major Damage"
    public decimal? TotalAmount { get; set; }
}