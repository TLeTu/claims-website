using claims_website.Entities;

namespace claims_website.Repositories;
public interface ICustomerRepository : IRepository<Customer>
{
    // Add specific methods not in the generic CRUD here
    Task<IEnumerable<Customer>> GetByBoroughAsync(string borough);
}