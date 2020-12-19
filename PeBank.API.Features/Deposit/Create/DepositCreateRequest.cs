using MediatR;
using System.Collections.Generic;

namespace PeBank.API.Features
{
    public class DepositCreateRequest : TransactionBaseRequest, IRequest<IEnumerable<TransactionModel>>
    {
    }
}
