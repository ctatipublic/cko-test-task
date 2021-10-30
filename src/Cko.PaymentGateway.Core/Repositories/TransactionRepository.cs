using Cko.PaymentGateway.Infrastructure.DomainModel;
using Cko.PaymentGateway.Infrastructure.Interfaces;
using Cko.PaymentGateway.Infrastructure.Interfaces.Repository;
using System.Threading.Tasks;

namespace Cko.PaymentGateway.Core.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IDocumentPersistance<PaymentGatewayTransactionResult> _persistance;

        public TransactionRepository(IDocumentPersistance<PaymentGatewayTransactionResult> persistance)
        {
            _persistance = persistance;
        }
        public async Task<PaymentGatewayTransactionResult> GetTransactionAsync(string transactionId)
        {
            return await _persistance.RetrieveDocumentByKeyAsync(transactionId);
        }

        public async Task StoreTransactionAsync(PaymentGatewayTransactionResult transaction)
        {
            await _persistance.StoreDocumentAsync(transaction);
        }
    }
}
