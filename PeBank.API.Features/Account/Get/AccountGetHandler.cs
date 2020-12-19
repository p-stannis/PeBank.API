using AutoMapper;
using MediatR;
using PeBank.API.Contracts;
using PeBank.API.Entities;
using PeBank.API.Features.Utils.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PeBank.API.Features
{
    public class AccountGetHandler : IRequestHandler<AccountGetRequest, AccountModel>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repository;

        public AccountGetHandler(IMapper mapper, IRepositoryWrapper repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public Task<AccountModel> Handle(AccountGetRequest request, CancellationToken cancellationToken)
        {
            Account account = GetAccount(request);

            ValidateRequestCustomer(request, account);

            var result = _mapper.Map<AccountModel>(account);

            result.Balance = result.Transactions.Sum(b => b.Ammount);

            return Task.FromResult(result);
        }

        private static void ValidateRequestCustomer(AccountGetRequest request, Account account)
        {
            if (account.CustomerId != request.CustomerId)
            {
                throw new BusinessException("This account does not belong to the customer requested.");
            }
        }

        private Account GetAccount(AccountGetRequest request)
        {
            var account = _repository.Account.FindById(request.AccountId, new List<string> { "Transactions" , "Transactions.TransactionType" });

            if (account == null)
            {
                throw new NotFoundException($"Account {request.AccountId} does not exist");
            }

            return account;
        }
    }
}
