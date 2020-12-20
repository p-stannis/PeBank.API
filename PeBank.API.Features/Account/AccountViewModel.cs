namespace PeBank.API.Features
{
    public class AccountViewModel
    {
        public int Id { get; set; }
        public double Balance { get; set; }
        public int CustomerId { get; set; }
        public int AccountTypeId { get; set; }
    }
}
