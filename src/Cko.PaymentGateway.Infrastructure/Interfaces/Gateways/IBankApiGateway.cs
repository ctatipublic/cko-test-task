using Cko.Common.Infrastructure.DomainModel;
using Cko.PaymentGateway.Infrastructure.DomainModel;
using System.Threading.Tasks;

namespace Cko.PaymentGateway.Infrastructure.Interfaces.Gateways
{
    public interface IBankApiGateway
    {
        Task<PaymentGatewayBankTransactionResult> ProcessTransactionAsync(Transaction transaction);
    }
}
