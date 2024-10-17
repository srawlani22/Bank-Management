using Microsoft.EntityFrameworkCore;
using AccountAndTransactions.Modals;

namespace AccountAndTransactions.DAL
{
    public class AppDBContext: DbContext
    {
        public AppDBContext(DbContextOptions options): base(options) 
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transactioncs>()
                .HasKey(t => t.id); // Set the primary key


            modelBuilder.Entity<Account>()
                .HasKey(t => t.id); // Set the primary key

        }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Transactioncs> Transactions { get; set; }
    }

}
