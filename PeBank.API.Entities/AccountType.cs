﻿using System.ComponentModel.DataAnnotations;

namespace PeBank.API.Entities
{
    public class AccountType : BaseEntity
    {
        [Required]
        [StringLength(1)]
        public string Code { get; set; }
        [Required]
        [StringLength(25)]
        public string Description { get; set; }
    }
}
