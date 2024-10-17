using AccountAndTransactions.DAL;
using Microsoft.EntityFrameworkCore;
using AccountAndTransactions.Modals;

namespace AccountAndTransactions.Tests
{
    public class AccountTests
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
        public async Task AccountService_Get_Account()
        {
            // Arrange
            databaseContext = await GetDBContext();
            var accountService = new AccountService(databaseContext);
            var id = 5;

            // Act
            var result = await accountService.GetAccount(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.id);
        }

        [Fact]
        public async Task AccountService_UpdateAccount_ValidData_UpdatesAccount()
        {
            // Arrange
            databaseContext = await GetDBContext();
            var accountService = new AccountService(databaseContext);
            var id = 5;
            var newCurrentBalance = 200;
            var newOverdraftBalance = 50;

            // Act with overdraft balance
            await accountService.UpdateAccount(id, newCurrentBalance, newOverdraftBalance);

            // Assert
            var updatedAccount = await accountService.GetAccount(id);
            Assert.Equal(newCurrentBalance, updatedAccount.currentBalance);
            Assert.Equal(newOverdraftBalance, updatedAccount.overdraftBalance);
        }

        [Fact]
        public async Task AccountService_CreateAccount_WithTransactions_CreatesAccountSuccessfully()
        {
            // Arrange
            var databaseContext = await GetDBContext(); // Use the existing GetDBContext to initialize
            var accountService = new AccountService(databaseContext);

            var newAccount = new Account()
            {
                name = "John Doe",
                accountNumber = "987654",
                currentBalance = 1500,
                overdraftBalance = 200,
                Transactions = new List<Transactioncs>()
                    {
                        new Transactioncs()
                        {
                            description = "First Deposit",
                            debitOrCredit = "credit",
                            amount = 1500,
                            type = TransactionType.Deposit
                        },
                        new Transactioncs()
                        {
                            description = "First Withdrawal",
                            debitOrCredit = "debit",
                            amount = 100,
                            type = TransactionType.Withdrawl
                        }
                    }
        };

            // Act
            var getNewAccount = await accountService.CreateOneAccount(newAccount); // Assuming this method exists

            // Assert
            var createdAccount = await accountService.GetAccount(getNewAccount.id); // Retrieve the created account
            Assert.NotNull(createdAccount);
            Assert.Equal(getNewAccount.name, createdAccount.name);
            Assert.Equal(getNewAccount.accountNumber, createdAccount.accountNumber);
            Assert.Equal(getNewAccount.currentBalance, createdAccount.currentBalance);
            Assert.Equal(getNewAccount.overdraftBalance, createdAccount.overdraftBalance);
            Assert.Equal(2, createdAccount.Transactions.Count); // Check that two transactions were added

            // Check the details of the transactions
            Assert.Equal("First Deposit", createdAccount.Transactions[0].description);
            Assert.Equal("First Withdrawal", createdAccount.Transactions[1].description);
        }

        [Fact]
        public async Task AccountService_DeleteAccount_RemovesAccountAndTransactions_ForAccountId5()
        {
            // Arrange
            var databaseContext = await GetDBContext();
            var accountService = new AccountService(databaseContext);

            var getAccount = await accountService.GetAccount(5);
            
            // Act
            await accountService.DeleteAccount(5);

            var checkDeletedAccount = await accountService.GetAccount(5);

            //Assert
            Assert.Null(checkDeletedAccount);
            var transactions = await databaseContext.Transactions
                .Where(t => t.accountId == 5)
                .ToListAsync();
            Assert.Empty(transactions);
        }

    }
}
