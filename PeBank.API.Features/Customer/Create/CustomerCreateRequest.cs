using MediatR;
namespace PeBank.API.Features
{
    public class CustomerCreateRequest : IRequest<CustomerModel>
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
