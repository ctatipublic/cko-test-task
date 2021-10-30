using Cko.Common.Core;
using Cko.PaymentGateway.Core.Gateways;
using Cko.PaymentGateway.Core.Services;
using Cko.PaymentGateway.Infrastructure.Interfaces.Gateways;
using Cko.PaymentGateway.Infrastructure.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Cko.PaymentGateway.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPaymentGatewayCore(this IServiceCollection services)
        {
            services.AddSingleton<IAppSettingsService, AppSettingsService>();
            services.AddHttpClient(nameof(BankSimulatorBankApiGateway));
            services.AddScoped<IBankApiGateway, BankSimulatorBankApiGateway>();
            services.AddCommonCore();
            return services;
        }
    }
}
