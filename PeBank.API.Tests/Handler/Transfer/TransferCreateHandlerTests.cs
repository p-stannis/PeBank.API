using MediatR;
using Moq;
using PeBank.API.Contracts;
using PeBank.API.Entities;
using PeBank.API.Features;
using PeBank.API.Features.Utils.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;


namespace PeBank.API.Tests.Handler
{
    public class TransferCreateHandlerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<IRepositoryWrapper> _mockRepo;
        public TransferCreateHandlerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _mockRepo = new Mock<IRepositoryWrapper>();
        }

        [Fact]
        public async Task Handle_CreateTransferSameAccount_ShouldReturnBusinessException()
        {
            var handler = new TransferCreateHandler(_mockMediator.Object, _mockRepo.Object);

            var request = new TransferCreateRequest { AccountId = 1, CustomerId = 1, Ammount = 100, RecipientAccountId = 1, RecipientCustomerId = 2 };
            
            await Assert.ThrowsAsync<BusinessException>(() => handler.Handle(request, default));
        }

        [Fact]
        public async Task Handle_CreateTransferNegativeBalance_ShouldReturnBusinessException()
        {
            var handler = new TransferCreateHandler(_mockMediator.Object, _mockRepo.Object);

            var request = new TransferCreateRequest { AccountId = 1, CustomerId = 1, Ammount = 200, RecipientAccountId = 2, RecipientCustomerId = 2 };
            var accountFrom = GetTestFromAccount();

            _mockMediator
                 .Setup(m => m.Send(It.Is<AccountGetRequest>(a => a.AccountId == accountFrom.Id), default))
                 .Returns(Task.FromResult(accountFrom))
                 .Verifiable();

            MockTransferTransactionType();

            await Assert.ThrowsAsync<BusinessException>(() => handler.Handle(request, default));
            _mockMediator.Verify();
        }

        [Fact]
        public async Task Handle_CreateTransfer_ShouldReturnTransaction()
        {
            var handler = new TransferCreateHandler(_mockMediator.Object, _mockRepo.Object);

            var request = new TransferCreateRequest { AccountId = 1, CustomerId = 1, Ammount = 10, RecipientAccountId = 2, RecipientCustomerId = 2 };
            var accountFrom = GetTestFromAccount();
            var accountTo = GetTestToAccount();
            var transferType = GetTransferType();

            var transferTransactions = handler.BuildTransferTransactions(request, accountFrom, accountTo, transferType);

            MockTransferTransactionType();
            MockMediator(transferTransactions, accountFrom, accountTo);

            var result = await handler.Handle(request, default);

            _mockMediator.Verify();
            Assert.IsAssignableFrom<IEnumerable<TransactionModel>>(result);
            Assert.Equal(3, result.Count());
        }

        private void MockTransferTransactionType()
        {
            _mockRepo
                  .Setup(repo => repo.TransactionTypeRepository.FindSingle(It.IsAny<Expression<Func<TransactionType, bool>>>(), It.IsAny<List<string>>()))
                  .Returns(GetTransferType())
                  .Verifiable();
        }

        private void MockMediator(IEnumerable<TransactionModel> transactionModels, AccountModel accountFrom, AccountModel accountTo)
        {
            _mockMediator
                 .Setup(m => m.Send(It.Is<AccountGetRequest>(a => a.AccountId == accountFrom.Id), default))
                 .Returns(Task.FromResult(accountFrom))
                 .Verifiable();

            _mockMediator
                .Setup(m => m.Send(It.Is<AccountGetRequest>(a => a.AccountId == accountTo.Id), default))
                .Returns(Task.FromResult(accountTo))
                .Verifiable();

            _mockMediator
                 .Setup(m => m.Send(It.IsAny<TransactionCreateRequest>(), default))
                 .Returns(Task.FromResult(transactionModels));
        }


        private AccountModel GetTestFromAccount()
        {
            return new AccountModel
            {
                Id = 1,
                AccountTypeId = 1,
                CustomerId = 1,
                Balance = 100,
                Transactions = new List<TransactionModel>
                {
                    new TransactionModel { Ammount = 100},
                    new TransactionModel { Ammount = -50},
                }
            };
        }

        private AccountModel GetTestToAccount()
        {
            return new AccountModel
            {
                Id = 2,
                AccountTypeId = 1,
                CustomerId = 2,
                Transactions = new List<TransactionModel>
                {
                    new TransactionModel { Ammount = 100},
                    new TransactionModel { Ammount = -50},
                }
            };
        }

        private TransactionType GetTransferType()
        {
            return new TransactionType { Id = 3, Code = "T", FixedCharge = 1 };
        }
    }
}
