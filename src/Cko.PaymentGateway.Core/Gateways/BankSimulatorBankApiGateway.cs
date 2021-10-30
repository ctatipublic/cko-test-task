using Cko.Common.Infrastructure.DomainModel;
using Cko.PaymentGateway.Infrastructure.Interfaces.Gateways;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cko.PaymentGateway.Core.Gateways
{
    public class BankSimulatorBankApiGateway : IBankApiGateway
    {
        private HttpClient _httpClient;

        public BankSimulatorBankApiGateway(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(nameof(BankSimulatorBankApiGateway));
        }

        public Task<TransactionResult> ProcessTransactionAsync(Transaction transaction)
        {
            throw new System.NotImplementedException();
        }
    }
}
