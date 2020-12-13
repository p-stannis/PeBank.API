using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PeBank.API.Entities
{
    public class Customer : BaseEntity
    {
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(50)]
        public string Email { get; set; }
        public IEnumerable<Account> Accounts { get; set; }
    }
}
