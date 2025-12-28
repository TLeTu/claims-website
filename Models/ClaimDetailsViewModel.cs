using System.ComponentModel.DataAnnotations;

namespace claims_website.Models;

public class ClaimDetailsViewModel
{
    // Header
    public string ClaimNo { get; set; } = string.Empty;
    public string? IncidentType { get; set; } // e.g., "Multi-vehicle Collision"
    public DateTime ClaimDate { get; set; }
    public decimal TotalAmount { get; set; }

    // Breakdown (New data!)
    public decimal VehicleCost { get; set; }   // Maps to CSV 'vehicle'
    public decimal InjuryCost { get; set; }    // Maps to CSV 'injury'
    public decimal PropertyCost { get; set; }  // Maps to CSV 'property'

    // Report (New data!)
    public DateTime IncidentDate { get; set; }
    public int IncidentHour { get; set; }
    public string? CollisionType { get; set; }      // e.g., "Side Collision"
    public string? DriverRelationship { get; set; } // e.g., "Own Child"
    public int DriverAge { get; set; }
    public int VehiclesInvolved { get; set; }
    public int Witnesses { get; set; }
    
    // Navigation
    public string PolicyNo { get; set; } = string.Empty;
}