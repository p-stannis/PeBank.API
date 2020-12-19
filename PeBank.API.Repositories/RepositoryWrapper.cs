using PeBank.API.Contracts;
using PeBank.API.Entities;
using System;

namespace PeBank.API.Repositories
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly AppDbContext _dbContext;
        private IAccountRepository _account;
        private IAccountTypeRepository _accountType;
        private ICustomerRepository _customer;
        private IOperationRepository _operation;
        private ITransactionRepository _transaction;
        private ITransactionTypeRepository _transactionType;

        public RepositoryWrapper(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IAccountRepository Account 
        {
            get
            {
                if (_account == null) _account = new AccountRepository(_dbContext);
                return _account;
            }
        }

        public IAccountTypeRepository AccountType 
        {
            get
            {
                if (_accountType == null) _accountType = new AccountTypeRepository(_dbContext);
                return _accountType;
            }
        }

        public ICustomerRepository Customer
        {
            get
            {
                if (_customer == null) _customer = new CustomerRepository(_dbContext);
                return _customer;
            }
        }

        public IOperationRepository Operation
        {
            get
            {
                if (_operation == null) _operation = new OperationRepository(_dbContext);
                return _operation;
            }
        }


        public ITransactionRepository TransactionRepository
        {
            get
            {
                if (_transaction == null) _transaction = new TransactionRepository(_dbContext);
                return _transaction;
            }
        }

        public ITransactionTypeRepository TransactionTypeRepository 
        {
            get 
            {
                if (_transactionType == null) _transactionType = new TransactionTypeRepository(_dbContext);
                return _transactionType;
            }
        }

        public IDisposable UseTransaction()
        {
            return new UnitOfWork(_dbContext);
        }
    }
}
