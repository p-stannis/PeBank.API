using MediatR;

namespace PeBank.API.Features
{
    public class CustomerGetRequest : IRequest<CustomerModel>
    {
        public int Id { get; set; }
    }
}
