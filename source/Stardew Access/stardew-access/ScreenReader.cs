/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using AccessibleOutput;
using StardewModdingAPI;

namespace stardew_access
{
    internal class ScreenReader
    {
        public static IAccessibleOutput? screenReader = null;
        internal static string prevText = "", prevTextTile = " ", prevChatText = "", prevMenuText = "";

        public static void initializeScreenReader()
        {
            NvdaOutput? nvdaOutput = null;
            JawsOutput? jawsOutput = null;
            SapiOutput? sapiOutput = null;

            // Initialize NVDA
            try
            {
                nvdaOutput = new NvdaOutput();
            }
            catch (Exception) { }

            // Initialize JAWS
            try
            {
                jawsOutput = new JawsOutput();
            }
            catch (Exception) { }

            // Initialize SAPI
            try
            {
                sapiOutput = new SapiOutput();
            }
            catch (Exception){ }

            if (nvdaOutput != null && nvdaOutput.IsAvailable())
                screenReader = nvdaOutput;
            else if (jawsOutput != null && jawsOutput.IsAvailable())
                screenReader = jawsOutput;
            else if (sapiOutput != null && sapiOutput.IsAvailable())
                screenReader = sapiOutput;
            else
                MainClass.monitor.Log($"Unable to load any screen reader!", LogLevel.Error);
        }

        public static void say(string text, bool interrupt)
        {
            if (screenReader == null)
                return;

            screenReader.Speak(text, interrupt);
        }

        public static void sayWithChecker(string text, bool interrupt)
        {
            if (screenReader == null)
                return;

            if (prevText != text)
            {
                prevText = text;
                screenReader.Speak(text, interrupt);
            }
        }

        public static void sayWithMenuChecker(string text, bool interrupt)
        {
            if (screenReader == null)
                return;

            if (prevMenuText != text)
            {
                prevMenuText = text;
                screenReader.Speak(text, interrupt);
            }
        }

        public static void sayWithChatChecker(string text, bool interrupt)
        {
            if (screenReader == null)
                return;

            if (prevChatText != text)
            {
                prevChatText = text;
                screenReader.Speak(text, interrupt);
            }
        }

        public static void sayWithTileQuery(string text, int x, int y, bool interrupt)
        {
            if (screenReader == null)
                return;

            string query = $"{text} x:{x} y:{y}";

            if (prevTextTile != query)
            {
                prevTextTile = query;
                screenReader.Speak(text, interrupt);
            }
        }
    }
}
