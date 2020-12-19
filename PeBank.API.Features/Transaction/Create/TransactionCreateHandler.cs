using AutoMapper;
using MediatR;
using PeBank.API.Contracts;
using PeBank.API.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PeBank.API.Features
{
    public class TransactionCreateHandler : IRequestHandler<TransactionCreateRequest, IEnumerable<TransactionModel>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repository;

        public TransactionCreateHandler(IMapper mapper, IRepositoryWrapper repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public Task<IEnumerable<TransactionModel>> Handle(TransactionCreateRequest request, CancellationToken cancellationToken)
        {
            var transactionsToCreate = _mapper.Map<IEnumerable<Transaction>>(request.Transactions);

            using (_repository.UseTransaction())
            {
                var operation = CreateOperation(request);

                transactionsToCreate.ToList().ForEach(t =>
                {
                    t.Operation = operation;
                });

                _repository.TransactionRepository.CreateMany(transactionsToCreate);
            }

            var result = _mapper.Map<IEnumerable<TransactionModel>>(transactionsToCreate);

            return Task.FromResult(result);
        }

        private Operation CreateOperation(TransactionCreateRequest request)
        {
            var operationToCreate = new Operation
            {
                Date = request.OperationDate,
                Description = request.OperationDetails
            };

            _repository.Operation.Create(operationToCreate);

            return operationToCreate;
        }
    }
}
