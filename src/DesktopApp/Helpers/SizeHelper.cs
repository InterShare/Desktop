
using System;
using System.Runtime.InteropServices;

namespace DesktopApp.Helpers
{
    // For some reason, the linux GTK version is just too small, while on the other hand the macOS and windows versions have a decent size
    // This small helper function is a hacky solution to that problem
    public static class SizeHelper
    {
        public static int GetSize(int value)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return (int) Math.Round(value * 1.2);
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return (int) Math.Round(value * 0.9);
            }

            return value;
        }
    }
}