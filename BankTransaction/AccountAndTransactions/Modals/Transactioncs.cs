namespace AccountAndTransactions.Modals
{
    public class Transactioncs
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        public decimal Amount { get; set; }

        public string Description { get; set; }

        public bool DebitOrCredit { get; set; }

        public TransactionType Type { get; set; }
    }
}
