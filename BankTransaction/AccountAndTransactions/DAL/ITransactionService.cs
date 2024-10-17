using AccountAndTransactions.Modals;
using Microsoft.AspNetCore.Mvc;

namespace AccountAndTransactions.DAL
{
    public interface ITransactionService
    {
        Task<Transactioncs> UpdateTransaction(int transactionId, decimal newAmount, string newDescription, string debit_credit);

        Task<Transactioncs> DeleteTransaction(int transactionId);

        Task<Transactioncs> AddTransaction(int accountId, decimal amount, string description, string debitOrCredit);

        Task<Transactioncs[]> GetTransactions(int accountId);
    }
}
