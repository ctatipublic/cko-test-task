using Amazon.DynamoDBv2;
using Cko.PaymentGateway.DynamoDbPersistance.Interfaces;
using Cko.PaymentGateway.Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Cko.PaymentGateway.DynamoDbPersistance
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDynamoDbPersistance(this IServiceCollection services)
        {
            services.AddSingleton<IDynamoDbSettings>(new DynamoDbSettings(tableName: "CkoTransactions"));
            services.AddAWSService<IAmazonDynamoDB>();
            services.AddSingleton(typeof(IDocumentPersistance<>), typeof(DynamoDbDocumentPersistance<>));
            return services;
        }
    }
}
