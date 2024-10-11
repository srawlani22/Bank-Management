using AccountAndTransactions.Modals;
using AccountAndTransactions.Modals.DTO;
using Microsoft.AspNetCore.Mvc;

namespace AccountAndTransactions.DAL
{
    public interface IAccountService
    {
        Task<Guid> CreatAccount(AccountDTO accountDTO);

        Task<Account> GetAccount(Guid id);

        Task<string> ExportAccounDetails(Guid id, string filePath);

    }
}
