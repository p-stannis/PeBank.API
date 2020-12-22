using MediatR;
using System.Collections.Generic;

namespace PeBank.API.Features
{
    public class AccountTypeGetAllRequest : IRequest<IEnumerable<AccountTypeModel>>
    {
    }
}
