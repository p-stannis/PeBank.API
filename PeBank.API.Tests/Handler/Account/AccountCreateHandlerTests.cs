using Moq;
using PeBank.API.Contracts;
using PeBank.API.Entities;
using PeBank.API.Features;
using PeBank.API.Features.Utils.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace PeBank.API.Tests.Handler
{
    public class AccountCreateHandlerTests : TestBase
    {
        private readonly Mock<IRepositoryWrapper> _mockRepo;
        public AccountCreateHandlerTests()
        {
            _mockRepo = new Mock<IRepositoryWrapper>();
        }

        [Fact]
        public async Task Handle_CreateExistingAccount_ShouldReturnBusinessException()
        {
            var handler = new AccountCreateHandler(Mapper, _mockRepo.Object);

            var existingAccount = GetTestAccount();

            _mockRepo
                .Setup(repo => repo.Account.FindSingle(It.IsAny<Expression<Func<Account, bool>>>(), It.IsAny<List<string>>()))
                .Returns(existingAccount);

            await Assert.ThrowsAsync<BusinessException>(() => handler.Handle(new AccountCreateRequest { AccountTypeId = 1, CustomerId = 2 }, default));

        }

        [Fact]
        public async Task Handle_CreateNonExistingAccountNonExistingCustomer_ShouldReturnBusinessException()
        {
            var handler = new AccountCreateHandler(Mapper, _mockRepo.Object);

            var request = new AccountCreateRequest { AccountTypeId = 1, CustomerId = 2 };

            MockReturnNullAccount();

            _mockRepo
                .Setup(repo => repo.Customer.FindById(It.Is<int>(c => c == request.CustomerId), It.IsAny<List<string>>()))
                .Returns(null as Customer);

            await Assert.ThrowsAsync<BusinessException>(() => handler.Handle(request, default));
        }

        [Fact]
        public async Task Handle_CreateAccountNonExistingAccountType_ShouldReturnBusinessException()
        {
            var handler = new AccountCreateHandler(Mapper, _mockRepo.Object);

            var request = new AccountCreateRequest { AccountTypeId = 1, CustomerId = 2 };

            MockReturnNullAccount();

            _mockRepo
                .Setup(repo => repo.Customer.FindById(It.Is<int>(c => c == request.CustomerId), It.IsAny<List<string>>()))
                .Returns(new Customer { Id = request.CustomerId.Value })
                .Verifiable();

            _mockRepo
                .Setup(repo => repo.AccountType.FindById(It.Is<int>(at => at == request.AccountTypeId), It.IsAny<List<string>>()))
                .Returns(null as AccountType)
                .Verifiable();

            await Assert.ThrowsAsync<BusinessException>(() => handler.Handle(request, default));

            _mockRepo.Verify();
        }

        [Fact]
        public async Task Handle_CreateNewAccount_ShouldReturnNewAccount()
        {
            var handler = new AccountCreateHandler(Mapper, _mockRepo.Object);

            var request = new AccountCreateRequest { AccountTypeId = 1, CustomerId = 2 };

            MockReturnNullAccount();

            _mockRepo
                .Setup(repo => repo.Customer.FindById(It.Is<int>(c => c == request.CustomerId), It.IsAny<List<string>>()))
                .Returns(new Customer { Id = request.CustomerId.Value })
                .Verifiable();

            _mockRepo
                .Setup(repo => repo.AccountType.FindById(It.Is<int>(at => at == request.AccountTypeId), It.IsAny<List<string>>()))
                .Returns(new AccountType { Id = 1})
                .Verifiable();

            _mockRepo
              .Setup(repo => repo.Account.Create(It.Is<Account>(a => a.AccountTypeId == request.AccountTypeId && request.CustomerId == a.CustomerId)))
              .Callback<Account>(a => a.Id = 4)
              .Verifiable();

            var result = await handler.Handle(request, default);

            _mockRepo.Verify();

            Assert.IsType<AccountModel>(result);
            Assert.Equal(4, result.Id);
        }


        private void MockRepo(Account account)
        {
            _mockRepo
                .Setup(repo => repo.Account.FindById(It.Is<int>(a => a == account.Id), It.IsAny<List<string>>()))
                .Returns(account)
                .Verifiable();
        }

        private void MockReturnNullAccount()
        {
            _mockRepo
               .Setup(repo => repo.Account.FindSingle(It.IsAny<Expression<Func<Account, bool>>>(), It.IsAny<List<string>>()))
               .Returns(null as Account);
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
