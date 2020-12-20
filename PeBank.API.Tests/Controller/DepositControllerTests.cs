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
    public class DepositControllerTests 
    {
        private readonly Mock<IMediator> _mockMediator;

        public DepositControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
        }

        [Fact]
        public async Task Create_DepositExistingAccount_ShouldReturnTransactions()
        {
            var request = new DepositCreateRequest
            {
                AccountId = 1,
                Ammount = 100,
                CustomerId = 2,
                Details = "Test"
            };

            var depositTransactions = DepositTransactions(request);

            _mockMediator
                .Setup(m => m.Send(It.Is<DepositCreateRequest>(d => d.AccountId == request.AccountId
                                                                 && d.Ammount == request.Ammount
                                                                 && d.CustomerId == request.CustomerId
                                                                 && d.Details == request.Details), default))
                .ReturnsAsync(depositTransactions)
                .Verifiable();

            var controller = new DepositController(_mockMediator.Object);

            var result = await controller.Create(request);

            _mockMediator.Verify();

            Assert.True(typeof(CreatedAtActionResult) == result.Result.GetType());

        }

        [Fact]
        public async Task Create_DepositAccountDoesntBelongToCustomer_ShouldReturnConflict()
        {
            var request = new DepositCreateRequest
            {
                AccountId = 1,
                Ammount = 100,
                CustomerId = 2,
                Details = "Test"
            };

            _mockMediator
                .Setup(m => m.Send(It.Is<DepositCreateRequest>(d => d.AccountId == request.AccountId
                                                                 && d.Ammount == request.Ammount
                                                                 && d.CustomerId == request.CustomerId
                                                                 && d.Details == request.Details), default))
                .ThrowsAsync(new BusinessException())
                .Verifiable();

            var controller = new DepositController(_mockMediator.Object);

            var result = await controller.Create(request);

            _mockMediator.Verify();

            Assert.True(typeof(ConflictObjectResult) == result.Result.GetType());

        }

        [Fact]
        public async Task Create_DepositNonExistingAccount_ShouldReturnNotFound()
        {
            var request = new DepositCreateRequest
            {
                AccountId = 1,
                Ammount = 100,
                CustomerId = 2,
                Details = "Test"
            };

            _mockMediator
                .Setup(m => m.Send(It.Is<DepositCreateRequest>(d => d.AccountId == request.AccountId
                                                                 && d.Ammount == request.Ammount
                                                                 && d.CustomerId == request.CustomerId
                                                                 && d.Details == request.Details), default))
                .ThrowsAsync(new NotFoundException())
                .Verifiable();

            var controller = new DepositController(_mockMediator.Object);

            var result = await controller.Create(request);

            _mockMediator.Verify();

            Assert.True(typeof(NotFoundObjectResult) == result.Result.GetType());
        }

        private List<TransactionModel> DepositTransactions(DepositCreateRequest request) 
        {
            return new List<TransactionModel> 
            {
                new TransactionModel
                {
                    AccountId = request.AccountId.Value,
                    Ammount = request.Ammount,
                    Date = DateTime.Now,
                    Details = request.Details,
                    OperationId = 1,
                    TransactionTypeId = 1
                },
                new TransactionModel
                {
                    AccountId = request.AccountId.Value,
                    Ammount = -(0.01 * request.Ammount),
                    Date = DateTime.Now,
                    Details = request.Details,
                    OperationId = 1,
                    TransactionTypeId = 1
                }
            };
        }
    }
}
