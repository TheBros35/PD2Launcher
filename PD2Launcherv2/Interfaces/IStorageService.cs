using Newtonsoft.Json;
using System.IO;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;


namespace PD2Launcherv2.Interfaces
{
    public interface IStorageService
    {
        Task SaveDataAsync<T>(string filename, T data);
        Task<T> LoadDataAsync<T>(string filename);
    }

    public class JsonFileStorageService : IStorageService
    {
        private readonly string _basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        public async Task SaveDataAsync<T>(string filename, T data)
        {
            var filePath = Path.Combine(_basePath, "YourAppName", filename);
            var directory = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task<T> LoadDataAsync<T>(string filename)
        {
            var filePath = Path.Combine(_basePath, "YourAppName", filename);

            if (!File.Exists(filePath))
            {
                return default;
            }

            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}