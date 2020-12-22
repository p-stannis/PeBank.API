using AutoMapper;
using MediatR;
using PeBank.API.Contracts;
using PeBank.API.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PeBank.API.Features
{
    public class AccountTypeGetAllHandler : IRequestHandler<AccountTypeGetAllRequest, IEnumerable<AccountTypeModel>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repository;
        public AccountTypeGetAllHandler(IMapper mapper, IRepositoryWrapper repository)
        {
            _mapper = mapper;
            _repository = repository;
        }
        public Task<IEnumerable<AccountTypeModel>> Handle(AccountTypeGetAllRequest request, CancellationToken cancellationToken)
        {
            var accountTypes = _repository.AccountType.FindAll();
            var result = _mapper.Map<IEnumerable<AccountType>, IEnumerable<AccountTypeModel>>(accountTypes);
            return Task.FromResult(result);
        }
    }
}
