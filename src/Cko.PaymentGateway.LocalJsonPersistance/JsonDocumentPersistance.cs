using Cko.Common.Infrastructure.Helpers;
using Cko.PaymentGateway.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cko.PaymentGateway.LocalJsonPersistance
{
    public class JsonDocumentPersistance<T> : IDocumentPersistance<T> where T : class
    {
        private const string _fileName = "CkoTransactionsStore.json";
        private string storePath => $"{Directory.GetCurrentDirectory()}/{_fileName}";

        public async Task<T> RetrieveDocumentByKeyAsync(string keyField, string keyValue)
        {
            T result = null;

            var json = await GetLocalFileJsonAsync();
            if (!string.IsNullOrWhiteSpace(json))
            {
                var d = JsonDocument.Parse(json);
                foreach (var item in d.RootElement.EnumerateArray())
                {
                    if (item.TryGetProperty(keyField, out JsonElement key))
                    {
                        if (key.ToString() == keyValue)
                        {
                            return JsonSerializer.Deserialize<T>(item.ToString(), Constants.JsonSerializerOptions);
                        }
                    }
                }
            }


            return result;
        }

        public async Task StoreDocumentAsync(T obj)
        {
            var contents = (await GetLocalFileContentsAsync()).ToList();
            contents.Add(obj);
            await StoreJsonAsync(contents);
        }

        private async Task<IEnumerable<T>> GetLocalFileContentsAsync()
        {
            var json = await GetLocalFileJsonAsync();
            if (!string.IsNullOrWhiteSpace(json))
            {
                return JsonSerializer.Deserialize<IEnumerable<T>>(json, Constants.JsonSerializerOptions);
            }
            return new List<T>();
        }

        private async Task<string> GetLocalFileJsonAsync()
        {
            if (!File.Exists(storePath))
            {
                using (var file = File.CreateText(storePath)) { }
                return null;
            }
            else
            {
                using (var file = File.OpenText(storePath))
                {
                    return await file.ReadToEndAsync();
                }
            }
        }

        private async Task StoreJsonAsync(IEnumerable<T> contents)
        {
            using (var file = File.CreateText(storePath))
            {
                await file.WriteAsync(JsonSerializer.Serialize(contents, Constants.JsonSerializerOptions));
            }
        }

    }
}
