/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pepoluan/StackSplitRedux
**
*************************************************/

using StardewModdingAPI;
using System.Linq;

namespace StackSplitRedux
    {
    /// <summary>
    /// Convenience class so we don't have to keep passing Mod.Instance.Monitor everywhere
    /// </summary>
    public static class Log {
        public static void Alert(string msg) => Mod.Instance.Monitor.Log(msg, LogLevel.Alert);
        public static void Error(string msg) => Mod.Instance.Monitor.Log(msg, LogLevel.Error);
        public static void Warn(string msg) => Mod.Instance.Monitor.Log(msg, LogLevel.Warn);
        public static void Info(string msg) => Mod.Instance.Monitor.Log(msg, LogLevel.Info);
        public static void Debug(string msg) => Mod.Instance.Monitor.Log(msg, LogLevel.Debug);
        public static void Trace(string msg) => Mod.Instance.Monitor.Log(msg, LogLevel.Trace);
        public static void TraceIfD(string msg) {
#if DEBUG
            bool debugging = true;
#else
            bool debugging = Mod.Config.DebuggingMode;
#endif
            if (debugging) Trace(msg);
            }
        }
    public static class Seq {
        public static int Min(params int[] args) => args.Min();
        }
    }
