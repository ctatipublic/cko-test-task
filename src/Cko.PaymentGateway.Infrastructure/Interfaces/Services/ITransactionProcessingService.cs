using Cko.Common.Infrastructure.DomainModel;
using Cko.PaymentGateway.Infrastructure.DomainModel;
using System.Threading.Tasks;

namespace Cko.PaymentGateway.Infrastructure.Interfaces.Services
{
    public interface ITransactionProcessingService
    {
        Task<PaymentGatewayTransactionResult> ProcessTransactionAsync(Transaction transaction);
    }
}