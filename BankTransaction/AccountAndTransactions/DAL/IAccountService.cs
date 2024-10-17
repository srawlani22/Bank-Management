using AccountAndTransactions.Modals;
using AccountAndTransactions.Modals.DTO;
using Microsoft.AspNetCore.Mvc;

namespace AccountAndTransactions.DAL
{
    public interface IAccountService
    {
        Task<string> CreateAccountsWithTransactions(AccountDataDTO accountDTO);

        Task<Account> GetAccount(int id);

        Task<string> ExportAccounDetails(int id, string filePath);

        Task<Account> CreateOneAccount(Account accountDTO);

        Task DeleteAccount(int id);

    }
}
