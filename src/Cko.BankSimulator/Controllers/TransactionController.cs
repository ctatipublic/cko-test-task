using Cko.BankSimulator.Infrastructure.Interfaces.Services;
using Cko.Common.Infrastructure.DomainModel;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Cko.BankSimulator.Controllers
{
    [Route("transaction")]
    public class TransactionController : Controller
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost]
        public async Task<IActionResult> PostTransactionAsync([FromBody] Transaction transaction)
        {
            var result = await _transactionService.ProcessTransactionAsync(transaction);
            var statusCode = result.FromErrorReasons.Any() || result.ToErrorReasons.Any() ? 412 : 202;

            return new ObjectResult(result) { StatusCode = statusCode };
        }

    }
}
