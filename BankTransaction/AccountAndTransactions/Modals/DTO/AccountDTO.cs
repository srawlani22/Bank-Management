using System.Diagnostics.Contracts;

namespace AccountAndTransactions.Modals.DTO
{
    public class AccountDTO
    {
        public int id {  get; set; }

        public string name { get; set; }

        public string accountNumber { get; set; }

        public decimal currentBalance { get; set; }

        public decimal overdraftBalance { get; set; }

    }
}
