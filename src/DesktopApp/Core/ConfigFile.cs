using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using DesktopApp.Entities;
using SMTSP.Entities;

namespace DesktopApp.Core
{
    public class ConfigFile : Notifiable
    {
        private string _downloadPath;
        private string _deviceName;
        private bool _useMdnsForDiscovery = true;
        private List<Contact> _contacts;

        [JsonIgnore]
        public string CliVersion { get; set; }

        [JsonPropertyName("downloadPath")]
        public string DownloadPath
        {
            get => _downloadPath;
            set => Set(ref _downloadPath, value);
        }

        [JsonPropertyName("deviceName")]
        public string DeviceName
        {
            get => _deviceName;
            set => Set(ref _deviceName, value);
        }

        [JsonPropertyName("deviceId")]
        public string DeviceIdentifier { get; set; }

        [JsonPropertyName("useMdnsForDiscovery")]
        public bool UseMdnsForDiscovery
        {
            get => _useMdnsForDiscovery;
            set => Set(ref _useMdnsForDiscovery, value);
        }
        
        [JsonPropertyName("contacts")]
        public List<Contact> Contacts
        {
            get => _contacts;
            set => Set(ref _contacts, value);
        }

        [JsonPropertyName("wasOpenedBefore")]
        public bool WasOpenedBefore { get; set; } = false;

        [JsonIgnore]
        public DeviceInfo MyDeviceInfo { get; set; }


        public ConfigFile()
        {
            CliVersion = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "unknown version";

            string downloadsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            DownloadPath ??= Path.Combine(downloadsFolderPath, "Downloads");

            try
            {
                DeviceName ??= Environment.MachineName;
            }
            catch (InvalidOperationException)
            {
                DeviceName ??= "Unknown-User";
            }

            DeviceIdentifier ??= Guid.NewGuid().ToString();
        }
    }
}