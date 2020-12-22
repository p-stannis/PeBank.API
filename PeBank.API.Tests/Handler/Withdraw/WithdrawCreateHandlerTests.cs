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
    public class WithdrawCreateHandlerTests : TestBase
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<IRepositoryWrapper> _mockRepo;
        public WithdrawCreateHandlerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _mockRepo = new Mock<IRepositoryWrapper>();
        }

        [Fact]
        public async Task Handle_CreateWithdrawNegativeBalance_ShouldReturnBusinessException()
        {
            var handler = new WithdrawCreateHandler(_mockMediator.Object, _mockRepo.Object);

            var request = new WithdrawCreateRequest { AccountId = 1, CustomerId = 1, Ammount = 200};
            var account = GetTestAccount();

            _mockMediator
                 .Setup(m => m.Send(It.Is<AccountGetRequest>(a => a.AccountId == account.Id), default))
                 .Returns(Task.FromResult(account))
                 .Verifiable();

            MockWithdrawTransactionType();

            await Assert.ThrowsAsync<BusinessException>(() => handler.Handle(request, default));
            _mockMediator.Verify();
        }

        [Fact]
        public async Task Handle_CreateWithdrawl_ShouldReturnTransaction()
        {
            var handler = new WithdrawCreateHandler(_mockMediator.Object, _mockRepo.Object);

            var request = new WithdrawCreateRequest { AccountId = 1, CustomerId = 1, Ammount = 10 };
            var account = GetTestAccount();
            var withdrawalType = GetWithdrawType();

            var withdrawTransactions = handler.BuildWithdrawalTransactions(request, account, withdrawalType);

            MockWithdrawTransactionType();
            MockMediator(withdrawTransactions, account);

            var result = await handler.Handle(request, default);

            _mockMediator.Verify();
            Assert.IsAssignableFrom<IEnumerable<TransactionModel>>(result);
            Assert.Equal(2, result.Count());
        }

        private AccountModel GetTestAccount()
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
        private void MockWithdrawTransactionType()
        {
            _mockRepo
                  .Setup(repo => repo.TransactionTypeRepository.FindSingle(It.IsAny<Expression<Func<TransactionType, bool>>>(), It.IsAny<List<string>>()))
                  .Returns(GetWithdrawType())
                  .Verifiable();
        }

        private void MockMediator(IEnumerable<TransactionModel> transactionModels, AccountModel account)
        {
            _mockMediator
                 .Setup(m => m.Send(It.Is<AccountGetRequest>(a => a.AccountId == account.Id), default))
                 .Returns(Task.FromResult(account))
                 .Verifiable();

            _mockMediator
                 .Setup(m => m.Send(It.IsAny<TransactionCreateRequest>(), default))
                 .Returns(Task.FromResult(transactionModels));
        }

        private TransactionType GetWithdrawType()
        {
            return new TransactionType { Id = 2, Code = "W", FixedCharge = 4 };
        }
    }
}
