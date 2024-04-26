/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using System;

namespace SkillPrestige.Logging
{
    /// <summary>A wrapper for the Stardew Valley logger to simplify the interface and restrict what is logged.</summary>
    public static class Logger
    {
        /*********
        ** Public methods
        *********/
        public static void LogVerbose(string message)
        {
            if (ModEntry.Config.LogLevel >= LogLevel.Verbose)
                // ReSharper disable once RedundantArgumentDefaultValue - Default values can change
                ModEntry.LogMonitor.Log(message, StardewModdingAPI.LogLevel.Trace);
        }

        public static void LogInformation(string message)
        {
            if (ModEntry.Config.LogLevel >= LogLevel.Information)
                ModEntry.LogMonitor.Log(message, StardewModdingAPI.LogLevel.Info);
        }

        public static void LogWarning(string message)
        {
            if (ModEntry.Config.LogLevel >= LogLevel.Warning)
                ModEntry.LogMonitor.Log(message, StardewModdingAPI.LogLevel.Warn);
        }

        public static void LogError(string message)
        {
            if (ModEntry.Config.LogLevel >= LogLevel.Error)
                ModEntry.LogMonitor.Log(message.AddErrorText(), StardewModdingAPI.LogLevel.Error);
        }

        public static void LogCritical(string message)
        {
            if (ModEntry.Config.LogLevel >= LogLevel.Critical)
                ModEntry.LogMonitor.Log(message.AddErrorText(), StardewModdingAPI.LogLevel.Alert);
        }

        public static void LogCriticalWarning(string message)
        {
            if (ModEntry.Config.LogLevel >= LogLevel.Critical)
                ModEntry.LogMonitor.Log(message, StardewModdingAPI.LogLevel.Alert);
        }

        public static void LogDisplay(string message)
        {
            ModEntry.LogMonitor.Log(message);
        }


        /*********
        ** Private methods
        *********/
        private static string AddErrorText(this string message)
        {
            return $"{message}{Environment.NewLine}Please file a bug report on NexusMods.";
        }
    }
}
