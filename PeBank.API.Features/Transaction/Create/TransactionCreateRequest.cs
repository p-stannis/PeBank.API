using MediatR;
using System.ComponentModel.DataAnnotations;

namespace PeBank.API.Features
{
    public class TransactionCreateRequest : IRequest<TransactionModel>
    {
        [Required(ErrorMessage = "{0} is required")]
        public int? AccountId {get;set;}

        [Required(ErrorMessage = "{0} is required")]
        public int? TransactionTypeId { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public float Ammount { get; set; }

        public string Details { get; set; }
    }
}
