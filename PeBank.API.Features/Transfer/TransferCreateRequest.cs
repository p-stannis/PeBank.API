using MediatR;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PeBank.API.Features
{
    public class TransferCreateRequest: TransactionBaseRequest, IRequest<IEnumerable<TransactionModel>>
    {
        [Required(ErrorMessage = "{0} is required")]
        public int RecipientCustomerId { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public int RecipientAccountId { get; set; }
    }
}
