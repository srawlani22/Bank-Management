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
        public async Task<Transactioncs> UpdateTransaction(int transactionId, decimal newAmount, string newDescription, string debit_credit)
        {
            // Find the transaction to update
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.id == transactionId);

            if (transaction == null)
            {
                throw new InvalidOperationException($"Transaction with ID {transactionId} not found.");
            }

            // Find the associated account
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.id == transaction.accountId);

            if (account == null)
            {
                throw new InvalidOperationException($"Account with ID {account.id} not found.");
            }

            // Reverse the old transaction amount
            if (transaction.type == TransactionType.Deposit)
            {
                account.currentBalance -= transaction.amount; // Remove the old credit amount
            }
            else
            {
                account.currentBalance += transaction.amount; // Add the old debit amount back
            }

            // Apply the new transaction amount
            TransactionType newType = debit_credit == "credit" ? TransactionType.Deposit : TransactionType.Withdrawl;
            if (newType == TransactionType.Deposit)
            {
                account.currentBalance += newAmount; // Add the new credit amount
            }
            else
            {
                account.currentBalance -= newAmount; // Subtract the new debit amount
            }


            if (account.currentBalance < account.overdraftBalance)
            {
                throw new InvalidOperationException($"Update would exceed overdraft limit for account {transaction.accountId}.");
            }
            transaction.amount = newAmount; // Update transaction amount
            transaction.description = newDescription; // Update description
            transaction.type = newType;

            // Save changes to the context
            await _context.SaveChangesAsync();

            return transaction; // Update successful
        }

        // function to delete transaction and adjust account
        public async Task<Transactioncs> DeleteTransaction(int transactionId)
        {
            try
            {
                var transaction = await _context.Transactions
                    .FirstOrDefaultAsync(t => t.id == transactionId);

                if (transaction == null)
                {
                    throw new InvalidOperationException($"Transaction with ID {transactionId} not found.");
                }

                var account = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.id == transaction.accountId);

                if (account == null)
                {
                    return null; // Account not found
                }

                // Adjust account balance
                account.currentBalance += transaction.type == TransactionType.Deposit ? -transaction.amount : transaction.amount;

                // Check overdraft limit
                if (account.currentBalance < account.overdraftBalance)
                {
                    return null; // Deletion would exceed overdraft limit
                }

                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();

                return transaction; // Deletion successful
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                throw new InvalidOperationException($"Transaction with ID {transactionId} not found.");
                return null;
            }
        }

        // function to add transaction
        public async Task<Transactioncs> AddTransaction(int accountId, decimal amount, string description, string debitOrCredit)
        {
            try
            {
                var account = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.id == accountId);

                if (account == null)
                {
                    return null; // Account not found
                }

                // Validate debitOrCredit
                if (debitOrCredit.ToLower() != "debit" && debitOrCredit.ToLower() != "credit")
                {
                    return null; // Invalid debit/credit type
                }

                // Calculate new balance
                decimal newBalance = debitOrCredit.ToLower() == "debit"
                    ? account.currentBalance - amount
                    : account.currentBalance + amount;

                // Check overdraft limit
                if (newBalance < account.overdraftBalance)
                {
                    return null; // Transaction would exceed overdraft limit
                }

                // Create new transaction
                var transaction = new Transactioncs
                {
                    accountId = accountId,
                    amount = amount,
                    description = description,
                    debitOrCredit = debitOrCredit,
                    type = debitOrCredit.ToLower() == "debit" ? TransactionType.Withdrawl : TransactionType.Deposit
                };

                _context.Transactions.Add(transaction);
                account.currentBalance = newBalance;
                await _context.SaveChangesAsync();

                return transaction; // Transaction added successfully
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine($"Error adding transaction: {ex.Message}");
                return null;
            }
        }

        /*
         GET's all the transactions for an account
         */
        public async Task<Transactioncs[]> GetTransactions(int accountId)
        {
            var transactions = await _context.Transactions
                .Where(t => t.accountId == accountId)
                .ToArrayAsync();

            return transactions;
        }
    }
}
