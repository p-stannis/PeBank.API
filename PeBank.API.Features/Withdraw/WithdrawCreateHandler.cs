using MediatR;
using PeBank.API.Contracts;
using PeBank.API.Entities;
using PeBank.API.Features.Utils.Exceptions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PeBank.API.Features
{
    public class WithdrawCreateHandler : IRequestHandler<WithdrawCreateRequest, IEnumerable<TransactionModel>>
    {
        private readonly IMediator _mediator;
        private readonly IRepositoryWrapper _repository;

        public WithdrawCreateHandler(IMediator mediator, IRepositoryWrapper repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        public async Task<IEnumerable<TransactionModel>> Handle(WithdrawCreateRequest request, CancellationToken cancellationToken)
        {
            var withdrawTransactionType = _repository.TransactionTypeRepository.FindSingle(tt => tt.Code == "W");

            AccountModel account = await GetAccount(request, cancellationToken);

            ValidateWithdrawal(request, account, withdrawTransactionType);

            IEnumerable<TransactionModel> withdrawTransaction = await CreateWithdrawalTransaction(request, account, withdrawTransactionType);

            return withdrawTransaction;
        }

        private async Task<AccountModel> GetAccount(WithdrawCreateRequest request, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new AccountGetRequest { AccountId = request.AccountId.Value, CustomerId = request.CustomerId }, cancellationToken);
        }

        private async Task<IEnumerable<TransactionModel>> CreateWithdrawalTransaction(WithdrawCreateRequest request, AccountModel account, TransactionType withdrawalTransactionType)
        {
            List<TransactionModel> transactionsToCreate = BuildWithdrawalTransactions(request, account, withdrawalTransactionType);

            var transactions = await _mediator.Send(new TransactionCreateRequest
            {
                Transactions = transactionsToCreate,
                OperationDetails = $"Withdrawal: ammount ${request.Ammount} "
            });

            return transactions;
        }

        public List<TransactionModel> BuildWithdrawalTransactions(WithdrawCreateRequest request, AccountModel account, TransactionType withdrawalTransactionType)
        {
            var withdrawalTransaction = new TransactionModel
            {
                AccountId = account.Id,
                Ammount = -request.Ammount,
                Details = request.Details,
                TransactionTypeId = withdrawalTransactionType.Id
            };

            var withdrawalSurcharge = new TransactionModel
            {
                AccountId = account.Id,
                Ammount = -withdrawalTransactionType.FixedCharge.Value,
                Details = $"Bank withdrawal surcharge of ${withdrawalTransactionType.FixedCharge.Value}",
                TransactionTypeId = withdrawalTransactionType.Id
            };

            return new List<TransactionModel> { withdrawalTransaction, withdrawalSurcharge };
        }

        private static void ValidateWithdrawal(WithdrawCreateRequest request, AccountModel account, TransactionType transactionType)
        {
            var ammountToWithdraw = request.Ammount + transactionType.FixedCharge.Value;

            var balanceAfterWithdrawal = account.Balance - ammountToWithdraw;

            if (balanceAfterWithdrawal < 0)
            {
                throw new BusinessException("Your account cannot have a negative balance.");
            }
        }
    }
}
