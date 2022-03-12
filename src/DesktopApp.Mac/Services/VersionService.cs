using DesktopApp.Services;
using MonoMac.Foundation;

namespace DesktopApp.Mac.Services
{
    public class VersionService : IVersionService
    {
        public string GetVersion()
        {
            return NSBundle.MainBundle.InfoDictionary[new NSString("CFBundleShortVersionString")].ToString();
        }
    }
}