using System.ComponentModel.DataAnnotations;

namespace claims_website.Models;

public class ClaimViewModel
{
    // --- Identity ---
    public string ClaimNo { get; set; } = String.Empty;
    public string PolicyNo { get; set; } = String.Empty;

    // --- The Car (Joined from Policy Table) ---
    public string? VehicleDescription { get; set; } // e.g. "2018 Toyota Corolla"

    // --- The Incident ---
    [Display(Name = "Incident Date")]
    [DataType(DataType.Date)]
    public DateTime IncidentDate { get; set; }

    public string? IncidentType { get; set; } // e.g. "Side Collision"
    
    // --- Status & Money ---
    [DataType(DataType.Currency)]
    public decimal TotalAmount { get; set; }
    
    public string? Status { get; set; } // "Paid", "In Review", "Rejected"

    // Helper for Badge Color
    public string StatusBadgeClass => Status switch
    {
        "Paid" => "bg-success",
        "In Review" => "bg-warning text-dark",
        "Rejected" => "bg-danger",
        _ => "bg-secondary"
    };
}