using System;
using System.ComponentModel.DataAnnotations;

namespace PeBank.API.Entities
{
    public class Transaction : BaseEntity
    {
        public Account Account { get; set; }
        public int AccountId { get; set; }
        public TransactionType TransactionType { get; set; }
        public int TransactionTypeId { get; set; }
        public Operation Operation { get; set; }
        public int OperationId { get; set; }
        public double Ammount { get; set; }
        public DateTime Date { get; set; }
        [StringLength(255)]
        public string Details { get; set; }

    }
}
