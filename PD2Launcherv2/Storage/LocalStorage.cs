using Newtonsoft.Json;
using PD2Launcherv2.Interfaces;
using PD2Launcherv2.Models;
using PD2Launcherv2.Models.ProjectDiablo2Launcherv2.Models;
using ProjectDiablo2Launcherv2.Models;
using System.IO;

namespace PD2Launcherv2.Storage
{
    public class LocalStorage : ILocalStorage
    {
        private const string StorageFileName = "launcherSettings.json";
        private readonly string _storageDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData");

        public AllSettings Load()
        {
            string filePath = Path.Combine(_storageDirectory, StorageFileName);
            if (!File.Exists(filePath)) return new AllSettings(); // Or default settings

            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<AllSettings>(json) ?? new AllSettings();
        }

        public void Update<T>(StorageKey key, T value) where T : class
        {
            var settings = Load(); // Load the entire settings to keep other parts intact

            switch (key)
            {
                case StorageKey.LauncherArgs:
                    settings.LauncherArgs = value as LauncherArgs; break;
                case StorageKey.DdrawOptions:
                    settings.DdrawOptions = value as DdrawOptions; break;
                case StorageKey.FileUpdateModel:
                    settings.FileUpdateModel = value as FileUpdateModel; break;
                case StorageKey.FilterStorage:
                    settings.FilterStorage = value as FilterStorage; break;
                case StorageKey.News:
                    settings.News = value as News; break;
                    // Add other cases as needed
            }

            // Check if the directory exists; if not, create it
            if (!Directory.Exists(_storageDirectory))
            {
                Directory.CreateDirectory(_storageDirectory);
            }

            // Serialize the updated settings and save them back to the file
            string filePath = Path.Combine(_storageDirectory, StorageFileName);
            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public T LoadSection<T>(StorageKey key) where T : class
        {
            var settings = Load();
            return key switch
            {
                StorageKey.LauncherArgs => settings.LauncherArgs as T,
                StorageKey.DdrawOptions => settings.DdrawOptions as T,
                StorageKey.FileUpdateModel => settings.FileUpdateModel as T,
                StorageKey.FilterStorage => settings.FilterStorage as T,
                StorageKey.News => settings.News as T,
                _ => default,
            };
        }
    }
}