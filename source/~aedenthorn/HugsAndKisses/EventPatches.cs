/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;

namespace HugsAndKisses
{
    public static class EventPatches
    {
        private static IMonitor Monitor;
        private static IModHelper Helper;

        // call this method from your Entry class
        public static void Initialize(IMonitor monitor, ModConfig config, IModHelper helper)
        {
            Monitor = monitor;
            Helper = helper;
        }
        public static bool Event_command_playSound_Prefix(Event __instance, string[] split)
        {
            try
            {
                if (split[1] == "dwop" && __instance.isWedding && ModEntry.SConfig.CustomKissSound.Length > 0 && Kissing.kissEffect != null)
                {
                    Kissing.kissEffect.Play();
                    int num = __instance.CurrentCommand;
                    __instance.CurrentCommand = num + 1;
                    return false;
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(Event_command_playSound_Prefix)}:\n{ex}", LogLevel.Error);
            }
            return true;
        }
    }
}