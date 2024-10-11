using Microsoft.EntityFrameworkCore;
using AccountAndTransactions.Modals;

namespace AccountAndTransactions.DAL
{
    public class AppDBContext: DbContext
    {
        public AppDBContext(DbContextOptions options): base(options) 
        {

        }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Transactioncs> Transactions { get; set; }
    }
}
