using MediatR;
using System.Collections.Generic;

namespace PeBank.API.Features
{
    public class WithdrawCreateRequest : TransactionBaseRequest, IRequest<IEnumerable<TransactionModel>>
    {
    }
}
