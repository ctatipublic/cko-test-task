using Cko.PaymentGateway.Infrastructure.DomainModel;
using System.Threading.Tasks;

namespace Cko.PaymentGateway.Infrastructure.Interfaces.Repository
{
    public interface ITransactionRepository
    {
        Task<PaymentGatewayTransactionResult> GetTransactionAsync(string transactionId);
        Task StoreTransactionAsync(PaymentGatewayTransactionResult transaction);
    }
}
