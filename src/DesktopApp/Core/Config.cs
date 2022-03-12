using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace DesktopApp.Core
{
    public static class Config<TConfigFile> where TConfigFile : class, new()
    {
        private const string MacOsApplicationPath = "Library/Application Support";
        private const string ConfigFileName = "Config.json";

        private static string _applicationName;

        public static TConfigFile Values { get; set; } = null!;

        public static void Initialize(string applicationName)
        {
            _applicationName = applicationName;
            Values = GetConfig();
        }

        private static string GetConfigPath()
        {
            string path = RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                ? $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/{MacOsApplicationPath}/{_applicationName}"
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _applicationName);

            Directory.CreateDirectory(path);

            string filePath = Path.Combine(path, ConfigFileName);

            return filePath;
        }

        private static void WriteConfigToFile(TConfigFile config)
        {
            string configSerialized = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(GetConfigPath(), configSerialized);
        }

        private static TConfigFile GetConfig()
        {
            try
            {
                string filePath = GetConfigPath();

                bool fileExists = File.Exists(filePath);

                var currentConfig = new TConfigFile();

                if (fileExists)
                {
                    string rawFile = File.ReadAllText(filePath);
                    var configFile = JsonSerializer.Deserialize<TConfigFile>(rawFile);

                    currentConfig = configFile;
                }
                else
                {
                    WriteConfigToFile(currentConfig);
                }

                return currentConfig;
            }
            catch(Exception)
            {
                return new TConfigFile();
            }
        }

        public static void Update()
        {
            WriteConfigToFile(Values);
        }
    }
}
