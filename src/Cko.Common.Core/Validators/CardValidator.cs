using Cko.Common.Infrastructure.DomainModel;
using Cko.Common.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Cko.Common.Core.Validators
{
    public class CreditCardValidator : IValidator<CardDetails>
    {
        private readonly IStaticValuesProvider _staticValuesProvider;

        public static class ErrorCodes
        {
            public const string InvalidCardDetails = "CARD_DETAILS";
            public const string CardNumber = "CARD_NUMBER";
            public const string Cvv = "CVV";
            public const string ExpiryDate = "EXPIRY";
            public const string CardHoldersName = "NAME";
        }

        public CreditCardValidator(IStaticValuesProvider staticValuesProvider)
        {
            _staticValuesProvider = staticValuesProvider;
        }

        public Task<IEnumerable<string>> ValidateAsync(CardDetails value)
        {
            var errors = new List<string>();
            if (value == null) { errors.Add(ErrorCodes.InvalidCardDetails); }
            else if (!errors.Any())
            {
                if (!ValidateCardNumber(value.CardNumber)) { errors.Add(ErrorCodes.CardNumber); };
                if (!ValidateCvv(value.Cvv)) { errors.Add(ErrorCodes.Cvv); }
                if (!ValidateExpiry(value.ExpiryDate)) { errors.Add(ErrorCodes.ExpiryDate); }
                if (!ValidateName(value.CardHoldersName)) { errors.Add(ErrorCodes.CardHoldersName); }
            }

            return Task.FromResult(errors as IEnumerable<string>);
        }

        private bool ValidateCardNumber(string cardNumber)
        {
            // based on https://github.com/aspnet/AspNetWebStack/blob/main/src/Microsoft.Web.Mvc/CreditCardAttribute.cs

            if (cardNumber == null)
            {
                return false;
            }
            cardNumber = cardNumber.Replace("-", String.Empty);

            int checksum = 0;
            bool evenDigit = false;

            foreach (char digit in cardNumber.Reverse())
            {
                if (!Char.IsDigit(digit))
                {
                    return false;
                }

                int digitValue = (digit - '0') * (evenDigit ? 2 : 1);
                evenDigit = !evenDigit;

                while (digitValue > 0)
                {
                    checksum += digitValue % 10;
                    digitValue /= 10;
                }
            }
            return (checksum % 10) == 0;
        }

        private bool ValidateCvv(string cvv)
        {
            // 3 or 4 digits long
            return !string.IsNullOrWhiteSpace(cvv) && (cvv.Length == 3 || cvv.Length == 4) && cvv.Select(c => c).All(c => char.IsDigit(c));
        }

        private bool ValidateExpiry(string expiryDate)
        {
            if (DateTime.TryParseExact(expiryDate, "MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
            {
                return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc) <= _staticValuesProvider.GetUtcNow();
            }
            return false;
        }

        private bool ValidateName(string name)
        {
            return !string.IsNullOrEmpty(name) && name.Length >= 4;
        }
    }
}
