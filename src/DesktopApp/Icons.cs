using Eto.Drawing;

namespace DesktopApp
{
    public class Icons
    {
        public static Icon AddressBookIcon { get; } = Icon.FromResource("DesktopApp.Resources.address-book.png");
        public static Icon SendIcon { get; } = Icon.FromResource("DesktopApp.Resources.send.png");
        public static Icon GearIcon { get; } = Icon.FromResource("DesktopApp.Resources.gear.png");
        public static Icon FileIcon { get; } = Icon.FromResource("DesktopApp.Resources.file.png");
        public static Icon ClipboardIcon { get; } = Icon.FromResource("DesktopApp.Resources.clipboard.png");
    }
}