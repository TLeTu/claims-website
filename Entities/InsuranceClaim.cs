// CREATE TABLE IF NOT EXISTS demo.claim (
//     claim_no                        VARCHAR(50)    NOT NULL PRIMARY KEY,
//     policy_no                       VARCHAR(50)    NOT NULL,   
//     claim_date                      DATE,           -- Use DATE type
//     months_as_customer              INT,
//     injury                          BIGINT,
//     property                        BIGINT,
//     vehicle                         BIGINT,
//     total                           BIGINT,
//     collision_type                  VARCHAR(50),
//     number_of_vehicles_involved     INT, 
//     driver_age                      INT,            -- Age is typically an integer
//     insured_relationship            VARCHAR(50),
//     license_issue_date              DATE,           -- Use DATE type
//     incident_date                   DATE,           -- Use DATE type
//     incident_hour                   INT,
//     incident_type                   VARCHAR(50),
//     incident_severity               VARCHAR(50),
//     number_of_witnesses             INT, 
//     suspicious_activity             BOOLEAN         -- Use BOOLEAN for true/false
// );

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace claims_website.Entities;

[Table("claim", Schema = "demo")]
public class InsuranceClaim
{
    [Key]
    [Column("claim_no")]
    public required string ClaimNo { get; set; }

    [Column("policy_no")]
    public required string PolicyNo { get; set; }

    [Column("claim_date")]
    public DateTime? ClaimDate { get; set; }

    [Column("months_as_customer")]
    public int? MonthsAsCustomer { get; set; }

    [Column("injury")]
    public long? Injury { get; set; }

    [Column("property")]
    public long? Property { get; set; }

    [Column("vehicle")]
    public long? Vehicle { get; set; }

    [Column("total")]
    public long? Total { get; set; }

    [Column("collision_type")]
    public string? CollisionType { get; set; }

    [Column("number_of_vehicles_involved")]
    public int? NumberOfVehiclesInvolved { get; set; }

    [Column("driver_age")]
    public int? DriverAge { get; set; }

    [Column("insured_relationship")]
    public string? InsuredRelationship { get; set; }

    [Column("license_issue_date")]
    public DateTime? LicenseIssueDate { get; set; }

    [Column("incident_date")]
    public DateTime? IncidentDate { get; set; }

    [Column("incident_hour")]
    public int? IncidentHour { get; set; }

    [Column("incident_type")]
    public string? IncidentType { get; set; }

    [Column("incident_severity")]
    public string? IncidentSeverity { get; set; }

    [Column("number_of_witnesses")]
    public int? NumberOfWitnesses { get; set; }

    [Column("suspicious_activity")]
    public bool? SuspiciousActivity { get; set; }

}