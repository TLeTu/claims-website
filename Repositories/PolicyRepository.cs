using Microsoft.EntityFrameworkCore;
using claims_website.Entities;
using claims_website.Data;

namespace claims_website.Repositories;

public class PolicyRepository : BaseRepository<Policy>, IPolicyRepository
{
    public PolicyRepository(AppDbContext context) : base(context)
    {
    }

    // Implementing the specific method defined in IPolicyRepository
    public async Task<IEnumerable<Policy>> GetByCustomerIdAsync(string customerId)
    {
        return await _dbSet.Where(p => p.CustId == customerId).ToListAsync();
    }
}