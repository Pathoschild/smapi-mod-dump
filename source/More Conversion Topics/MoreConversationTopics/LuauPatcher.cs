/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/elizabethcd/MoreConversationTopics
**
*************************************************/

using System;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace MoreConversationTopics
{
    // Applies Harmony patches to Event.cs to add a conversation topic for luau results.
    public class LuauPatcher
    {
        private static IMonitor Monitor;
        private static ModConfig Config;

        // call this method from your Entry class
        public static void Initialize(IMonitor monitor, ModConfig config)
        {
            Monitor = monitor;
            Config = config;
        }

        // Method to apply harmony patch
        public static void Apply(Harmony harmony)
        {
            try
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(Event),"governorTaste"),
                    postfix: new HarmonyMethod(typeof(LuauPatcher), nameof(LuauPatcher.Event_governorTaste_Postfix))
                );
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add postfix luau taste with exception: {ex}", LogLevel.Error);
            }
        }

        // Method that is used to postfix
        private static void Event_governorTaste_Postfix(Event __instance)
        {
            try
            {
                string governorReactionString = __instance.eventCommands[__instance.CurrentCommand + 1];
                if (governorReactionString.EndsWith("6"))
                {
                    Game1.player.activeDialogueEvents.Add("luauShorts", Config.LuauDuration);
                }
                else if (governorReactionString.EndsWith("4"))
                {
                    Game1.player.activeDialogueEvents.Add("luauBest", Config.LuauDuration);
                }
                else if (governorReactionString.EndsWith("0"))
                {
                    Game1.player.activeDialogueEvents.Add("luauPoisoned", Config.LuauDuration);
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add luau conversation topic with exception: {ex}", LogLevel.Error);
            }
        }
    }

}
