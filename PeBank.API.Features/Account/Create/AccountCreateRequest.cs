using MediatR;
using System.ComponentModel.DataAnnotations;

namespace PeBank.API.Features
{
    public class AccountCreateRequest : IRequest<AccountModel>
    {
        [Required(ErrorMessage = "{0} is required")]
        public int? CustomerId { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public int? AccountTypeId { get; set; }
    }
}
