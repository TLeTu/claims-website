using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace claims_website.Entities;

[Table("user", Schema = "demo")]
public class User
{
    // 2. Map C# "UserId" to SQL "user_id"
    [Key]
    [Column("user_id")]
    public int Id { get; set; } // Or public int UserId { get; set; }

    [Column("email")]
    public required string Email { get; set; }

    // 3. Map C# "HashedPassword" (from your log) to SQL "password_hash"
    [Column("password_hash")]
    public required string HashedPassword { get; set; }
    // 4. Map C# "Phone" (from your log) to SQL "phone_number"
    [Column("phone_number")]
    public required string Phone { get; set; }

    [Column("created_at")]
    public required DateTime CreatedAt { get; set; }
}