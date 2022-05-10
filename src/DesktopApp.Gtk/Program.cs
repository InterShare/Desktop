using System;
using DesktopApp.Gtk.Services;
using Gtk;
using Application = Eto.Forms.Application;

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
