using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PeBank.API.Controllers;
using PeBank.API.Features;
using PeBank.API.Features.Utils.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PeBank.API.Tests.Controller
{
    public class StatementControllerTests : TestBase
    {
        private readonly Mock<IMediator> _mockMediator;

        public StatementControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
        }

        [Fact]
        public async Task Get_Statements_ShouldReturnStatements()
        {
            var testAccountId = 1;
            var testCustomerId = 1;

            var testAccount = GetTestAccount(testAccountId, testCustomerId);

            _mockMediator
                .Setup(m => m.Send(It.Is<AccountGetRequest>(a => a.AccountId == testAccountId && a.CustomerId == testCustomerId), default))
                .ReturnsAsync(testAccount)
                .Verifiable();


            var controller = new StatementController(Mapper, _mockMediator.Object);

            var result = await controller.Get(testAccountId, testCustomerId);

            _mockMediator.Verify();


            Assert.True(typeof(StatementModel) == result.Value.GetType());
            Assert.Equal(testAccount.Transactions.Count(), result.Value.Transactions.Count());

        }

        [Fact]
        public async Task Get_ExistingAccountDifferentCustomerStatement_ShouldReturnConflict()
        {
            var testAccountId = 1;
            var testCustomerId = 1;

            _mockMediator
                .Setup(m => m.Send(It.Is<AccountGetRequest>(a => a.AccountId == testAccountId && a.CustomerId == testCustomerId), default))
                .ThrowsAsync(new BusinessException())
                .Verifiable();

            var controller = new StatementController(Mapper, _mockMediator.Object);

            var result = await controller.Get(testAccountId, testCustomerId);

            _mockMediator.Verify();

            Assert.True(typeof(ConflictObjectResult) == result.Result.GetType());
        }

        [Fact]
        public async Task Get_ExistingAccountDifferentCustomerStatement_ShouldReturnNotFound()
        {
            var testAccountId = 1;
            var testCustomerId = 1;

            _mockMediator
                .Setup(m => m.Send(It.Is<AccountGetRequest>(a => a.AccountId == testAccountId && a.CustomerId == testCustomerId), default))
                .ThrowsAsync(new NotFoundException())
                .Verifiable();

            var controller = new StatementController(Mapper, _mockMediator.Object);

            var result = await controller.Get(testAccountId, testCustomerId);

            _mockMediator.Verify();

            Assert.True(typeof(NotFoundObjectResult) == result.Result.GetType());

        }

        private AccountModel GetTestAccount(int accountId, int customerId)
        {
            return new AccountModel
            {
                AccountTypeId = 1,
                Balance = 100,
                CustomerId = customerId,
                Id = accountId,
                Transactions = GetTransactions(accountId)
            };
        }

        private List<TransactionModel> GetTransactions(int accountId)
        {
            return new List<TransactionModel>
            {
                new TransactionModel
                {
                    AccountId = accountId,
                    Ammount = 1000,
                    Date = DateTime.Now,
                    Details = "deposit",
                    OperationId = 1,
                    TransactionTypeId = 1
                },
                new TransactionModel
                {
                    AccountId = accountId,
                    Ammount = -(0.01 * 1000),
                    Date = DateTime.Now,
                    Details = "deposit",
                    OperationId = 1,
                    TransactionTypeId = 1
                },
                new TransactionModel
                {
                    AccountId = accountId,
                    Ammount = -300,
                    Date = DateTime.Now,
                    Details = "withdraw",
                    OperationId = 2,
                    TransactionTypeId = 2
                },
                new TransactionModel
                {
                    AccountId = accountId,
                    Ammount = -4,
                    Date = DateTime.Now,
                    Details = "withdraw",
                    OperationId = 2,
                    TransactionTypeId = 2
                }
            };
        }
    }
}
