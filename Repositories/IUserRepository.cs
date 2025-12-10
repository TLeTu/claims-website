
using claims_website.Entities;

namespace claims_website.Repositories;

public interface IUserRepository : IRepository<User>
{
    // Add specific methods not in the generic CRUD here
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByPhoneAsync(string phone);
}