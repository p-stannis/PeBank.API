using Moq;
using PeBank.API.Contracts;
using PeBank.API.Entities;
using PeBank.API.Features;
using PeBank.API.Features.Utils.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PeBank.API.Tests.Handler
{
    public class AccountGetHandlerTests : TestBase
    {
        private readonly Mock<IRepositoryWrapper> _mockRepo;
        public AccountGetHandlerTests()
        {
            _mockRepo = new Mock<IRepositoryWrapper>();
        }

        [Fact]
        public async Task Handle_GetNonExistingAccount_ShouldReturnNotFound()
        {
            var handler = new AccountGetHandler(Mapper, _mockRepo.Object);

            _mockRepo
                .Setup(repo => repo.Account.FindById(It.Is<int>(a => a == 1), It.IsAny<List<string>>()))
                .Returns(null as Account);

            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(new AccountGetRequest { AccountId = 1, CustomerId = 2 }, default));

        }

        [Fact]
        public async Task Handle_GetExistingAccount_ShouldReturnAccount()
        {
            var handler = new AccountGetHandler(Mapper, _mockRepo.Object);

            var account = GetTestAccount();
            var request = new AccountGetRequest { AccountId = 1, CustomerId = 1 };

            MockRepo(account);
            
            var result = await handler.Handle(request, default);

            _mockRepo.Verify();

            Assert.IsType<AccountModel>(result);
            Assert.Equal(request.CustomerId , result.CustomerId);
            Assert.Equal(request.AccountId , result.Id);
            Assert.Equal(50, result.Balance);
        }

        [Fact]
        public async Task Handle_GetExistingAccountDifferentCustomer_ShouldReturnBusinessException()
        {
            var handler = new AccountGetHandler(Mapper, _mockRepo.Object);

            var account = GetTestAccount();
            var request = new AccountGetRequest { AccountId = 1, CustomerId = 2 };

            MockRepo(account);

            await Assert.ThrowsAsync<BusinessException>(() => handler.Handle(request, default));
        }

        private void MockRepo(Account account)
        {
            _mockRepo
                .Setup(repo => repo.Account.FindById(It.Is<int>(a => a == account.Id), It.IsAny<List<string>>()))
                .Returns(account)
                .Verifiable();
        }

        private Account GetTestAccount()
        {
            return new Account
            {
                Id = 1,
                AccountTypeId = 1,
                CustomerId = 1,
                Customer = new Customer { Id = 1, Name = "Pedro" },
                Transactions = new List<Transaction> 
                {
                    new Transaction { Ammount = 100},
                    new Transaction { Ammount = -50},
                }
            };
        }
    }
}
