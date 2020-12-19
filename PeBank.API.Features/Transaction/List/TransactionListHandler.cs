using AutoMapper;
using MediatR;
using PeBank.API.Contracts;
using PeBank.API.Entities;
using PeBank.API.Features.Utils.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PeBank.API.Features
{
    public class TransactionListHandler : IRequestHandler<TransactionListRequest, IEnumerable<TransactionModel>>
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IRepositoryWrapper _repository;

        public TransactionListHandler(IMapper mapper, IMediator mediator, IRepositoryWrapper repository)
        {
            _mapper = mapper;
            _mediator = mediator;
            _repository = repository;
        }

        public Task<IEnumerable<TransactionModel>> Handle(TransactionListRequest request, CancellationToken cancellationToken)
        {
            var transactions = _repository.TransactionRepository.Find(expression: t => t.AccountId == request.AccountId, null, null,
                                                                      includes: new List<string> { "Account" });

            var result = _mapper.Map<IEnumerable<TransactionModel>>(transactions);

            return Task.FromResult(result);
        }
    }
}
