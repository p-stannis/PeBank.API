using System.Collections.Generic;

namespace PeBank.API.Entities
{
    public class Account : BaseEntity
    {
        public Customer Customer { get; set; }
        public int CustomerId { get; set; }
        public AccountType AccountType { get; set; }
        public int AccountTypeId { get; set; }

        public IEnumerable<Transaction> Transactions { get; set; }
    }
}
