using Cko.PaymentGateway.Infrastructure.Interfaces.Services;
using System;

namespace Cko.PaymentGateway.Core.Services
{
    public class AppSettingsService : IAppSettingsService
    {
        public string BankApiUrl { get; set; }
        public AppSettingsService()
        {
            BankApiUrl = Environment.GetEnvironmentVariable("BANK_API_URL");
        }
    }
}
