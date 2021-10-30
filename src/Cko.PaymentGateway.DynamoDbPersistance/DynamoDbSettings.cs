using Cko.PaymentGateway.DynamoDbPersistance.Interfaces;

namespace Cko.PaymentGateway.DynamoDbPersistance
{
    public static partial class ServiceCollectionExtensions
    {
        public class DynamoDbSettings : IDynamoDbSettings
        {
            private readonly string _tableName;

            public DynamoDbSettings(string tableName)
            {
                _tableName = tableName;
            }
            public string TableName => _tableName;
        }
    }
}
