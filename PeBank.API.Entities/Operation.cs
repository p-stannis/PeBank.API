using System;
using System.ComponentModel.DataAnnotations;

namespace PeBank.API.Entities
{
    public class Operation : BaseEntity
    {
        public DateTime Date { get; set; }
        [StringLength(255)]
        public string Description { get; set; }
    }
}
