using AccountAndTransactions.Modals;

namespace AccountAndTransactions.DAL
{
    public interface ITransactionService
    {
        Task<bool> UpdateTransaction(Guid transactionId, decimal newAmount, string newDescription, bool debit_credit);

        Task<bool> DeleteTransaction(Guid transactionId);

        Task<bool> AddTransaction(Guid accountId, decimal amount, string description, TransactionType transactionType);
    }
}
