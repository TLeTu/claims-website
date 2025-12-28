using claims_website.Entities;

namespace claims_website.Repositories;

public interface IPolicyRepository : IRepository<Policy>
{
    // Add specific methods not in the generic CRUD here
    Task<IEnumerable<Policy>> GetByCustomerIdAsync(string customerId);
    Task<Policy?> GetByPolicyNoAsync(string policyNo);
}