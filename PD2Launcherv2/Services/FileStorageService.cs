using Newtonsoft.Json;
using PD2Launcherv2.Interfaces;
using System.IO;


namespace PD2Launcherv2.Services
{
    public class FileStorageService : IStorageService
    {
        private readonly string _baseDirectory;

        public FileStorageService(string baseDirectory)
        {
            _baseDirectory = baseDirectory;
        }

        public async Task SaveDataAsync(string fileName, object data)
        {
            string json = JsonConvert.SerializeObject(data);
            string filePath = Path.Combine(_baseDirectory, fileName);
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task<T> LoadDataAsync<T>(string fileName)
        {
            string filePath = Path.Combine(_baseDirectory, fileName);
            if (!File.Exists(filePath))
            {
                return Activator.CreateInstance<T>();
            }

            string json = await File.ReadAllTextAsync(filePath);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public Task SaveDataAsync<T>(string filename, T data)
        {
            throw new NotImplementedException();
        }
    }
}
