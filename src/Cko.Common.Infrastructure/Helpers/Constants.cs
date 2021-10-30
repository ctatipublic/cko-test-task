using System.Text.Json;

namespace Cko.Common.Infrastructure.Helpers
{
    public static class Constants
    {
        public static JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    }
}
