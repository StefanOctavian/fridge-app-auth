using System.Text.Json;
using System.Text.Json.Serialization;

using Auth.Errors;

namespace Auth.Extensions;

public static class HttpExtensions {
    private static readonly JsonSerializerOptions jsonSerializerOptions = new() {
        PropertyNameCaseInsensitive = true,
        Converters = {
            new JsonStringEnumConverter()
        }
    };

    public static async Task<T> FromJson<T>(
        this Task<HttpResponseMessage> reqTask, 
        bool dontThrow = false,
        bool dontDeserialize = false
    )
    {
        var response = await reqTask;
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode) {
            if (dontThrow) return default!;
            try {
                var error = JsonSerializer.Deserialize<ServiceError>(json, jsonSerializerOptions);
                throw new ServerException(response.StatusCode, error!.Message);
            } catch (JsonException) {
                throw new ServerException(response.StatusCode, response.ReasonPhrase ?? "Unknown error");
            }
        }

        if (dontDeserialize) return (T)(object)json;
        return JsonSerializer.Deserialize<T>(json, jsonSerializerOptions)
            ?? throw new InternalServerErrorException(
                cause: new("Failed to deserialize response")
            );
    }

    public static Task Unpack(this Task<HttpResponseMessage> reqTask) 
        => FromJson<string>(reqTask, dontDeserialize: true);

    private static StringContent ToJson(object? body) => new(
        JsonSerializer.Serialize(body, jsonSerializerOptions),
        System.Text.Encoding.UTF8,
        "application/json"
    );

    public static Task<HttpResponseMessage> PatchAsync(this HttpClient client, string url, object? body = null)
        => client.PatchAsync(url, ToJson(body));

    public static Task<HttpResponseMessage> PostAsync(this HttpClient client, string url, object? body = null)
        => client.PostAsync(url, ToJson(body));

    public static Task<HttpResponseMessage> PutAsync(this HttpClient client, string url, object? body = null)
        => client.PutAsync(url, ToJson(body));
}