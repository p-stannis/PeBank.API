using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PeBank.API.Controllers;
using PeBank.API.Features;
using PeBank.API.Features.Utils.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;


namespace PeBank.API.Tests.Controller
{
    public class WithdrawControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;

        public WithdrawControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
        }

        [Fact]
        public async Task Create_WithdrawExistingAccount_ShouldReturnTransactions()
        {
            var request = new WithdrawCreateRequest
            {
                AccountId = 1,
                Ammount = 100,
                CustomerId = 2,
                Details = "Test",
            };

            var transferTransactions = WithdrawTransactions(request);

            _mockMediator
                .Setup(m => m.Send(It.Is<WithdrawCreateRequest>(t => t.AccountId == request.AccountId
                                                                 && t.Ammount == request.Ammount
                                                                 && t.CustomerId == request.CustomerId
                                                                 && t.Details == request.Details), default))
                .ReturnsAsync(transferTransactions)
                .Verifiable();

            var controller = new WithdrawController(_mockMediator.Object);

            var result = await controller.Create(request);

            _mockMediator.Verify();

            Assert.True(typeof(CreatedAtActionResult) == result.Result.GetType());
        }

        [Fact]
        public async Task Create_WithdrawAccountDoesntBelongToCustomer_ShouldReturnConflict()
        {
            var request = new WithdrawCreateRequest
            {
                AccountId = 1,
                Ammount = 100,
                CustomerId = 2,
                Details = "Test",
            };

            _mockMediator
                .Setup(m => m.Send(It.Is<WithdrawCreateRequest>(t => t.AccountId == request.AccountId
                                                                 && t.Ammount == request.Ammount
                                                                 && t.CustomerId == request.CustomerId
                                                                 && t.Details == request.Details), default))
                .ThrowsAsync(new BusinessException())
                .Verifiable();

            var controller = new WithdrawController(_mockMediator.Object);

            var result = await controller.Create(request);

            _mockMediator.Verify();

            Assert.True(typeof(ConflictObjectResult) == result.Result.GetType());
        }

        [Fact]
        public async Task Create_WithdrawNonExistingAccount_ShouldReturnNotFound()
        {
            var request = new WithdrawCreateRequest
            {
                AccountId = 1,
                Ammount = 100,
                CustomerId = 2,
                Details = "Test",
            };

            _mockMediator
                .Setup(m => m.Send(It.Is<WithdrawCreateRequest>(t => t.AccountId == request.AccountId
                                                                 && t.Ammount == request.Ammount
                                                                 && t.CustomerId == request.CustomerId
                                                                 && t.Details == request.Details), default))
                .ThrowsAsync(new NotFoundException())
                .Verifiable();

            var controller = new WithdrawController(_mockMediator.Object);

            var result = await controller.Create(request);

            _mockMediator.Verify();

            Assert.True(typeof(NotFoundObjectResult) == result.Result.GetType());
        }

        private List<TransactionModel> WithdrawTransactions(WithdrawCreateRequest request)
        {
            return new List<TransactionModel>
            {
                new TransactionModel
                {
                    AccountId = request.AccountId.Value,
                    Ammount = -request.Ammount,
                    Date = DateTime.Now,
                    Details = request.Details,
                    OperationId = 1,
                    TransactionTypeId = 2
                },
                new TransactionModel
                {
                    AccountId = request.AccountId.Value,
                    Ammount = -4,
                    Date = DateTime.Now,
                    Details = request.Details,
                    OperationId = 1,
                    TransactionTypeId = 2
                },
            };
        }
    }
}
