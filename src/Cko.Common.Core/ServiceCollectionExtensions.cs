using Cko.Common.Core.Providers;
using Cko.Common.Core.Validators;
using Cko.Common.Infrastructure.DomainModel;
using Cko.Common.Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Cko.Common.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommonCore(this IServiceCollection services)
        {
            services.AddScoped<IValidator<CardDetails>, CardValidator>();
            services.AddSingleton<IStaticValuesProvider, StaticValuesProvider>();
            return services;
        }
    }
}
