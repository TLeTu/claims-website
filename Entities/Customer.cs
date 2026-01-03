using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace claims_website.Entities;

[Table("customer", Schema = "demo")]
public class Customer
{
    [Key]
    [Column("customer_id")]
    public required int Id { get; set; }

    [Column("date_of_birth")]
    public DateTime? DateOfBirth { get; set; } // Nullable because DB allows NULL

    [Column("borough")]
    public string? Borough { get; set; } // Nullable string

    [Column("neighborhood")]
    public string? Neighborhood { get; set; }

    [Column("zip_code")]
    public string? ZipCode { get; set; }

    [Column("name")]
    public string? Name { get; set; }
}