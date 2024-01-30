/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

/*
    Linux speech dispatcher library used:
    https://github.com/shoaib11120/libspeechdwrapper
*/

using System.Reflection;
using System.Runtime.InteropServices;

namespace stardew_access.ScreenReader
{
    public struct GoString(string msg, long len)
    {
        public string msg = msg;
        public long len = len;
    }

    public class ScreenReaderLinux : ScreenReaderAbstract
    {
        [DllImport("libspeechdwrapper")]
        private static extern int Initialize();

        [DllImport("libspeechdwrapper")]
        private static extern int Speak(GoString text, bool interrupt);

        [DllImport("libspeechdwrapper")]
        private static extern int Close();

        private bool initialized = false, resolvedDll = false;

        public override void InitializeScreenReader()
        {
            Log.Info("Initializing speech dispatcher...");
            if (!resolvedDll)
            {
                NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), DllImportResolver);
                resolvedDll = true;
            }
            int res = Initialize();
            if (res == 1)
            {
                initialized = true;
                Log.Info("Successfully initialized.");
            }
            else
            {
                Log.Error("Unable to initialize.");
            }
        }

        public override void CloseScreenReader()
        {
            if (initialized)
            {
                _ = Close();
                initialized = false;
            }
        }

        public override bool Say(string text, bool interrupt)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            if (!initialized) return false;
            if (!MainClass.Config.TTS) return false;

            if (text.Contains('^')) text = text.Replace('^', '\n');

            GoString str = new(text, text.Length);
            int re = Speak(str, interrupt);

            if (re != 1)
            {
                Log.Error($"Failed to output text: {text}");
                return false;
            }
            else
            {
                #if DEBUG
                Log.Verbose($"Speaking(interrupt: {interrupt}) = {text}");
                #endif
                return true;
            }
        }

        private static IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            // libraryName is the name provided in DllImport i.e., [DllImport(libraryName)]
            if (libraryName != "libspeechdwrapper") return IntPtr.Zero;
            if (MainClass.ModHelper is null) return IntPtr.Zero;

            string libraryPath = Path.Combine(MainClass.ModHelper.DirectoryPath, "libraries", "linux", "libspeechdwrapper.so");
            return NativeLibrary.Load(libraryPath, assembly, searchPath);
        }
    }
}
