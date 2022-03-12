using DesktopApp.XamMac.Services;
using Eto.Forms;

namespace DesktopApp.XamMac
{
    class Program
    {
        static void Main(string[] args)
        {
            new Application(Eto.Platforms.XamMac2).Run(new MainForm(new VersionService()));
        }
    }
}
