using System;
using System.Diagnostics;
using LogApi = StardewModdingAPI.Log;

namespace Igorious.StardewValley.DynamicAPI.Utils
{
    public static class Log
    {
        public static void Error(Exception e) => Error(e.ToString());

        public static void Error(string message)
        {
            LogApi.SyncColour($"[DAPI] {message}", ConsoleColor.Red);
        }

        public static void Fail(string message)
        {
            LogApi.SyncColour($"[DAPI] {message}", ConsoleColor.DarkRed);
        }

        public static void Out(string message)
        {
            LogApi.SyncColour($"[DAPI] {message}", ConsoleColor.DarkGray);
        }

        [Conditional("DEBUG")]
        public static void Info(string message)
        {
            LogApi.SyncColour($"[DAPI] {message}", ConsoleColor.DarkGray);
        }

        [Conditional("DEBUG")]
        public static void InfoAsync(string message)
        {
            LogApi.AsyncColour($"[DAPI] {message}", ConsoleColor.DarkGray);
        }

        [Conditional("DEBUG")]
        public static void ImportantInfo(string message)
        {
            LogApi.SyncColour($"[DAPI] {message}", ConsoleColor.DarkYellow);
        }
    }
}
