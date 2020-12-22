using MediatR;
using System.Collections.Generic;

namespace PeBank.API.Features
{
    public class TransactionCreateRequest : IRequest<IEnumerable<TransactionModel>>
    {
        public string OperationDetails { get; set; }
        public IEnumerable<TransactionModel> Transactions { get; set; }
    }
}
