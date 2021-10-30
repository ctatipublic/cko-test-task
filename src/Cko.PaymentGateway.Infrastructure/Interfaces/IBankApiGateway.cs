using Cko.Common.Infrastructure.DomainModel;
using System.Threading.Tasks;

namespace Cko.PaymentGateway.Infrastructure.Interfaces
{
    public interface IBankApiGateway
    {
        Task<TransactionResult> ProcessTransactionAsync(Transaction transaction);
    }
}
