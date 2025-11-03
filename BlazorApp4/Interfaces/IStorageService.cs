namespace BlazorApp4.Interfaces
{
    /// <summary>
    /// Defines methods for saving, loading, and exporting data using the browser's localStorage.
    /// </summary>
    public interface IStorageService
    {
        // Saves an object to localStorage.
        Task SetItemAsync<T>(string key, T value);

        // Retrieves and deserializes an object from localStorage.
        Task<T> GetItemAsync<T>(string key);

        // Retrieves a plain string value from localStorage.
        Task<string> GetItemAsStringAsync(string key);

        // Saves a plain string value to localStorage.
        Task SetItemAsStringAsync(string key, string value);

        // Exports the given data as a JSON file and downloads it in the browser.
        Task ExportToFileAsync<T>(string fileName, T data);
    }
}