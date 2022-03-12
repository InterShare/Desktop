using System;
using DesktopApp.Wpf.Services;
using Eto.Forms;

namespace DesktopApp.Wpf
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            new Application(Eto.Platforms.Wpf).Run(new MainForm(new VersionService()));
        }
    }
}
