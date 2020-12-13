using System;

namespace PeBank.API.Contracts
{
    public interface IRepositoryWrapper
    {
        IAccountRepository Account { get; }
        IAccountTypeRepository AccountType { get; }
        ICustomerRepository Customer { get; }
        ITransactionRepository TransactionRepository { get; }
        ITransactionTypeRepository TransactionTypeRepository { get; }
        IDisposable UseTransaction();
    }
}
