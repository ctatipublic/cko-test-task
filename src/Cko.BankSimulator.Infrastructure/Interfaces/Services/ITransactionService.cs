using Cko.Common.Infrastructure.DomainModel;
using System.Threading.Tasks;

namespace Cko.BankSimulator.Infrastructure.Interfaces.Services
{
    public interface ITransactionService
    {
        Task<BankTransactionResult> ProcessTransactionAsync(Transaction transaction);
    }
}
