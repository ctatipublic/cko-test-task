using Cko.Common.Infrastructure.DomainModel;
using Cko.Common.Infrastructure.Interfaces;
using Cko.PaymentGateway.Infrastructure.DomainModel;
using Cko.PaymentGateway.Infrastructure.Helpers;
using Cko.PaymentGateway.Infrastructure.Interfaces.Gateways;
using Cko.PaymentGateway.Infrastructure.Interfaces.Repository;
using Cko.PaymentGateway.Infrastructure.Interfaces.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cko.PaymentGateway.Core.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IBankApiGateway _bankApiGateway;
        private readonly IStaticValuesProvider _staticValuesProvider;
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(IBankApiGateway bankApiGateway, IStaticValuesProvider staticValuesProvider, ITransactionRepository transactionRepository)
        {
            _bankApiGateway = bankApiGateway;
            _staticValuesProvider = staticValuesProvider;
            _transactionRepository = transactionRepository;
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

            await _transactionRepository.StoreTransactionAsync(result);

            MaskResult(result);
            return result;
        }

        public async Task<PaymentGatewayTransactionResult> RetrieveTransactionAsync(string transactionId)
        {
            var result = await _transactionRepository.GetTransactionAsync(transactionId);
            MaskResult(result);

            return result;
        }

        private void MaskResult(PaymentGatewayTransactionResult result)
        {
            if (result != null)
            {
                result.OriginalTransaction.From.CardNumber = result.OriginalTransaction.From.CardNumber.ReplaceWithStars(4);
                result.OriginalTransaction.From.Cvv = result.OriginalTransaction.From.Cvv.ReplaceWithStars(0);
                result.OriginalTransaction.To.CardNumber = result.OriginalTransaction.To.CardNumber.ReplaceWithStars(4);
                result.OriginalTransaction.To.Cvv = result.OriginalTransaction.To.Cvv.ReplaceWithStars(0);
            }
        }
    }
}
