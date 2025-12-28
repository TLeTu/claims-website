using Microsoft.EntityFrameworkCore;
using claims_website.Entities;

namespace claims_website.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> user { get; set; }
    public DbSet<Customer> customer { get; set; }
    public DbSet<Policy> policy { get; set; }
    public DbSet<InsuranceClaim> claim { get; set; }
}