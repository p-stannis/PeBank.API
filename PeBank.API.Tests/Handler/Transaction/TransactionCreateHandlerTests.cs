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
    public class TransactionCreateHandlerTests : TestBase
    {
        private readonly Mock<IRepositoryWrapper> _mockRepo;
        public TransactionCreateHandlerTests()
        {
            _mockRepo = new Mock<IRepositoryWrapper>();
        }

        [Fact]
        public async Task Handle_CreateTransaction_ShouldReturnTransactions()
        {
            var handler = new TransactionCreateHandler(Mapper, _mockRepo.Object);

            var request = new TransactionCreateRequest
            {
                Transactions = TransactionModels()
            };

            var now = DateTime.Now;

            _mockRepo
              .Setup(repo => repo.Operation.Create(It.IsAny<Operation>()))
              .Callback<Operation>(a => a.Id = 666)
              .Verifiable();

            _mockRepo
              .Setup(repo => repo.TransactionRepository.CreateMany(It.IsAny<IEnumerable<Transaction>>()))
              .Verifiable();


            var result = await handler.Handle(request, default);

            Assert.True(result.All(t => t.OperationId == 666));

            Assert.IsAssignableFrom<IEnumerable<TransactionModel>>(result);
        }

        private List<TransactionModel> TransactionModels()
        {
            return new List<TransactionModel>
            {
                new TransactionModel
                {
                    AccountId = 1,
                    Ammount = -10,
                    Details = "withdraw",
                    TransactionTypeId = 2
                },
                new TransactionModel
                {
                    AccountId = 1,
                    Ammount = -4,
                    Details = "withdraw",
                    TransactionTypeId = 2
                },
            };
        }
    }
}
