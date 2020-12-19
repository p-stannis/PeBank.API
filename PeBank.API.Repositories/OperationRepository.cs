using PeBank.API.Contracts;
using PeBank.API.Entities;

namespace PeBank.API.Repositories
{
    public class OperationRepository : RepositoryBase<Operation>, IOperationRepository
    {
        public OperationRepository(AppDbContext dbContext) : base(dbContext)
        {

        }
    }
}
