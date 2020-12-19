using Microsoft.EntityFrameworkCore;

namespace PeBank.API.Entities
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionType> TransactionType { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                entityType.SetTableName(entityType.DisplayName());
            }

            modelBuilder.Entity<Account>()
                .HasIndex(a => new { a.CustomerId, a.AccountTypeId })
                .IsUnique();


            modelBuilder.Entity<AccountType>().HasData(
               new AccountType
               {
                   Id = 1,
                   Code = "CC",
                   Description = "Checking Account"
               },
               new AccountType
               {
                   Id = 2,
                   Code = "S",
                   Description = "Savings"
               });

            modelBuilder.Entity<TransactionType>().HasData(
                new TransactionType
                {
                    Id = 1,
                    Code = "D",
                    Description = "Deposit",
                    PercentCharge = 1,
                },
                new TransactionType
                {
                    Id = 2,
                    Code = "W",
                    Description = "Withdraw",
                    FixedCharge = 4
                },
                new TransactionType
                {
                    Id = 3,
                    Code = "T",
                    Description = "Transfer",
                    FixedCharge = 1,
                });
        }
    }
}
