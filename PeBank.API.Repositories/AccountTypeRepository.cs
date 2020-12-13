using PeBank.API.Contracts;
using PeBank.API.Entities;

namespace PeBank.API.Repositories
{
    public class AccountTypeRepository : RepositoryBase<AccountType>, IAccountTypeRepository
    {
        public AccountTypeRepository(AppDbContext dbContext): base(dbContext)
        {
               
        }
    }
}
