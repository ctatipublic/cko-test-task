using Cko.BankSimulator.Core.Services;
using Cko.BankSimulator.Core.Validators;
using Cko.BankSimulator.Infrastructure.Interfaces.Services;
using Cko.Common.Core;
using Cko.Common.Infrastructure.DomainModel;
using Cko.Common.Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Cko.BankSimulator.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBankSimulatorCore(this IServiceCollection services)
        {
            services.AddCommonCore();
            services.AddScoped<IValidator<CardDetails>, FraudCardValidator>();
            services.AddScoped<ITransactionService, TransactionService>();
            return services;
        }
    }
}
