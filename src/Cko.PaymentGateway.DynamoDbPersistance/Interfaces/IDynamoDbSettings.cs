namespace Cko.PaymentGateway.DynamoDbPersistance.Interfaces
{
    public interface IDynamoDbSettings
    {
        string TableName { get; }
    }
}
