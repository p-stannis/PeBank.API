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
    public class CustomerControllerTests : TestBase
    {
        private readonly Mock<IMediator> _mockMediator;

        public CustomerControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
        }

        [Fact]
        public async Task Get_ExistingCustomer_ShouldReturnCustomer()
        {
            var testCustomerId = 1;

            var testCustomer = GetTestCustomer();

            _mockMediator
                .Setup(m => m.Send(It.Is<CustomerGetRequest>(c => c.Id == testCustomerId), default))
                .ReturnsAsync(testCustomer)
                .Verifiable();

            var controller = new CustomerController( _mockMediator.Object);

            var result = await controller.Get(testCustomerId);
            _mockMediator.Verify();
            Assert.Equal(testCustomerId, result.Value.Id);
        }

        [Fact]
        public async Task Get_NonExistingCustomer_ShouldReturnNotFound()
        {
            var testCustomerId = 1;

            _mockMediator
                .Setup(m => m.Send(It.Is<CustomerGetRequest>(c => c.Id == testCustomerId), default))
                .ReturnsAsync(null as CustomerModel)
                .Verifiable();

            var controller = new CustomerController(_mockMediator.Object);

            var result = await controller.Get(testCustomerId);
            _mockMediator.Verify();
            Assert.True(typeof(NotFoundResult) == result.Result.GetType());
        }

        [Fact]
        public async Task Create_NonExistingCustomer_ShouldReturnCreateAtAction()
        {
            var request = new CustomerCreateRequest { Name = "Pedro", Email = "pe@gmail.com"};

            var createdCustomer = new CustomerModel { Name = "Pedro", Email = "pe@gmail.com", Id = 1 };
            
            _mockMediator
                .Setup(m => m.Send(It.Is<CustomerCreateRequest>(c => c.Email == request.Email && c.Name == request.Name), default))
                .ReturnsAsync(createdCustomer)
                .Verifiable();

            var controller = new CustomerController(_mockMediator.Object);

            var result = await controller.Create(request);

            _mockMediator.Verify();
            Assert.True(typeof(CreatedAtActionResult) == result.Result.GetType());
        }

        [Fact]
        public async Task Create_ExistingCustomer_ShouldReturnConflict()
        {
            var request = new CustomerCreateRequest { Name = "Pedro", Email = "pe@gmail.com" };

            _mockMediator
                .Setup(m => m.Send(It.Is<CustomerCreateRequest>(c => c.Email == request.Email && c.Name == request.Name), default))
                .ThrowsAsync(new BusinessException());

            var controller = new CustomerController(_mockMediator.Object);

            var result = await controller.Create(request);

            Assert.True(typeof(ConflictObjectResult) == result.Result.GetType());
        }

        private CustomerModel GetTestCustomer()
        {
            return new CustomerModel 
            {
                Email = "pe@gmail.com",
                Name = "Pedro",
                Id = 1
            };
        }
    }
}
