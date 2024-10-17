using AccountAndTransactions.DAL;
using AccountAndTransactions.Modals.DTO;
using AccountAndTransactions.Modals;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AccountAndTransactions.Controllers
{
    [Route("transaction/api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPut]
        public async Task<Transactioncs> UpdateTransaction(int transactionId, decimal amount, string description, string debit_credit)
        {
            debit_credit = debit_credit.ToLower();
            // Validate the debit_credit value
            if (debit_credit != "credit" && debit_credit != "debit")
            {
                return null;
            }

            // Call the service method to update the transaction
            var isUpdated = await _transactionService.UpdateTransaction(transactionId, amount, description, debit_credit);

            return isUpdated;
        }

        [HttpDelete] // Route for deleting a transaction
        public async Task<Transactioncs> DeleteTransaction(int transactionId)
        {
            var isDeleted = await _transactionService.DeleteTransaction(transactionId);
            return isDeleted;
        }

        [HttpPost]
        public async Task<IActionResult> AddTransaction(int accountId, decimal amount, string description, string debitOrCredit)
        {
            var isAdded = await _transactionService.AddTransaction(accountId, amount, description, debitOrCredit);
            if (isAdded != null)
            {
                return CreatedAtAction(nameof(AddTransaction), new { accountId }, null); // Return 201 Created
            }
            return NotFound(); // Return 404 if the account was not found
        }

        [HttpGet("all/transactions")]
        public async Task<IActionResult> GetTransactions(int accountId)
        {
            var transactions = await _transactionService.GetTransactions(accountId);

            if (transactions == null || transactions.Length == 0)
            {
                return NotFound(); // Return 404 if no transactions found
            }

            return Ok(transactions); // Return 200 with the transactions
        }

    }
}
