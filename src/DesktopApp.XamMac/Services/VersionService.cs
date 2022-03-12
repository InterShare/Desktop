using DesktopApp.Services;
using Foundation;

namespace DesktopApp.XamMac.Services
{
    public class VersionService : IVersionService
    {
        public string GetVersion()
        {
            return NSBundle.MainBundle.InfoDictionary[new NSString("CFBundleShortVersionString")].ToString();
        }
    }
}