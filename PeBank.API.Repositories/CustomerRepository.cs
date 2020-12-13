using PeBank.API.Contracts;
using PeBank.API.Entities;

namespace PeBank.API.Repositories
{
    public class CustomerRepository : RepositoryBase<Customer>, ICustomerRepository
    {
        public CustomerRepository(AppDbContext dbContext ) : base(dbContext)
        {

        }
    }
}
