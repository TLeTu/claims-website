using Microsoft.EntityFrameworkCore;
using claims_website.Entities;
using claims_website.Data;

namespace claims_website.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    // You only need to implement the specific methods here.
    // The standard CRUD is already handled by BaseRepository!
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }
}