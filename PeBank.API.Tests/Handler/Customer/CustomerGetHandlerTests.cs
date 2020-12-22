using Moq;
using PeBank.API.Contracts;
using PeBank.API.Entities;
using PeBank.API.Features;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PeBank.API.Tests.Handler
{
    public class CustomerGetHandlerTests : TestBase
    {
        private readonly Mock<IRepositoryWrapper> _mockRepo;
        public CustomerGetHandlerTests()
        {
            _mockRepo = new Mock<IRepositoryWrapper>();
        }

        [Fact]
        public async Task Handle_GetExistingCustomer_ShouldReturnCustomer() 
        {
            var handler = new CustomerGetHandler(Mapper, _mockRepo.Object);
            var customer = new Customer
            {
                Email = "@",
                Id = 1,
                Name = "Pedro"
            };

            MockRepo(customer);

            var result = await handler.Handle(new CustomerGetRequest { Id = customer.Id}, default);

            _mockRepo.Verify();

            Assert.IsType<CustomerModel>(result);
            Assert.Equal(customer.Id, result.Id);
            Assert.Equal(customer.Name, result.Name);
            Assert.Equal(customer.Email, result.Email);
        }

        [Fact]
        public async Task Handle_GetNonExistingCustomer_ShouldReturnNull()
        {
            var handler = new CustomerGetHandler(Mapper, _mockRepo.Object);

            MockRepo(null);

            var result = await handler.Handle(new CustomerGetRequest { Id = 2 }, default);

            Assert.True(result == null);
        }

        private void MockRepo(Customer customer) 
        {
            _mockRepo
                .Setup(repo => repo.Customer.FindById(It.Is<int>(c => c == 1), It.IsAny<List<string>>()))
                .Returns(customer)
                .Verifiable();
        }
    }
}
