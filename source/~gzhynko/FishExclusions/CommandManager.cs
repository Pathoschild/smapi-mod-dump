/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/StardewMods
**
*************************************************/

using StardewModdingAPI;

namespace FishExclusions
{
    public static class CommandManager
    {
        public static void RegisterCommands(IModHelper helper)
        {
            helper.ConsoleCommands.Add("fex_toggle", "Toggle exclusions.", Toggle);
        }

        private static void Toggle(string command, string[] args)
        {
            ModEntry.ExclusionsEnabled = !ModEntry.ExclusionsEnabled;
            ModEntry.ModMonitor.Log($"Exclusions { (ModEntry.ExclusionsEnabled ? "enabled" : "disabled") }.", LogLevel.Info);
        }
    }
}
