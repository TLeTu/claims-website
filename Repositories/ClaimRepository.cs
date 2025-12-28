using Microsoft.EntityFrameworkCore;
using claims_website.Entities;
using claims_website.Data;

namespace claims_website.Repositories;

public class ClaimRepository : BaseRepository<InsuranceClaim>, IClaimRepository
{
    public ClaimRepository(AppDbContext context) : base(context)
    {
    }

    //get by id but the id is claim no which is varchar
    public async Task<InsuranceClaim?> GetByClaimNoAsync(string claimNo)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.ClaimNo == claimNo);
    }

    // Implementing the specific method defined in IClaimRepository
    public async Task<IEnumerable<InsuranceClaim>> GetByCustomerIdAsync(string customerId)
    {
        return await _dbSet
            .FromSqlRaw(@"
                SELECT c.*
                FROM demo.claim c
                JOIN demo.policy p ON c.policy_no = p.policy_no
                WHERE p.cust_id = {0}", customerId)
            .ToListAsync();
    }

    public async Task<IEnumerable<InsuranceClaim>> GetByPolicyNoAsync(string policyNo)
    {
        return await _dbSet
            .Where(c => c.PolicyNo == policyNo)
            .ToListAsync();
    }
}