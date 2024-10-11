using AccountAndTransactions.DAL;
using AccountAndTransactions.Modals;
using AccountAndTransactions.Modals.DTO;
using Microsoft.AspNetCore.Mvc;

namespace AccountAndTransactions.Controllers
{
    [Route("account/api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        public async Task<Guid> CreateAccount(AccountDTO accountDTO)
        {
            var accountId = await _accountService.CreatAccount(accountDTO);
            return accountId;
        }

        [HttpGet]
        public async Task<Account> GetAccount(Guid id)
        {
            var accountDetails = await _accountService.GetAccount(id);
            return accountDetails;
        }

        [HttpGet("export/{id}")] // Route to export account details
        public async Task<IActionResult> ExportAccountToExcel(Guid id)
        {
            string tempPath = "C:\\Users\\Sparsh\\Desktop";
            var filePath = await _accountService.ExportAccounDetails(id, tempPath);
            if (filePath == null)
            {
                return NotFound(); // Return 404 if the account was not found
            }

            var excelName = Path.GetFileName(filePath);
            return PhysicalFile(filePath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
    }
}
