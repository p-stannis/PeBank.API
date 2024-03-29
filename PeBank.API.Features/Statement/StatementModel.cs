﻿using System.Collections.Generic;

namespace PeBank.API.Features
{
    public class StatementModel
    {
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public double Balance { get; set; }
        public IEnumerable<TransactionModel> Transactions { get; set; }
    }
}
