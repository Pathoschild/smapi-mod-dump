/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ichortower/HatMouseLacey
**
*************************************************/

using StardewModdingAPI;

namespace ichortower_HatMouseLacey
{
    /*
     * Holds constants and static references that I need to use in multiple
     * source files around the namespace.
     *
     * The Monitor, ModHelper, and Manifest aren't available until mod entry,
     * so it's ModEntry's job to set them.
     */
    internal sealed class HML
    {
        public static string CPId = "ichortower.HatMouseLacey";
        public static string CoreId = "ichortower.HatMouseLacey.Core";
        public static string LaceyInternalName = $"{CPId}_Lacey";
        public static string MailPrefix = $"{CPId}_Mail_";
        public static string EventPrefix = $"{CPId}_Event_";
        public static string QuestPrefix = $"{CPId}_Quest_";
        public static string CTPrefix = $"{CPId}_CT_";
        public static string MusicPrefix = $"{CPId}_Music_";
        public static string TriggerActionPrefix = $"{CPId}_TriggerAction_";
        public static string CommandWord = "hatmouselacey";

        public static IMonitor Monitor = null!;
        public static IModHelper ModHelper = null!;
        public static IManifest Manifest = null!;
    }

    /*
     * Shorthand for the monitor's log functions.
     */
    internal sealed class Log
    {
        public static void Trace(string text) {
            HML.Monitor.Log(text, LogLevel.Trace);
        }
        public static void Debug(string text) {
            HML.Monitor.Log(text, LogLevel.Debug);
        }
        public static void Info(string text) {
            HML.Monitor.Log(text, LogLevel.Info);
        }
        public static void Warn(string text) {
            HML.Monitor.Log(text, LogLevel.Warn);
        }
        public static void Error(string text) {
            HML.Monitor.Log(text, LogLevel.Error);
        }
        public static void Alert(string text) {
            HML.Monitor.Log(text, LogLevel.Alert);
        }
        public static void Verbose(string text) {
            HML.Monitor.VerboseLog(text);
        }
    }
}
