using System.Reflection;
using DesktopApp.Services;

namespace DesktopApp.Wpf.Services
{
    public class VersionService : IVersionService
    {
        public string GetVersion()
        {
            return Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        }
    }
}