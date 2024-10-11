using System.Diagnostics.Contracts;

namespace AccountAndTransactions.Modals.DTO
{
    public class AccountDTO
    {

        public string Name { get; set; }

        public string AccountNumber { get; set; }

        public decimal CurrentBalance { get; set; }

        public decimal OverdraftBalance { get; set; }

    }
}
