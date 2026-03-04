using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Api.Tests.Helpers;

public static class HttpClientExtensions
{
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public static async Task<T?> PostAsJsonAsync<T>(this HttpClient client, string requestUri, object? content = null)
    {
        var response = await client.PostAsJsonAsync(requestUri, content, JsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
    }

    public static async Task<T?> PutAsJsonAsync<T>(this HttpClient client, string requestUri, object? content = null)
    {
        var response = await client.PutAsJsonAsync(requestUri, content, JsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
    }

}
