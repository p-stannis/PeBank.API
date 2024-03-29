﻿using MediatR;
using PeBank.API.Contracts;
using PeBank.API.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PeBank.API.Features
{
    public class DepositCreateHandler : IRequestHandler<DepositCreateRequest, IEnumerable<TransactionModel>>
    {
        private readonly IMediator _mediator;
        private readonly IRepositoryWrapper _repository;

        public DepositCreateHandler(IMediator mediator, IRepositoryWrapper repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        public async Task<IEnumerable<TransactionModel>> Handle(DepositCreateRequest request, CancellationToken cancellationToken)
        {
            var depositTransactionType = _repository.TransactionTypeRepository.FindSingle(tt => tt.Code == "D");

            AccountModel account = await GetAccount(request, cancellationToken);

            IEnumerable<TransactionModel> depositTransaction = await CreateDepositTransaction(request, account, depositTransactionType);

            return depositTransaction;
        }

        private async Task<IEnumerable<TransactionModel>> CreateDepositTransaction(DepositCreateRequest request, AccountModel account, TransactionType depositTransactionType)
        {
            List<TransactionModel> depositTransactions = BuildDepositTransaction(request, account, depositTransactionType);

            var transactions = await _mediator.Send(new TransactionCreateRequest
            {
                Transactions = depositTransactions,
                OperationDetails = $"Deposit: ammount {request.Ammount} "
            });

            return transactions;
        }

        public List<TransactionModel> BuildDepositTransaction(DepositCreateRequest request, AccountModel account, TransactionType depositTransactionType)
        {
            var decimalValueToCharge = depositTransactionType.PercentCharge.Value / 100;

            var depositTransaction = new TransactionModel
            {
                AccountId = account.Id,
                Ammount = request.Ammount,
                Details = request.Details,
                TransactionTypeId = depositTransactionType.Id
            };

            var depositSurcharge = new TransactionModel
            {
                AccountId = account.Id,
                Ammount = -(decimalValueToCharge * request.Ammount),
                Details = $"Bank deposit surcharge of {depositTransactionType.PercentCharge}%",
                TransactionTypeId = depositTransactionType.Id
            };

            return new List<TransactionModel> { depositTransaction, depositSurcharge };

        }

        private async Task<AccountModel> GetAccount(DepositCreateRequest request, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new AccountGetRequest { AccountId = request.AccountId.Value, CustomerId = request.CustomerId }, cancellationToken);
        }

    }
}
