using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace claims_website.Entities;

[Table("policy", Schema = "demo")]
public class Policy
{
    [Key]
    [Column("policy_no")]
    public required string PolicyNo { get; set; }
    [Column("cust_id")]
    public required string CustId { get; set; }
    [Column("policytype")]
    public string? PolicyType { get; set; }
    [Column("pol_issue_date")]
    public DateTime? PolIssueDate { get; set; }
    [Column("pol_eff_date")]
    public DateTime? PolEffDate { get; set; }
    [Column("pol_expiry_date")]
    public DateTime? PolExpiryDate { get; set; }
    [Column("make")]
    public string? Make { get; set; }
    [Column("model")]
    public string? Model { get; set; }
    [Column("model_year")]
    public int? ModelYear { get; set; }
    [Column("chassis_no")]
    public string? ChassisNo { get; set; }
    [Column("use_of_vehicle")]
    public string? UseOfVehicle { get; set; }
    [Column("product")]
    public string? Product { get; set; }
    [Column("sum_insured")]
    public decimal? SumInsured { get; set; }
    [Column("premium")]
    public decimal? Premium { get; set; }
    [Column("deductable")]
    public int? Deductable { get; set; }
}