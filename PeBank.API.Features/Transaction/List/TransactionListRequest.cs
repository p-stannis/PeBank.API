using MediatR;
using System.Collections.Generic;

namespace PeBank.API.Features
{
    public class TransactionListRequest : IRequest<IEnumerable<TransactionModel>>
    {
        public int AccountId { get; set; }
    }
}
