using AccountAndTransactions.Modals;
using Microsoft.EntityFrameworkCore;

namespace AccountAndTransactions.DAL
{
    public class TransactionService: ITransactionService
    {
        private readonly AppDBContext _context;

        public TransactionService(AppDBContext context)
        {
            _context = context;
        }

        // function to update transcation
        public async Task<bool> UpdateTransaction(Guid transactionId, decimal newAmount, string newDescription, bool debit_credit)
        {
            // Find the transaction to update
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == transactionId);

            if (transaction == null)
            {
                return false; // Transaction not found
            }

            // Find the associated account
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == transaction.AccountId);

            if (account == null)
            {
                return false; // Account not found
            }

            if (account.CurrentBalance >= account.OverdraftBalance)
            {
                decimal amountDifference = newAmount - transaction.Amount;

                // Update the transaction properties
                // Adjust current balance based on the type of transaction
                if (debit_credit)
                {
                    account.CurrentBalance += amountDifference; // Add new amount
                }
                else
                {
                    account.CurrentBalance -= amountDifference; // Subtract new amount
                }

                transaction.Amount = newAmount; // Update transaction amount
                transaction.Description = newDescription; // Update description
            }

            // Save changes to the context
            await _context.SaveChangesAsync();

            return true; // Update successful
        }

        // function to delete transaction and adjust account
        public async Task<bool> DeleteTransaction(Guid transactionId)
        {
            // Find the transaction to delete
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == transactionId);

            if (transaction == null)
            {
                return false; // Transaction not found
            }

            // Find the associated account
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == transaction.AccountId);

            if (account == null)
            {
                return false; // Account not found
            }

            // Adjust the account balance based on the transaction amount and type
            if (transaction.Type == TransactionType.Deposit) // Assuming a type field exists
            {
                account.CurrentBalance -= transaction.Amount; // Subtract for debit transactions
            }
            else if (transaction.Type == TransactionType.Withdrawl) // Assuming a type field exists
            {
                account.CurrentBalance += transaction.Amount; // Add for credit transactions
            }

            // Remove the transaction from the context
            _context.Transactions.Remove(transaction);

            // Save changes to the context
            await _context.SaveChangesAsync();

            return true; // Deletion successful
        }

        // function to add transaction
        public async Task<bool> AddTransaction(Guid accountId, decimal amount, string description, TransactionType transactionType)
        {
            // Find the account to associate with the transaction
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);

            if (account == null)
            {
                return false; // Account not found
            }

            // Create the new transaction
            var transaction = new Transactioncs
            {
                Id = Guid.NewGuid(),
                Amount = amount,
                Description = description,
                Type = transactionType,
                AccountId = accountId,
            };

            // Update the account balance based on transaction type
            if (transactionType == TransactionType.Deposit)
            {
                account.CurrentBalance += amount; // Add for deposits
            }
            else if (transactionType == TransactionType.Withdrawl)
            {
                account.CurrentBalance -= amount; // Subtract for withdrawals
            }

            // Add the transaction to the context
            await _context.Transactions.AddAsync(transaction);

            // Save changes to the context
            await _context.SaveChangesAsync();

            return true; // Transaction added successfully
        }


    }
}
