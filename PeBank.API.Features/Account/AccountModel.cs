using PeBank.API.Entities;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PeBank.API.Features
{
    public class AccountModel
    {
        public int Id { get; set; }
        public double Balance { get; set; }
        public int CustomerId { get; set; }
        public AccountType AccountType { get; set; }
        public int AccountTypeId { get; set; }
        [JsonIgnore]
        public IEnumerable<TransactionModel> Transactions { get; set; }
    }
}
