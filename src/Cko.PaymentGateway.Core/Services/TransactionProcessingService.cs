using Cko.Common.Infrastructure.DomainModel;
using Cko.Common.Infrastructure.Interfaces;
using Cko.PaymentGateway.Infrastructure.DomainModel;
using Cko.PaymentGateway.Infrastructure.Interfaces.Gateways;
using Cko.PaymentGateway.Infrastructure.Interfaces.Repository;
using Cko.PaymentGateway.Infrastructure.Interfaces.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cko.PaymentGateway.Core.Services
{
    public class TransactionProcessingService : ITransactionProcessingService
    {
        private readonly IBankApiGateway _bankApiGateway;
        private readonly IStaticValuesProvider _staticValuesProvider;
        private readonly IEnumerable<ITransactionRepository> _transactionRepositories;

        public TransactionProcessingService(IBankApiGateway bankApiGateway, IStaticValuesProvider staticValuesProvider, IEnumerable<ITransactionRepository> transactionRepositories)
        {
            _bankApiGateway = bankApiGateway;
            _staticValuesProvider = staticValuesProvider;
            _transactionRepositories = transactionRepositories;
        }

        public async Task<PaymentGatewayTransactionResult> ProcessTransactionAsync(Transaction transaction)
        {
            var result = new PaymentGatewayTransactionResult
            {
                OriginalTransaction = transaction,
                TransactionDateUtc = _staticValuesProvider.GetUtcNow(),
                TransactionId = _staticValuesProvider.GetGuid().ToString(),
            };

            result.BankTransactionResult = await _bankApiGateway.ProcessTransactionAsync(transaction);
            result.OriginalTransaction = transaction;

            var persistanceTasks = _transactionRepositories.Select(r => r.StoreTransactionAsync(result));
            await Task.WhenAll(persistanceTasks);

            return result;
        }
    }
}
