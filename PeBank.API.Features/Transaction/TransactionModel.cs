using PeBank.API.Entities;
using System;

namespace PeBank.API.Features
{
    public class TransactionModel
    {
        public int AccountId { get; set; }
        public TransactionType TransactionType { get; set; }
        public int TransactionTypeId { get; set; }
        public int OperationId { get; set; }
        public double Ammount { get; set; }
        public DateTime Date { get; set; }
        public string Details { get; set; }
    }
}
