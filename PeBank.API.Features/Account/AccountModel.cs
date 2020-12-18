using PeBank.API.Entities;
using System.Collections.Generic;

namespace PeBank.API.Features
{
    public class AccountModel
    {
        public int Id { get; set; }
        public float CurrentBalance { get; set; }
        public Customer Customer { get; set; }
        public int CustomerId { get; set; }
        public AccountType AccountType { get; set; }
        public int AccountTypeId { get; set; }

        public IEnumerable<Transaction> Transactions { get; set; }
    }
}
