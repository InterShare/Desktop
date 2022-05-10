using System;
using DesktopApp.Mac.Services;
using Eto.Forms;
using MonoMac.AppKit;

namespace DesktopApp.Mac
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {

            Eto.Style.Add<Eto.Mac.Forms.FormHandler>("unified", h =>
            {
                // var effect = new NSVisualEffectView();
                // effect.BlendingMode = NSVisualEffectBlendingMode.BehindWindow;
                // effect.Material = NSVisualEffectMaterial.UnderWindowBackground;
                // effect.State = NSVisualEffectState.Active;
                //
                // h.Control.ContentView = effect;
                MonoMac.ObjCRuntime.Messaging.void_objc_msgSend_IntPtr(h.Control.Handle, MonoMac.ObjCRuntime.Selector.GetHandle("setTitleVisibility:"), (IntPtr)1);
            });

            Eto.Style.Add<Eto.Mac.Forms.ToolBar.ToolBarHandler>(null, h =>
            {
                h.Control.DisplayMode = NSToolbarDisplayMode.Icon;
                h.Control.SizeMode = NSToolbarSizeMode.Small;
            });

            new Application(Eto.Platforms.Mac64).Run(new MainForm(new VersionService()));
        }
    }
}
