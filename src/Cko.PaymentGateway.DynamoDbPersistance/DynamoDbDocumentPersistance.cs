using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Cko.Common.Infrastructure.Helpers;
using Cko.PaymentGateway.DynamoDbPersistance.Interfaces;
using Cko.PaymentGateway.Infrastructure.Interfaces;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cko.PaymentGateway.DynamoDbPersistance
{
    public class DynamoDbDocumentPersistance<T> : IDocumentPersistance<T> where T : class
    {
        private readonly IAmazonDynamoDB _client;
        private readonly IDynamoDbSettings _dynamoDbSettings;

        public DynamoDbDocumentPersistance(IAmazonDynamoDB client, IDynamoDbSettings dynamoDbSettings)
        {
            _client = client;
            _dynamoDbSettings = dynamoDbSettings;
        }

        public async Task<T> RetrieveDocumentByKeyAsync(string keyField, string keyValue)
        {
            var table = Table.LoadTable(_client, _dynamoDbSettings.TableName);
            var doc = await table.GetItemAsync(keyValue);
            if (doc == null) { return null; }
            return JsonSerializer.Deserialize<T>(doc.ToJson(), Constants.JsonSerializerOptions);
        }

        public async Task StoreDocumentAsync(T document)
        {
            var table = Table.LoadTable(_client, _dynamoDbSettings.TableName);
            var dynamoDbDocument = Document.FromJson(JsonSerializer.Serialize(document, Constants.JsonSerializerOptions));
            var batchWrite = table.CreateBatchWrite();
            batchWrite.AddDocumentToPut(dynamoDbDocument);
            await batchWrite.ExecuteAsync();
        }
    }
}
