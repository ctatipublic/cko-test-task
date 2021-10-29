using Cko.Common.Infrastructure.DomainModel;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Cko.BankSimulator.Controllers
{
    [Route("transaction")]
    public class TransactionController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> PostTransactionAsync([FromBody] Transaction transaction)
        {
            var isValid = new CreditCardAttribute().IsValid(transaction.From.CardNumber);
            return null;
        }

    }
}
