using MediatR;
using Moq;
using PeBank.API.Contracts;
using PeBank.API.Entities;
using PeBank.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;


namespace PeBank.API.Tests.Handler
{
    public class DepositCreateHandlerTests : TestBase
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<IRepositoryWrapper> _mockRepo;
        public DepositCreateHandlerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _mockRepo = new Mock<IRepositoryWrapper>();
        }

        [Fact]
        public async Task Handle_CreateDeposit_ShouldReturnTransaction()
        {
            var handler = new DepositCreateHandler(_mockMediator.Object, _mockRepo.Object);

            var request = new DepositCreateRequest { AccountId = 1, CustomerId = 1, Ammount = 100 };
            var account = GetTestAccount();
            var depositType = GetDepositType();

            var depositTransactions = handler.BuildDepositTransaction(request, account, depositType);

            MockDepositTransactionType();
            MockMediator(depositTransactions);

            var result = await handler.Handle(request, default);

            Assert.IsAssignableFrom<IEnumerable<TransactionModel>>(result);
            Assert.Equal(2, result.Count());
        }

        private void MockMediator(IEnumerable<TransactionModel> transactionModels)
        {
            var testAccount = GetTestAccount();
            _mockMediator
                 .Setup(m => m.Send(It.IsAny<AccountGetRequest>(), default))
                 .Returns(Task.FromResult(testAccount));

            _mockMediator
                 .Setup(m => m.Send(It.IsAny<TransactionCreateRequest>(), default))
                 .Returns(Task.FromResult(transactionModels));
        }

        private void MockDepositTransactionType()
        {
            _mockRepo
                  .Setup(repo => repo.TransactionTypeRepository.FindSingle(It.IsAny<Expression<Func<TransactionType, bool>>>(), It.IsAny<List<string>>()))
                  .Returns(GetDepositType())
                  .Verifiable();
        }


        private AccountModel GetTestAccount()
        {
            return new AccountModel
            {
                Id = 1,
                AccountTypeId = 1,
                CustomerId = 1,
                Transactions = new List<TransactionModel>
                {
                    new TransactionModel { Ammount = 100},
                    new TransactionModel { Ammount = -50},
                }
            };
        }

        private TransactionType GetDepositType()
        {
            return new TransactionType { Id = 1, Code = "D", PercentCharge = 1 };
        }
    }
}
