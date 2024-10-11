namespace AccountAndTransactions.Modals.DTO
{
    public class TransactionDTO
    {

        public string Description { get; set; }

        public string DebitOrCredit { get; set; }

        public decimal Amount { get; set; }

        public Guid AccountId { get; set; }

        public TransactionType Type { get; set; }
    }
}
