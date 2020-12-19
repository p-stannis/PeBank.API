using MediatR;
using PeBank.API.Entities;
using System;
using System.Collections.Generic;

namespace PeBank.API.Features
{
    public class TransactionCreateRequest : IRequest<IEnumerable<TransactionModel>>
    {
       public DateTime OperationDate { get; set; }
       public string OperationDetails { get; set; }
        public IEnumerable<TransactionModel> Transactions { get; set; }
    }
}
