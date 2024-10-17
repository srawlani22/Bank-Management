using System.Transactions;

namespace AccountAndTransactions.Modals
{
    public class Account
    {
        public int id { get; set; }

        public string name { get; set; }

        public string accountNumber { get; set; }

        public decimal currentBalance { get; set; }

        public decimal overdraftBalance { get; set; }

        public List<Transactioncs> Transactions { get; set; }
    }
}
