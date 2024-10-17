namespace AccountAndTransactions.Modals
{
    public class Transactioncs
    {
        public int id { get; set; }

        public int accountId { get; set; }

        public decimal amount { get; set; }

        public string description { get; set; }

        public string debitOrCredit { get; set; }

        public TransactionType type { get; set; }
    }
}
