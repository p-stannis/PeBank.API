using System.ComponentModel.DataAnnotations;

namespace PeBank.API.Features
{
    public class TransactionBaseRequest
    {
        [Required(ErrorMessage = "{0} is required")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public int? AccountId { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public double Ammount { get; set; }

        public string Details { get; set; }
    }
}
