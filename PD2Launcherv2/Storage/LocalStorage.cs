
using Newtonsoft.Json;
using PD2Launcherv2.Interfaces;
using PD2Launcherv2.Models;
using System.IO;

namespace PD2Launcherv2.Storage
{
    /// <summary>
    /// Provides local storage functionality for the application settings.
    /// Implements the <see cref="ILocalStorage"/> interface.
    /// </summary>
    public class LocalStorage : ILocalStorage
    {
        /// <summary>
        /// The file name for storing the launcher settings.
        /// </summary>
        private const string StorageFileName = "launcherSettings.json";

        /// <summary>
        /// The directory path where the storage file is located.
        /// </summary>
        private readonly string _storageDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalStorage"/> class.
        /// Sets the storage directory path to the 'AppData' folder within the current domain's base directory.
        /// </summary>
        public LocalStorage()
        {
            _storageDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData");
        }

        /// <summary>
        /// Saves the provided application settings to the local storage file.
        /// </summary>
        /// <param name="settings">The settings to save.</param>
        public void Save(AllSettings settings)
        {
            // Ensures that the storage directory exists, creates it if it doesn't.
            if (!Directory.Exists(_storageDirectory))
            {
                Directory.CreateDirectory(_storageDirectory);
            }

            // Combines the directory path and file name to get the full path.
            string filePath = Path.Combine(_storageDirectory, StorageFileName);
            // Serializes the settings object to JSON with indented formatting for readability.
            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            // Writes the JSON string to the file, overwriting any existing content.
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Loads the application settings from the local storage file.
        /// </summary>
        /// <returns>An <see cref="AllSettings"/> instance containing the loaded settings.</returns>
        public AllSettings Load()
        {
            // Combines the directory path and file name to get the full path.
            string filePath = Path.Combine(_storageDirectory, StorageFileName);
            // Checks if the file exists. If not, returns a new instance of settings.
            if (!File.Exists(filePath))
            {
                return new AllSettings(); // Alternatively, return a default settings object.
            }

            // Reads the JSON string from the file.
            string json = File.ReadAllText(filePath);
            // Deserializes the JSON string back into an AllSettings object.
            // Returns a new instance of settings if deserialization returns null.
            return JsonConvert.DeserializeObject<AllSettings>(json) ?? new AllSettings();
        }
    }
}