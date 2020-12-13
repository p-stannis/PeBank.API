using PeBank.API.Contracts;
using PeBank.API.Entities;

namespace PeBank.API.Repositories
{
    public class TransactionTypeRepository : RepositoryBase<TransactionType>, ITransactionTypeRepository
    {
        public TransactionTypeRepository(AppDbContext dbContext) : base(dbContext)
        {
               
        }
    }
}
