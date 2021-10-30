using Cko.BankSimulator.Infrastructure.Interfaces.Services;
using Cko.Common.Infrastructure.DomainModel;
using Cko.Common.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cko.BankSimulator.Core.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IEnumerable<IValidator<CardDetails>> _validators;
        private readonly IStaticValuesProvider _staticValuesProvider;

        public TransactionService(IEnumerable<IValidator<CardDetails>> validators, IStaticValuesProvider staticValuesProvider)
        {
            _validators = validators;
            _staticValuesProvider = staticValuesProvider;
        }

        public async Task<TransactionResult> ProcessTransactionAsync(Transaction transaction)
        {
            var result = new TransactionResult
            {
                TransactionId = _staticValuesProvider.GetGuid().ToString(),
                FromErrorReasons = await ProcessCardValidationAsync(transaction.From),
                ToErrorReasons = await ProcessCardValidationAsync(transaction.To)
            };

            // TODO: Process actual transaction

            return result;
        }

        private async Task<IEnumerable<string>> ProcessCardValidationAsync(CardDetails cardDetails)
        {
            var result = new List<string>();
            foreach (var validator in _validators)
            {
                result.AddRange(await validator.ValidateAsync(cardDetails));
                if (result.Any()) { break; }
            }
            return result;
        }


    }
}
