
using Microsoft.JSInterop;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorApp4.Services
{
    public class StorageService : IStorageService
    {
        private readonly IJSRuntime _jsRuntime;
        private JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };
        public StorageService(IJSRuntime jsRuntime) => _jsRuntime = jsRuntime;

        public async Task<T> GetItemAsync<T>(string key)
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
            if (string.IsNullOrEmpty(json))
            { 
                return default;
            }
            return JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions)!;
        }

        public async Task SetItemAsync<T>(string key, T value)
        {
            var json = JsonSerializer.Serialize(value, _jsonSerializerOptions);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json);

        }
    }
}
