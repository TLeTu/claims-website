using System.ComponentModel.DataAnnotations;

namespace claims_website.Models;

public class AccountDetailsViewModel
{
    // --- User Info (Read Only mostly) ---
    public int UserId { get; set; }
    
    public string Email { get; set; } = string.Empty;
    
    public string Phone { get; set; } = string.Empty;
    
    [Display(Name = "Account Created")]
    public DateTime CreatedAt { get; set; }

    // --- Customer Info (Editable) ---
    public int CustomerId { get; set; } // Defaults to 0 if no customer linked

    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    public DateTime? DateOfBirth { get; set; } // Must be nullable (?)

    public string? Borough { get; set; }
    
    public string? Neighborhood { get; set; }
    
    [Display(Name = "Zip Code")]
    public string? ZipCode { get; set; }
    
    [Display(Name = "Full Name")]
    public string? Name { get; set; }
}