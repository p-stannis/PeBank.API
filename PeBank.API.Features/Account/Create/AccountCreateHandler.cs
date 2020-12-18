using AutoMapper;
using MediatR;
using PeBank.API.Contracts;
using PeBank.API.Entities;
using PeBank.API.Features.Utils.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace PeBank.API.Features
{
    public class AccountCreateHandler : IRequestHandler<AccountCreateRequest, AccountModel>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repository;

        public AccountCreateHandler(IMapper mapper, IRepositoryWrapper repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public Task<AccountModel> Handle(AccountCreateRequest request, CancellationToken cancellationToken)
        {
            ValidateIfAccountExists(request);

            ValidateIfCustomerExists(request);

            ValidateAccountType(request);

            var accountToCreate = new Account
            {
                AccountTypeId = (int)request.AccountTypeId,
                CurrentBalance = 0,
                CustomerId = (int)request.CustomerId
            };

            using (_repository.UseTransaction())
            {
                _repository.Account.Create(accountToCreate);
            }

            var result = _mapper.Map<AccountModel>(accountToCreate);

            return Task.FromResult(result);
        }

        private void ValidateIfAccountExists(AccountCreateRequest request)
        {
            var existingAccount = _repository.Account.FindSingle(a => a.AccountTypeId == request.AccountTypeId && a.CustomerId == request.CustomerId);

            if (existingAccount != null)
            {
                throw new BusinessException("Customer has an existing account for this type.");
            }
        }

        private void ValidateAccountType(AccountCreateRequest request)
        {
            var accountType = _repository.AccountType.FindById((int)request.AccountTypeId);

            if (accountType == null)
            {
                throw new BusinessException("Account type does not exist");
            }
        }

        private void ValidateIfCustomerExists(AccountCreateRequest request)
        {
            var customer = _repository.Customer.FindById((int)request.CustomerId);

            if (customer == null)
                throw new BusinessException("Account can only be created with an existing customer");
        }
    }
}
