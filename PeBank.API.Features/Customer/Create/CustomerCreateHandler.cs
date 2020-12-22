using AutoMapper;
using MediatR;
using PeBank.API.Contracts;
using PeBank.API.Entities;
using PeBank.API.Features.Utils.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace PeBank.API.Features
{
    public class CustomerCreateHandler : IRequestHandler<CustomerCreateRequest, CustomerModel>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repository;

        public CustomerCreateHandler(IMapper mapper, IRepositoryWrapper repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public Task<CustomerModel> Handle(CustomerCreateRequest request, CancellationToken cancellationToken)
        {
            var customerToCreate = _mapper.Map<Customer>(request);

            var existingCustomer = _repository.Customer.FindSingle(c => c.Email == request.Email);

            if(existingCustomer != null) 
            {
                throw new BusinessException("Customer already exists");
            }

            using (_repository.UseTransaction())
            {
                _repository.Customer.Create(customerToCreate);
            }

            var result = _mapper.Map<CustomerModel>(customerToCreate);

            return Task.FromResult(result);
        }
    }
}
