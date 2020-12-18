using AutoMapper;
using MediatR;
using PeBank.API.Contracts;
using PeBank.API.Entities;
using PeBank.API.Features.Utils.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PeBank.API.Features
{
    public class TransactionCreateHandler : IRequestHandler<TransactionCreateRequest, TransactionModel>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repository;

        public TransactionCreateHandler(IMapper mapper, IRepositoryWrapper repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public Task<TransactionModel> Handle(TransactionCreateRequest request, CancellationToken cancellationToken)
        {
            var transactionTypes = ListTransactionTypes(request);

            Account account = GetAccount(request);

            if(request.TransactionTypeId == 1) // Deposit
            {
                var transactionType = transactionTypes.First(tt => tt.Id == request.TransactionTypeId);

                var decimalValueToCharge = transactionType.PercentCharge / 100;

                var ammountToBeDeposited = request.Ammount - (decimalValueToCharge * request.Ammount);

                account.CurrentBalance += ammountToBeDeposited.Value;
            }

            if (request.TransactionTypeId == 2) // Withdraw
            {

            }

            if (request.TransactionTypeId == 3) // Transfer
            {

            }

            var newTransaction = new Transaction 
            {
                Account = account,
                AccountId = account.Id,
                Ammount = request.Ammount,
                Date = DateTime.Now,
                Details = request.Details,
                TransactionTypeId = (int)request.TransactionTypeId
            };

            using (_repository.UseTransaction()) 
            {
                _repository.TransactionRepository.Create(newTransaction);
            }

            var result = _mapper.Map<TransactionModel>(newTransaction);

            return Task.FromResult(result);
        }

        private Account GetAccount(TransactionCreateRequest request)
        {
            var account = _repository.Account.FindSingle(a => a.Id == request.AccountId);

            if (account == null)
            {
                throw new BusinessException("Account does not exist");
            }

            return account;
        }

        private IEnumerable<TransactionType> ListTransactionTypes(TransactionCreateRequest request)
        {
            var transactionTypes = _repository.TransactionTypeRepository.FindAll();

            ValidateTransactionTypeFromRequest(request, transactionTypes);

            return transactionTypes;
        }

        private static void ValidateTransactionTypeFromRequest(TransactionCreateRequest request, IEnumerable<TransactionType> transactionTypes)
        {
            var transactionTypeExist = transactionTypes.Any(tt => tt.Id == request.TransactionTypeId);

            if (!transactionTypeExist)
            {
                throw new BusinessException("Transaction type does not exist");
            }
        }
    }
}
