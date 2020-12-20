using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PeBank.API.Controllers;
using PeBank.API.Features;
using PeBank.API.Features.Utils.Exceptions;
using System.Threading.Tasks;
using Xunit;

namespace PeBank.API.Tests.Controller
{
    public class AccountControllerTests : TestBase
    {
        private readonly Mock<IMediator> _mockMediator;

        public AccountControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
        }

        [Fact]
        public async Task Get_ExistingAccount_ShouldReturnAccount()
        {
            var testAccountId = 1;
            var testCustomerId = 1;

            var testAccount = GetTestAccount(testAccountId, testCustomerId);

            _mockMediator
                .Setup(m => m.Send(It.Is<AccountGetRequest>(a => a.AccountId == testAccountId && a.CustomerId == testCustomerId), default))
                .ReturnsAsync(testAccount)
                .Verifiable();


            var controller = new AccountController(Mapper, _mockMediator.Object);

            var result = await controller.Get(testAccountId, testCustomerId);

            _mockMediator.Verify();
            Assert.Equal(testAccountId, result.Value.Id);
            Assert.Equal(testCustomerId, result.Value.CustomerId);
        }

        [Fact]
        public async Task Get_ExistingAccountDifferentCustomer_ShouldReturnConflict()
        {
            var testAccountId = 1;
            var testCustomerId = 1;

            _mockMediator
                .Setup(m => m.Send(It.Is<AccountGetRequest>(a => a.AccountId == testAccountId && a.CustomerId == testCustomerId), default))
                .ThrowsAsync(new BusinessException())
                .Verifiable();

            var controller = new AccountController(Mapper, _mockMediator.Object);

            var result = await controller.Get(testAccountId, testCustomerId);

            _mockMediator.Verify();
            Assert.True(typeof(ConflictObjectResult) == result.Result.GetType());
        }

        [Fact]
        public async Task Get_NonExistingAccount_ShouldReturnNotFound()
        {
            var testAccountId = 1;
            var testCustomerId = 1;

            _mockMediator
                .Setup(m => m.Send(It.Is<AccountGetRequest>(a => a.AccountId == testAccountId && a.CustomerId == testCustomerId), default))
                .ThrowsAsync(new NotFoundException())
                .Verifiable();

            var controller = new AccountController(Mapper, _mockMediator.Object);

            var result = await controller.Get(testAccountId, testCustomerId);

            Assert.True(typeof(NotFoundObjectResult) == result.Result.GetType());
        }

        [Fact]
        public async Task Create_NonExistingAccount_ShouldReturnCreatedAccount()
        {
            var request = new AccountCreateRequest { AccountTypeId = 1, CustomerId = 1 };

            var createdAccount = GetTestAccount(99, request.CustomerId.Value);

           
            _mockMediator
                .Setup(m => m.Send(It.Is<AccountCreateRequest>(a => a.CustomerId == request.CustomerId && a.AccountTypeId == request.AccountTypeId), default))
                .ReturnsAsync(createdAccount)
                .Verifiable();

            var controller = new AccountController(Mapper, _mockMediator.Object);

            var result = await controller.Create(request);

            _mockMediator.Verify();
            Assert.True(typeof(CreatedAtActionResult) == result.Result.GetType());

        }

        [Fact]
        public async Task Create_ExistingAccount_ShouldReturnConflict()
        {
            var request = new AccountCreateRequest { AccountTypeId = 1, CustomerId = 1 };

            var createdAccount = GetTestAccount(99, request.CustomerId.Value);


            _mockMediator
                .Setup(m => m.Send(It.Is<AccountCreateRequest>(a => a.CustomerId == request.CustomerId && a.AccountTypeId == request.AccountTypeId), default))
                .ThrowsAsync(new BusinessException())
                .Verifiable();

            var controller = new AccountController(Mapper, _mockMediator.Object);

            var result = await controller.Create(request);

            _mockMediator.Verify();
            Assert.True(typeof(ConflictObjectResult) == result.Result.GetType());
        }



        private AccountModel GetTestAccount(int accountId, int customerId)
        {
            return new AccountModel
            {
                AccountTypeId = 1,
                Balance = 100,
                CustomerId = customerId,
                Id = accountId
            };
        }
    }
}
