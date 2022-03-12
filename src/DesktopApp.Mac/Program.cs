using System;
using DesktopApp.Mac.Services;
using Eto.Forms;

namespace DesktopApp.Mac
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            new Application(Eto.Platforms.Mac64).Run(new MainForm(new VersionService()));
        }
    }
}
