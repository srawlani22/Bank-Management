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

        public async Task<string> CreateAccountsWithTransactions(AccountDataDTO accountDataDTO)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Enable IDENTITY_INSERT for Accounts table
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Accounts ON");

                foreach (var accountDTO in accountDataDTO.Accounts)
                {
                    // Create new Account object with explicit id
                    var newAccount = new Account
                    {
                        id = accountDTO.id,  // Set the id manually
                        name = accountDTO.name,
                        accountNumber = accountDTO.accountNumber,
                        currentBalance = accountDTO.currentBalance,
                        overdraftBalance = accountDTO.overdraftBalance,
                        Transactions = new List<Transactioncs>()
                    };

                    // Add account to the context
                    _context.Accounts.Add(newAccount);
                }

                // Perform SaveChanges to insert all accounts
                await _context.SaveChangesAsync();

                // Disable IDENTITY_INSERT for Accounts table
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Accounts OFF");

                // Enable IDENTITY_INSERT for Transactions table
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Transactions ON");

                foreach (var transactionDTO in accountDataDTO.Transactions)
                {
                    // Create new Transaction object with explicit id
                    var newTransaction = new Transactioncs
                    {
                        id = transactionDTO.id,  // Set the id manually
                        accountId = transactionDTO.accountId,  // Ensure accountId matches existing account
                        amount = transactionDTO.amount,
                        description = transactionDTO.description,
                        debitOrCredit = transactionDTO.debitOrCredit,
                        type = transactionDTO.type
                    };

                    // Add transaction to the context
                    _context.Transactions.Add(newTransaction);
                }

                // Perform SaveChanges to insert all transactions
                await _context.SaveChangesAsync();

                // Disable IDENTITY_INSERT for Transactions table
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Transactions OFF");

                // Commit the transaction
                await transaction.CommitAsync();

                return "successfully created accounts and transactions"; // Return success
            }
            catch (Exception ex)
            {
                // Rollback the transaction if any error occurs
                await transaction.RollbackAsync();
                throw;  // Rethrow the exception to handle it as needed
            }
        }


        public async Task<Account> GetAccount(int id)
        {   
            var account = await _context.Accounts.Include(t => t.Transactions).FirstOrDefaultAsync(a => a.id == id);

            if (account == null)
            {
                return null;
                throw new Exception("Account not found");
            }
            else
            {
                return account;
            }
        }

        // function to export account details to excel
        public async Task<string> ExportAccounDetails(int id, string filePath)
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
            worksheet.Cells[2, 1].Value = accountDetails.id;
            worksheet.Cells[2, 2].Value = accountDetails.name;
            worksheet.Cells[2, 3].Value = accountDetails.accountNumber;
            worksheet.Cells[2, 4].Value = accountDetails.currentBalance;
            worksheet.Cells[2, 5].Value = accountDetails.overdraftBalance;

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
                transactionWorksheet.Cells[i + 2, 1].Value = transaction.id;
                transactionWorksheet.Cells[i + 2, 2].Value = transaction.accountId;
                transactionWorksheet.Cells[i + 2, 3].Value = transaction.amount;
                transactionWorksheet.Cells[i + 2, 4].Value = transaction.description;
                transactionWorksheet.Cells[i + 2, 5].Value = transaction.debitOrCredit;
                transactionWorksheet.Cells[i + 2, 6].Value = transaction.type;
            }

            // Set the response type and return the Excel file

            var fullPath = Path.Combine(filePath, $"Account_{id}.xlsx");
            FileInfo fileInfo = new FileInfo(fullPath);
            package.SaveAs(fileInfo); // Save to the specified path

            return fullPath;
        }

        // functions for unit testing
        public async Task<Account> CreateOneAccount(Account accountDTO)
        {
                var account = new Account
                {
                    id = accountDTO.id,
                    accountNumber = accountDTO.accountNumber,
                    currentBalance = accountDTO.currentBalance,
                    overdraftBalance = accountDTO.overdraftBalance,
                    name = accountDTO.name,
                    Transactions = accountDTO.Transactions // Include transactions
                };

                await _context.Accounts.AddAsync(account);
                await _context.SaveChangesAsync();

                return account;

        }


        public async Task UpdateAccount(int accountId, decimal currentBalance, decimal overdraftBalance)
        {
            var account = await GetAccount(accountId);

            if (account == null)
            {
                throw new ArgumentException("Account not found", nameof(accountId));
            }

            account.currentBalance = currentBalance;
            account.overdraftBalance = overdraftBalance;
            

            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }


        public async Task DeleteAccount(int id)
        {
            var account = await _context.Accounts
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.id == id);

            if (account != null)
            {
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
            }
        }
    }
}
