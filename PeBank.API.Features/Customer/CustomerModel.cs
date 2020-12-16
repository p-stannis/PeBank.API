using System.Collections.Generic;

namespace PeBank.API.Features
{
    public class CustomerModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public IEnumerable<AccountModel> Accounts { get; set; }
    }
}
