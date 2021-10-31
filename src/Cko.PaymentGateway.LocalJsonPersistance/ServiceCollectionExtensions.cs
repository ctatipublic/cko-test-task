using Cko.PaymentGateway.Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Cko.PaymentGateway.LocalJsonPersistance
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLocalJsonDocumentPersistance(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IDocumentPersistance<>), typeof(JsonDocumentPersistance<>));
            return services;
        }
    }
}
