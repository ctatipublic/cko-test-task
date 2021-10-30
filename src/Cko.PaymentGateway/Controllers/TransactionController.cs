using Cko.Common.Infrastructure.DomainModel;
using Cko.PaymentGateway.Infrastructure.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cko.PaymentGateway.Controllers
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
            var transactionResult = await _transactionService.ProcessTransactionAsync(transaction);
            Response.StatusCode = transactionResult.TransactionStatus < 0 ? 412 : 202;
            return new ObjectResult(transactionResult);
        }

        [HttpGet]
        [Route("{transactionId}")]
        public async Task<IActionResult> GetTransactionAsync(string transactionId)
        {
            var result = await _transactionService.RetrieveTransactionAsync(transactionId);
            if (result == null) { return new NotFoundResult(); }
            return new OkObjectResult(result);
        }
    }
}
