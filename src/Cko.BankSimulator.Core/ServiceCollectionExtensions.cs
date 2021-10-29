using Cko.Common.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Cko.BankSimulator.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBankSimulatorCore(this IServiceCollection services)
        {
            services.AddCommonCore();
            return services;
        }
    }
}
