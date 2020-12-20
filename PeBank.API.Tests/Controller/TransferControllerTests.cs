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
    public class TransferControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;

        public TransferControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
        }

        [Fact]
        public async Task Create_TransferExistingAccount_ShouldReturnTransactions()
        {
            var request = new TransferCreateRequest
            {
                AccountId = 1,
                Ammount = 100,
                CustomerId = 2,
                Details = "Test",
                RecipientAccountId = 20,
                RecipientCustomerId = 10
            };

            var transferTransactions = TransferTransactions(request);

            _mockMediator
                .Setup(m => m.Send(It.Is<TransferCreateRequest>(t => t.AccountId == request.AccountId
                                                                 && t.Ammount == request.Ammount
                                                                 && t.CustomerId == request.CustomerId
                                                                 && t.Details == request.Details
                                                                 && t.RecipientAccountId == request.RecipientAccountId
                                                                 && t.RecipientCustomerId == request.RecipientCustomerId), default))
                .ReturnsAsync(transferTransactions)
                .Verifiable();

            var controller = new TransferController(_mockMediator.Object);

            var result = await controller.Create(request);

            _mockMediator.Verify();

            Assert.True(typeof(CreatedAtActionResult) == result.Result.GetType());
        }

        [Fact]
        public async Task Create_TransferAccountDoesntBelongToCustomer_ShouldReturnConflict()
        {
            var request = new TransferCreateRequest
            {
                AccountId = 1,
                Ammount = 100,
                CustomerId = 2,
                Details = "Test",
                RecipientAccountId = 20,
                RecipientCustomerId = 10
            };

            _mockMediator
                .Setup(m => m.Send(It.Is<TransferCreateRequest>(t => t.AccountId == request.AccountId
                                                                 && t.Ammount == request.Ammount
                                                                 && t.CustomerId == request.CustomerId
                                                                 && t.Details == request.Details
                                                                 && t.RecipientAccountId == request.RecipientAccountId
                                                                 && t.RecipientCustomerId == request.RecipientCustomerId), default))
                .ThrowsAsync(new BusinessException())
                .Verifiable();

            var controller = new TransferController(_mockMediator.Object);

            var result = await controller.Create(request);

            _mockMediator.Verify();

            Assert.True(typeof(ConflictObjectResult) == result.Result.GetType());
        }

        [Fact]
        public async Task Create_TransferNonExistingAccount_ShouldReturnNotFound()
        {
            var request = new TransferCreateRequest
            {
                AccountId = 1,
                Ammount = 100,
                CustomerId = 2,
                Details = "Test",
                RecipientAccountId = 20,
                RecipientCustomerId = 10
            };

            _mockMediator
                .Setup(m => m.Send(It.Is<TransferCreateRequest>(t => t.AccountId == request.AccountId
                                                                 && t.Ammount == request.Ammount
                                                                 && t.CustomerId == request.CustomerId
                                                                 && t.Details == request.Details
                                                                 && t.RecipientAccountId == request.RecipientAccountId
                                                                 && t.RecipientCustomerId == request.RecipientCustomerId), default))
                .ThrowsAsync(new NotFoundException())
                .Verifiable();

            var controller = new TransferController(_mockMediator.Object);

            var result = await controller.Create(request);

            _mockMediator.Verify();

            Assert.True(typeof(NotFoundObjectResult) == result.Result.GetType());
        }

        private List<TransactionModel> TransferTransactions(TransferCreateRequest request)
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
                    TransactionTypeId = 3
                },
                new TransactionModel
                {
                    AccountId = request.AccountId.Value,
                    Ammount = -1,
                    Date = DateTime.Now,
                    Details = request.Details,
                    OperationId = 1,
                    TransactionTypeId = 3
                },
                new TransactionModel
                {
                    AccountId = request.RecipientAccountId,
                    Ammount = request.Ammount,
                    Date = DateTime.Now,
                    Details = request.Details,
                    OperationId = 1,
                    TransactionTypeId = 3
                }
            };
        }
    }
}
