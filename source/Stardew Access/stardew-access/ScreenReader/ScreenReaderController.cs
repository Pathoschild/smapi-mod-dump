/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using System.Runtime.InteropServices;

namespace stardew_access.ScreenReader
{
    public class ScreenReaderController
    {
        public static IScreenReader Initialize()
        {
            IScreenReader ScreenReader = new ScreenReaderWindows(); // Default is windows

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ScreenReaderWindows screenReaderWindows = new();
                screenReaderWindows.InitializeScreenReader();

                ScreenReader = screenReaderWindows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                ScreenReaderLinux screenReaderLinux = new();
                screenReaderLinux.InitializeScreenReader();

                ScreenReader = screenReaderLinux;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                ScreenReaderMac screenReaderMac = new();
                screenReaderMac.InitializeScreenReader();

                ScreenReader = screenReaderMac;
            }
            else
            {
                ScreenReader.InitializeScreenReader();
            }

            return ScreenReader;
        }
    }
}