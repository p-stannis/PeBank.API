using PeBank.API.Contracts;
using PeBank.API.Entities;

namespace PeBank.API.Repositories
{
    public class TransactionRepository : RepositoryBase<Transaction>, ITransactionRepository
    {
        public TransactionRepository(AppDbContext dbContext) : base(dbContext)
        {

        }
    }
}
