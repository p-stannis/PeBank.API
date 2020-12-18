using MediatR;

namespace PeBank.API.Features
{
    public class AccountGetRequest : IRequest<AccountModel>
    {
        public int AccountId { get; set; }
        public int CustomerId { get; set; }
    }
}
