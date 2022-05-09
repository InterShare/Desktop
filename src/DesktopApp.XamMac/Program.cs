using AppKit;
using DesktopApp.XamMac.Services;
using Eto.Forms;

namespace DesktopApp.XamMac
{
    class Program
    {
        static void Main(string[] args)
        {
            Eto.Style.Add<Eto.Mac.Forms.FormHandler>("unified", h =>
            {
                // var effect = new NSVisualEffectView();
                // effect.BlendingMode = NSVisualEffectBlendingMode.BehindWindow;
                // effect.Material = NSVisualEffectMaterial.UnderWindowBackground;
                // effect.State = NSVisualEffectState.Active;
                //
                // h.Control.ContentView = effect;
                h.Control.TitlebarAppearsTransparent = true;
                h.Control.ToolbarStyle = NSWindowToolbarStyle.UnifiedCompact;
            });

            Eto.Style.Add<Eto.Mac.Forms.ToolBar.ToolBarHandler>(null, h =>
            {
                h.Control.DisplayMode = NSToolbarDisplayMode.Icon;
                h.Control.SizeMode = NSToolbarSizeMode.Small;
            });

            new Application(Eto.Platforms.XamMac2).Run(new MainForm(new VersionService()));
        }
    }
}
