using Cko.Common.Infrastructure.DomainModel;
using Cko.Common.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cko.BankSimulator.Core.Validators
{
    /// <summary>
    /// A mock validator to set up card number 0000-0000-0000-0000 as a fraudulent card
    /// </summary>
    public class FraudCardValidator : IValidator<CardDetails>
    {
        private static string[] _fraudulentCards = new[] { "0000-0000-0000-0000" };
        public static class ErrorCodes
        {
            public const string InvalidCardDetails = "CARD_DETAILS";
            public const string Fraudulent = "FRAUD";
        }

        public Task<IEnumerable<string>> ValidateAsync(CardDetails value)
        {
            var result = new List<string>();
            if (value == null) { result.Add(ErrorCodes.InvalidCardDetails); }
            else if (_fraudulentCards.Contains(value.CardNumber)) { result.Add(ErrorCodes.Fraudulent); }
            return Task.FromResult(result.AsEnumerable());
        }
    }
}
