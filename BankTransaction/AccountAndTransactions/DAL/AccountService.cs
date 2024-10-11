using AccountAndTransactions.Modals;
using AccountAndTransactions.Modals.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.IO.MemoryMappedFiles;

namespace AccountAndTransactions.DAL
{
    public class AccountService : IAccountService
    {

        private readonly AppDBContext _context;

        public AccountService(AppDBContext context) 
        {
            _context = context;
        }

        public async Task<Guid> CreatAccount(AccountDTO accountDTO)
        {
            var newAccount = new Account
            {
                Name = accountDTO.Name,
                AccountNumber = accountDTO.AccountNumber,
                CurrentBalance = accountDTO.CurrentBalance,
                OverdraftBalance = accountDTO.OverdraftBalance,
                Transactions = new List<Transactioncs>()
            };
            // add to the context
            _context.Accounts.Add(newAccount);
            await _context.SaveChangesAsync();

            // Insert into Transaction
            var intialTransaction = new Transactioncs
            {
                AccountId = newAccount.Id,
                Amount = accountDTO.CurrentBalance,
                Description = "Initial Deposit",
                DebitOrCredit = true, // if money deposited(credit), this flag = true
                Type = TransactionType.Deposit            
            };

            _context.Transactions.Add(intialTransaction);
            await _context.SaveChangesAsync();

            return newAccount.Id;
        }

        public async Task<Account> GetAccount(Guid id)
        {
            var account = await _context.Accounts.Include(t => t.Transactions).FirstOrDefaultAsync(a => a.Id == id);

            return account;
        }

        // function to export account details to excel
        public async Task<string> ExportAccounDetails(Guid id, string filePath)
        {
            // Get the account details
            var accountDetails = await GetAccount(id);
            if (accountDetails == null)
            {
                return null; // Return 404 if account not found
            }

            // Create an Excel package
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Account Details");

            // Set the headers
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Name";
            worksheet.Cells[1, 3].Value = "Account Number";
            worksheet.Cells[1, 4].Value = "Current Balance";
            worksheet.Cells[1, 5].Value = "Overdraft Balance";

            // Fill in account details
            worksheet.Cells[2, 1].Value = accountDetails.Id;
            worksheet.Cells[2, 2].Value = accountDetails.Name;
            worksheet.Cells[2, 3].Value = accountDetails.AccountNumber;
            worksheet.Cells[2, 4].Value = accountDetails.CurrentBalance;
            worksheet.Cells[2, 5].Value = accountDetails.OverdraftBalance;

            // Add a new sheet for transactions
            var transactionWorksheet = package.Workbook.Worksheets.Add("Transactions");
            transactionWorksheet.Cells[1, 1].Value = "Transaction ID";
            transactionWorksheet.Cells[1, 2].Value = "Account ID";
            transactionWorksheet.Cells[1, 3].Value = "Amount";
            transactionWorksheet.Cells[1, 4].Value = "Description";
            transactionWorksheet.Cells[1, 5].Value = "Debit/Credit";
            transactionWorksheet.Cells[1, 6].Value = "Type";

            // Fill in transaction details
            for (int i = 0; i < accountDetails.Transactions.Count; i++)
            {
                var transaction = accountDetails.Transactions[i];
                transactionWorksheet.Cells[i + 2, 1].Value = transaction.Id;
                transactionWorksheet.Cells[i + 2, 2].Value = transaction.AccountId;
                transactionWorksheet.Cells[i + 2, 3].Value = transaction.Amount;
                transactionWorksheet.Cells[i + 2, 4].Value = transaction.Description;
                transactionWorksheet.Cells[i + 2, 5].Value = transaction.DebitOrCredit;
                transactionWorksheet.Cells[i + 2, 6].Value = transaction.Type;
            }

            // Set the response type and return the Excel file

            var fullPath = Path.Combine(filePath, $"Account_{id}.xlsx");
            FileInfo fileInfo = new FileInfo(fullPath);
            package.SaveAs(fileInfo); // Save to the specified path

            return fullPath;
        }
    }
}
