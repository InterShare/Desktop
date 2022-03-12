using System;
using DesktopApp.Gtk.Services;
using Eto.Forms;

namespace DesktopApp.Gtk
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            new Application(Eto.Platforms.Gtk).Run(new MainForm(new VersionService()));
        }
    }
}
