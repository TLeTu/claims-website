using claims_website.Entities;

namespace claims_website.Repositories;

public interface IClaimRepository : IRepository<InsuranceClaim>
{
    // Add specific methods not in the generic CRUD here
    Task<InsuranceClaim?> GetByClaimNoAsync(string claimNo);
    Task<IEnumerable<InsuranceClaim>> GetByCustomerIdAsync(string customerId);
    Task<IEnumerable<InsuranceClaim>> GetByPolicyNoAsync(string policyNo);
}