using System.Transactions;

namespace AccountAndTransactions.Modals
{
    public class Account
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string AccountNumber { get; set; }

        public decimal CurrentBalance { get; set; }

        public decimal OverdraftBalance { get; set; }

        public List<Transactioncs> Transactions { get; set; }
    }
}
