using Microsoft.EntityFrameworkCore;
using claims_website.Entities;
using claims_website.Data;

namespace claims_website.Repositories;

public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(AppDbContext context) : base(context)
    {
    }

    // Implementing the specific method defined in ICustomerRepository
    public async Task<IEnumerable<Customer>> GetByBoroughAsync(string borough)
    {
        return await _dbSet.Where(c => c.Borough == borough).ToListAsync();
    }
}