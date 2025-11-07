using Microsoft.JSInterop;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorApp4.Services
{
    /// <summary>
    /// Handles saving, loading, and exporting data using the browser's localStorage.
    /// </summary>
    public class StorageService : IStorageService
    {
        private readonly IJSRuntime _jsRuntime;

        private JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        /// <summary>
        /// Creates a new instance of the StorageService.
        /// </summary>
        /// <param name="jsRuntime">JS runtime used for interacting with localStorage.</param>
        public StorageService(IJSRuntime jsRuntime) => _jsRuntime = jsRuntime;

        /// <summary>
        /// Gets and deserializes a value from localStorage.
        /// </summary>
        /// <typeparam name="T">The type of the object to retrieve.</typeparam>
        /// <param name="key">The key under which the object is stored.</param>
        /// <returns>The deserialized object or default if not found.</returns>
        public async Task<T> GetItemAsync<T>(string key)
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
            if (string.IsNullOrEmpty(json))
            {
                Console.WriteLine($"No item found in localStorage for key '{key}'.");
                return default;
            }

            Console.WriteLine($"Retrieved item from localStorage for key '{key}'.");
            return JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions)!;
        }

        /// <summary>
        /// Serializes and saves an object to localStorage.
        /// </summary>
        /// <typeparam name="T">The type of the object to store.</typeparam>
        /// <param name="key">The key to store the object under.</param>
        /// <param name="value">The object to store.</param>
        public async Task SetItemAsync<T>(string key, T value)
        {
            var json = JsonSerializer.Serialize(value, _jsonSerializerOptions);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json);
            Console.WriteLine($"Saved item to localStorage for key '{key}'.");
        }

        /// <summary>
        /// Stores a plain string value in localStorage.
        /// </summary>
        /// <param name="key">The key to store the value under.</param>
        /// <param name="value">The string value to store.</param>
        public async Task SetItemAsStringAsync(string key, string value)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
            Console.WriteLine($"Saved string to localStorage for key '{key}'.");
        }

        /// <summary>
        /// Retrieves a plain string value from localStorage.
        /// </summary>
        /// <param name="key">The key of the stored value.</param>
        /// <returns>The stored string or an empty string if not found.</returns>
        public async Task<string> GetItemAsStringAsync(string key)
        {
            var value = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
            if (value == null)
            {
                Console.WriteLine($"No string found in localStorage for key '{key}'.");
            }
            else
            {
                Console.WriteLine($"Retrieved string from localStorage for key '{key}'.");
            }

            return value ?? string.Empty;
        }

        /// <summary>
        /// Serializes the given object to pretty JSON and triggers a file download in the browser.
        /// </summary>
        /// <typeparam name="T">The type of the object to export.</typeparam>
        /// <param name="fileName">The name of the file to create.</param>
        /// <param name="data">The data to serialize and export.</param>
        public async Task ExportToFileAsync<T>(string fileName, T data)
        {
            try
            {
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters = { new JsonStringEnumConverter() },
                    WriteIndented = true
                };

                var json = JsonSerializer.Serialize(data, jsonOptions);

                var bytes = System.Text.Encoding.UTF8.GetBytes(json);
                var base64 = Convert.ToBase64String(bytes);

                await _jsRuntime.InvokeVoidAsync("downloadFileFromBlazor", fileName, base64);

                Console.WriteLine($"Exported data to file '{fileName}'.");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON serialization error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Export error: {ex.Message}");
                throw;
            }
        }
    }
}
