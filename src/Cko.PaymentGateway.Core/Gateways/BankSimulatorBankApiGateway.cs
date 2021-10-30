using Cko.Common.Infrastructure.DomainModel;
using Cko.Common.Infrastructure.Helpers;
using Cko.PaymentGateway.Infrastructure.DomainModel;
using Cko.PaymentGateway.Infrastructure.Interfaces.Gateways;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cko.PaymentGateway.Core.Gateways
{
    public class BankSimulatorBankApiGateway : IBankApiGateway
    {
        private readonly ILogger<BankSimulatorBankApiGateway> _logger;
        private HttpClient _httpClient;

        public static class ErrorCodes
        {
            public static string ConnectionFailed = "CONNECTION_FAILED";
        }

        public BankSimulatorBankApiGateway(IHttpClientFactory httpClientFactory, ILogger<BankSimulatorBankApiGateway> logger)
        {
            _httpClient = httpClientFactory.CreateClient(nameof(BankSimulatorBankApiGateway));
            _logger = logger;
        }

        public async Task<PaymentGatewayBankTransactionResult> ProcessTransactionAsync(Transaction transaction)
        {
            PaymentGatewayBankTransactionResult result = new PaymentGatewayBankTransactionResult();
            try
            {
                var response = await _httpClient.PostAsync("transaction", new StringContent(JsonSerializer.Serialize(transaction, Constants.JsonSerializerOptions), Encoding.UTF8, "application/json"));
                var responseContent = await response.Content.ReadAsStringAsync();
                result.BankTransactionResult = JsonSerializer.Deserialize<BankTransactionResult>(responseContent, Constants.JsonSerializerOptions);
            }
            catch (Exception ex)
            {
                result.ConnectionError = ErrorCodes.ConnectionFailed;
                _logger.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
            return result;
        }
    }
}
