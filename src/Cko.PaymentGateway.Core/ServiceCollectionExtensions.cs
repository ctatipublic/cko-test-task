using Cko.Common.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Cko.PaymentGateway.Core
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
