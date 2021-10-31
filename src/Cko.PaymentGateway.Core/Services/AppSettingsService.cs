using Cko.PaymentGateway.Infrastructure.Interfaces.Services;
using System;

namespace Cko.PaymentGateway.Core.Services
{
    public class AppSettingsService : IAppSettingsService
    {
        public string BankApiUrl { get; private set; }
        public bool UseLocalStorage { get; private set; }

        public AppSettingsService()
        {
            BankApiUrl = Environment.GetEnvironmentVariable("BANK_API_URL");

            if (bool.TryParse(Environment.GetEnvironmentVariable("USE_LOCAL_STORAGE"), out bool useLocalStorage))
            {
                UseLocalStorage = useLocalStorage;
            }
        }
    }
}
