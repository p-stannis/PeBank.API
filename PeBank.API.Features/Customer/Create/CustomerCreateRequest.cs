using MediatR;
using System.ComponentModel.DataAnnotations;

namespace PeBank.API.Features
{
    public class CustomerCreateRequest : IRequest<CustomerModel>
    {
        [Required(ErrorMessage = "{0} is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string Email { get; set; }
    }
}
