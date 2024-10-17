using AccountAndTransactions.DAL;
using AccountAndTransactions.Modals;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountAndTransactions.Tests
{
    public class TransactionTests
    {
        private AppDBContext databaseContext;

        private async Task<AppDBContext> GetDBContext()
        {
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new AppDBContext(options);
            context.Database.EnsureCreated();
            context.Accounts.Add(new Account()
            {
                id = 5,
                name = "Sparsh Rawlani",
                accountNumber = "123456",
                currentBalance = 1000,
                overdraftBalance = 100,
                Transactions = new List<Transactioncs>()
                {
                    new Transactioncs() // Add a new Transaction
                    {
                        id = 4,
                        accountId = 5,
                        description = "Initial Deposit",
                        debitOrCredit = "credit",
                        amount = 500,
                        type = TransactionType.Deposit
                    },
                    new Transactioncs() // Add a new Transaction
                    {
                        id = 5,
                        accountId = 5,
                        description = "Initial Withdrawal",
                        debitOrCredit = "debit",
                        amount = 100,
                        type = TransactionType.Withdrawl
                    },
                }
            });
            await context.SaveChangesAsync();

            return context;
        }


        [Fact]
        public async Task TransactionService_AddTransaction_ValidData_AddsTransactionSuccessfully()
        {
            // Arrange
            databaseContext = await GetDBContext();
            var transactionService = new TransactionService(databaseContext);
            var accountId = 5;
            var newTransaction = new Transactioncs()
            {
                description = "Test Transaction",
                debitOrCredit = "credit",
                amount = 200,
                type = TransactionType.Deposit
            };

            // Act
            var addedTransaction = await transactionService.AddTransaction(accountId, newTransaction.amount, newTransaction.description, newTransaction.debitOrCredit);
            // Assert
            var account = await databaseContext.Accounts
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.id == accountId);
            Assert.NotNull(addedTransaction);
            Assert.Equal(newTransaction.description, addedTransaction.description);
            Assert.Equal(newTransaction.amount, addedTransaction.amount);
            Assert.Equal(accountId, addedTransaction.accountId);
            Assert.Equal(account.currentBalance, 1200); // Current balance should increase by 200
        }

        [Fact]
        public async Task TransactionService_UpdateTransaction_ValidData_Credit()
        {
            // Arrange
            databaseContext = await GetDBContext();
            var transactionService = new TransactionService(databaseContext);
            var accountId = 5;
            var transactionId = 4;
            var updatedTransaction = new Transactioncs()
            {
                description = "Updated Transaction",
                debitOrCredit = "credit",
                amount = 300,
                type = TransactionType.Deposit
            };

            // Act
            await transactionService.UpdateTransaction(transactionId, updatedTransaction.amount, updatedTransaction.description, updatedTransaction.debitOrCredit);

            // Assert
            var account = await databaseContext.Accounts
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.id == accountId);
            var updatedTrans = await databaseContext.Transactions
                .FirstOrDefaultAsync(t => t.id == transactionId);
            Assert.Equal(updatedTransaction.description, updatedTrans.description);
            Assert.Equal(updatedTransaction.amount, updatedTrans.amount);
            Assert.Equal(account.currentBalance, 800); // Current balance should increase by 100
        }

        [Fact]
        public async Task TransactionService_DeleteTransaction_ValidData_RemovesTransactionAndUpdatesAccountBalance()
        {
            // Arrange
            databaseContext = await GetDBContext();
            var transactionService = new TransactionService(databaseContext);
            var accountId = 5;
            var transactionId = 4;

            // Act
            await transactionService.DeleteTransaction(transactionId);

            // Assert
            var account = await databaseContext.Accounts
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.id == accountId);
            var deletedTrans = await databaseContext.Transactions
                .FirstOrDefaultAsync(t => t.id == transactionId);
            Assert.Null(deletedTrans);
            Assert.Equal(500, account.currentBalance); // Current balance should decrease by 500
        }

        [Fact]
        public async Task TransactionService_DeleteTransaction_NonExistentTransaction_ThrowsException()
        {
            // Arrange
            databaseContext = await GetDBContext();
            var transactionService = new TransactionService(databaseContext);
            var accountId = 5;
            var transactionId = 999;

            // Act and Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => transactionService.DeleteTransaction(transactionId));
            Assert.Equal($"Transaction with ID {transactionId} not found.", exception.Message);
        }


        [Fact]
        public async Task TransactionService_GetTransaction_ValidData_RetrievesTransaction()
        {
            // Arrange
            databaseContext = await GetDBContext();
            var transactionService = new TransactionService(databaseContext);
            var accountId = 5;
            var transactionId = 4;

            // Act
            var transaction = await transactionService.GetTransactions(accountId);

            // Assert
            // Assert
            Assert.NotEmpty(transaction);
            Assert.Contains(transaction, t => t.id == transactionId && t.accountId == accountId);
        }

        [Fact]
        public async Task TransactionService_UpdateTransaction_ValidData_Debit()
        {
            // Arrange
            databaseContext = await GetDBContext();
            var transactionService = new TransactionService(databaseContext);
            var accountId = 5;
            var transactionId = 4;
            var updatedTransaction = new Transactioncs()
            {
                description = "Updated Transaction",
                debitOrCredit = "debit",
                amount = 50,
                type = TransactionType.Deposit
            };

            // Act
            await transactionService.UpdateTransaction(transactionId, updatedTransaction.amount, updatedTransaction.description, updatedTransaction.debitOrCredit);

            // Assert
            var account = await databaseContext.Accounts
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.id == accountId);
            var updatedTrans = await databaseContext.Transactions
                .FirstOrDefaultAsync(t => t.id == transactionId);
            Assert.Equal(updatedTransaction.description, updatedTrans.description);
            Assert.Equal(updatedTransaction.amount, updatedTrans.amount);
            Assert.Equal(account.currentBalance, 450);
        }


        [Fact]
        public async Task TransactionService_UpdateTransaction_OverdraftLimitExceeded_ThrowsException()
        {
            // Arrange
            databaseContext = await GetDBContext();
            var transactionService = new TransactionService(databaseContext);
            var accountId = 5;
            var transactionId = 4;
            var updatedTransaction = new Transactioncs()
            {
                description = "Updated Transaction",
                debitOrCredit = "debit",
                amount = 10000, // Amount exceeds overdraft limit
                type = TransactionType.Withdrawl
            };

            // Act and Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                transactionService.UpdateTransaction(transactionId, updatedTransaction.amount, updatedTransaction.description, updatedTransaction.debitOrCredit));
        }
    }
}
