using AutoMapper;
using MediatR;
using PeBank.API.Contracts;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PeBank.API.Features
{
    public class CustomerGetHandler : IRequestHandler<CustomerGetRequest, CustomerModel>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repository;

        public CustomerGetHandler(IMapper mapper, IRepositoryWrapper repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public Task<CustomerModel> Handle(CustomerGetRequest request, CancellationToken cancellationToken)
        {
            var customer = _repository.Customer.FindById(request.Id, new List<string> { "Accounts "});

            var result = _mapper.Map<CustomerModel>(customer);

            return Task.FromResult(result);
        }
    }
}
