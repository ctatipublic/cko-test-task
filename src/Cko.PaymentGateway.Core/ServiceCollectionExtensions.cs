using Cko.Common.Core;
using Cko.PaymentGateway.Core.Gateways;
using Cko.PaymentGateway.Core.Repositories;
using Cko.PaymentGateway.Core.Services;
using Cko.PaymentGateway.DynamoDbPersistance;
using Cko.PaymentGateway.Infrastructure.Interfaces.Gateways;
using Cko.PaymentGateway.Infrastructure.Interfaces.Repository;
using Cko.PaymentGateway.Infrastructure.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Cko.PaymentGateway.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPaymentGatewayCore(this IServiceCollection services)
        {
            var appSettingsService = new AppSettingsService();
            services.AddSingleton<IAppSettingsService>(appSettingsService);
            services.AddHttpClient(nameof(BankSimulatorBankApiGateway))
                .ConfigureHttpClient(options =>
                {
                    options.BaseAddress = new Uri(appSettingsService.BankApiUrl, UriKind.Absolute);
                });
            services.AddScoped<IBankApiGateway, BankSimulatorBankApiGateway>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<ITransactionProcessingService, TransactionProcessingService>();
            services.AddDynamoDbDocumentPersistance();
            services.AddCommonCore();
            return services;
        }
    }
}
