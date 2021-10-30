using Cko.Common.Infrastructure.DomainModel;
using Cko.PaymentGateway.Infrastructure.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cko.PaymentGateway.Controllers
{
    [Route("process")]
    public class ProcessController : Controller
    {
        private readonly ITransactionProcessingService _transactionProcessingService;

        public ProcessController(ITransactionProcessingService transactionProcessingService)
        {
            _transactionProcessingService = transactionProcessingService;
        }

        [HttpPost]
        public async Task<IActionResult> PostProcessAsync([FromBody] Transaction transaction)
        {
            var transactionResult = await _transactionProcessingService.ProcessTransactionAsync(transaction);
            Response.StatusCode = transactionResult.TransactionStatus < 0 ? 412 : 202;
            return new ObjectResult(transactionResult);
        }
    }
}
