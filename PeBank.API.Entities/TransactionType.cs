using System.ComponentModel.DataAnnotations;

namespace PeBank.API.Entities
{
    public class TransactionType : BaseEntity
    {
        [Required]
        [StringLength(5)]
        public string Code { get; set; }
        [Required]
        [StringLength(25)]
        public string Description { get; set; }
        public double? FixedCharge { get; set; }
        public double? PercentCharge { get; set; }
    }
}
