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
    public class CustomerCreateHandlerTests : TestBase
    {
        private readonly Mock<IRepositoryWrapper> _mockRepo;
        public CustomerCreateHandlerTests()
        {
            _mockRepo = new Mock<IRepositoryWrapper>();
        }

        [Fact]
        public async Task Handle_CreateExistingCustomer_ShouldReturnBusinessException()
        {
            var handler = new CustomerCreateHandler(Mapper, _mockRepo.Object);

            var name = "pedro";
            var email = "pedro@gmail.com";

            var customer = new Customer
            {
                Email = email,
                Id = 1,
                Name = name
            };

            _mockRepo
               .Setup(repo => repo.Customer.FindSingle(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<List<string>>()))
               .Returns(customer)
               .Verifiable();

            await Assert.ThrowsAsync<BusinessException>(() => handler.Handle(new CustomerCreateRequest { Email = email, Name = name }, default));
        }

        [Fact]
        public async Task Handle_CreateNonExistingCustomer_ShouldReturnNewCustomer()
        {
            var handler = new CustomerCreateHandler(Mapper, _mockRepo.Object);

            var name = "pedro";
            var email = "pedro@gmail.com";

            var customer = new Customer
            {
                Email = email,
                Id = 1,
                Name = name
            };

            MockRepo(customer);

            var result = await handler.Handle(new CustomerCreateRequest { Email = email, Name = name }, default);

            Assert.Equal(email, result.Email);
            Assert.Equal(name, result.Name);
            Assert.Equal(5, result.Id);
            Assert.IsType<CustomerModel>(result);

        }

        private void MockRepo(Customer customer)
        {
            _mockRepo
                .Setup(repo => repo.Customer.FindSingle(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<List<string>>()))
                .Returns(null as Customer);

            _mockRepo
                .Setup(repo => repo.Customer.Create(It.Is<Customer>(c => c.Email == customer.Email && c.Name == customer.Name)))
                .Callback<Customer>(c => c.Id = 5)
                .Verifiable();
        }

    }
}
