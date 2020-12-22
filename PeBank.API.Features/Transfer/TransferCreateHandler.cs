using MediatR;
using PeBank.API.Contracts;
using PeBank.API.Entities;
using PeBank.API.Features.Utils.Exceptions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PeBank.API.Features
{
    public class TransferCreateHandler : IRequestHandler<TransferCreateRequest, IEnumerable<TransactionModel>>
    {
        private readonly IMediator _mediator;
        private readonly IRepositoryWrapper _repository;

        public TransferCreateHandler(IMediator mediator, IRepositoryWrapper repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        public async Task<IEnumerable<TransactionModel>> Handle(TransferCreateRequest request, CancellationToken cancellationToken)
        {
            if (request.AccountId == request.RecipientAccountId)
            {
                throw new BusinessException("You cannot transfer to your own account.");
            }

            var transferTransactionType = _repository.TransactionTypeRepository.FindSingle(tt => tt.Code == "T");

            AccountModel accountFrom = await GetAccount(request.AccountId.Value, request.CustomerId, cancellationToken);

            ValidateAmmountToBeTransfered(request, transferTransactionType, accountFrom);

            AccountModel accountTo = await GetAccount(request.RecipientAccountId, request.RecipientCustomerId, cancellationToken);

            IEnumerable<TransactionModel> transferTransactions = await CreateTransferTransaction(request, accountFrom, accountTo, transferTransactionType);

            return transferTransactions;
        }

        private static void ValidateAmmountToBeTransfered(TransferCreateRequest request, Entities.TransactionType transferTransactionType, AccountModel accountFrom)
        {
            var ammountToBeTransfered = request.Ammount + transferTransactionType.FixedCharge.Value;

            var balanceAfterTransfer = accountFrom.Balance - ammountToBeTransfered;

            if (balanceAfterTransfer < 0)
            {
                throw new BusinessException("Your account cannot have a negative balance.");
            }
        }

        private async Task<AccountModel> GetAccount(int accountId, int customerId, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new AccountGetRequest { AccountId = accountId, CustomerId = customerId }, cancellationToken);
        }

        private async Task<IEnumerable<TransactionModel>> CreateTransferTransaction(
            TransferCreateRequest request,
            AccountModel accountFrom,
            AccountModel accountTo,
            TransactionType transferTransactionType)
        {
            List<TransactionModel> transactionsToCreate = BuildTransferTransactions(request, accountFrom, accountTo, transferTransactionType);

            var transactions = await _mediator.Send(new TransactionCreateRequest
            {
                Transactions = transactionsToCreate,
                OperationDetails = $"Transfer from Account {accountFrom.Id} to Account {accountTo.Id}. " +
                                   $"Ammount of ${request.Ammount} "
            });

            return transactions;
        }

        public List<TransactionModel> BuildTransferTransactions(TransferCreateRequest request, AccountModel accountFrom, AccountModel accountTo, TransactionType transferTransactionType)
        {
            var transferAccountFromTransaction = new TransactionModel
            {
                AccountId = accountFrom.Id,
                Ammount = -request.Ammount,
                Details = request.Details,
                TransactionTypeId = transferTransactionType.Id
            };

            var transferSurcharge = new TransactionModel
            {
                AccountId = accountFrom.Id,
                Ammount = -transferTransactionType.FixedCharge.Value,
                Details = $"Bank transfer surcharge of ${transferTransactionType.FixedCharge.Value}",
                TransactionTypeId = transferTransactionType.Id
            };

            var transferAccountToTransaction = new TransactionModel
            {
                AccountId = accountTo.Id,
                Ammount = request.Ammount,
                Details = request.Details,
                TransactionTypeId = transferTransactionType.Id
            };

            return new List<TransactionModel>
            {
                transferAccountFromTransaction,
                transferSurcharge,
                transferAccountToTransaction
            };
        }
    }
}
