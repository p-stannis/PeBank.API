using PeBank.API.Entities;
using System;

namespace PeBank.API.Features
{
    public class TransactionModel
    {
        public Account Account { get; set; }
        public int AccountId { get; set; }
        public TransactionType TransactionType { get; set; }
        public int TransactionTypeId { get; set; }
        public float Ammount { get; set; }
        public DateTime Date { get; set; }
        public string Details { get; set; }
    }
}
