using AccountAndTransactions.DAL;
using AccountAndTransactions.Modals.DTO;
using AccountAndTransactions.Modals;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

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
        public async Task<bool> UpdateTransaction(Guid transactionId, decimal amount, string description, bool debit_credit)
        {
            var isUpdated = await _transactionService.UpdateTransaction(transactionId, amount, description, debit_credit);
            return isUpdated;
        }

        [HttpDelete] // Route for deleting a transaction
        public async Task<bool> DeleteTransaction(Guid transactionId)
        {
            var isDeleted = await _transactionService.DeleteTransaction(transactionId);
            return isDeleted;
        }

        [HttpPost]
        public async Task<IActionResult> AddTransaction(Guid accountId, decimal amount, string description, TransactionType transactionType)
        {
            var isAdded = await _transactionService.AddTransaction(accountId, amount, description, transactionType);
            if (isAdded)
            {
                return CreatedAtAction(nameof(AddTransaction), new { accountId }, null); // Return 201 Created
            }
            return NotFound(); // Return 404 if the account was not found
        }
    }
}
