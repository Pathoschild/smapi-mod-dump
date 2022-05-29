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
using StardewValley.Events;

namespace MoreConversationTopics
{
    // Applies Harmony patches to BirthingEvent.cs and PlayerCoupleBirthingEvent.cs to add a conversation topic for when a child is born, by gender of the child.
    public class BirthPatcher
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
                Monitor.Log("Adding Harmony postfix to setUp() in BirthingEvent.cs",LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(BirthingEvent),nameof(BirthingEvent.setUp)),
                    postfix: new HarmonyMethod(typeof(BirthPatcher), nameof(BirthPatcher.BirthingEvent_setUp_Postfix))
                );
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add postfix for player-NPC childbirth with exception: {ex}", LogLevel.Error);
            }

            try
            {
                Monitor.Log("Adding Harmony postfix to setUp() in PlayerCoupleBirthingEvent.cs", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(PlayerCoupleBirthingEvent), nameof(PlayerCoupleBirthingEvent.setUp)),
                    postfix: new HarmonyMethod(typeof(BirthPatcher), nameof(BirthPatcher.PlayerCoupleBirthingEvent_setUp_Postfix))
                );
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add postfix for player-player childbirth with exception: {ex}", LogLevel.Error);
            }
        }

        // Method that is used to postfix
        private static void BirthingEvent_setUp_Postfix(bool ___isMale)
        {
            // If a player married to an NPC has a child, add conversation topics depending on gender
            try
            {
                if (___isMale)
                {
                    MCTHelperFunctions.AddOrExtendCT("babyBoy", Config.BirthDuration);
                }
                else
                {
                    MCTHelperFunctions.AddOrExtendCT("babyGirl", Config.BirthDuration);
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add birth conversation topic with exception: {ex}", LogLevel.Error);
            }
        }

        private static void PlayerCoupleBirthingEvent_setUp_Postfix(bool __result, Farmer ___spouse, bool ___isMale)
        {
            // If two players are married and have a child, add the conversation topic for having a new baby to both players
            try
            {
                if (!__result)
                {
                    if (___isMale)
                    {
                        MCTHelperFunctions.AddOrExtendCT("babyBoy", Config.BirthDuration);
                    }
                    else
                    {
                        MCTHelperFunctions.AddOrExtendCT("babyGirl", Config.BirthDuration);
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add player's childbirth conversation topic with exception: {ex}", LogLevel.Error);
            }

            try
            {
                if (!__result)
                {
                    if (___isMale)
                    {
                        MCTHelperFunctions.AddOrExtendCT(___spouse, "babyBoy", Config.BirthDuration);
                    }
                    else
                    {
                        MCTHelperFunctions.AddOrExtendCT(___spouse, "babyGirl", Config.BirthDuration);
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add spouse's childbirth conversation topic with exception: {ex}", LogLevel.Error);
            }
        }
    }

}
