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
    // Applies Harmony patches to Farmer.cs to add a conversation topic for divorce.
    public class DivorcePatcher
    {
        private static IMonitor Monitor;
        private static ModConfig Config;

        private enum DivorceState
        {
            UnAssigned,
            UnMarriedNoDivorce,
            NPCDivorce,
            MultiplayerDivorce
        }

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
                Monitor.Log("Adding Harmony pass-through prefix to doDivorce() in Farmer.cs", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(Farmer), nameof(Farmer.doDivorce)),
                    prefix: new HarmonyMethod(typeof(DivorcePatcher), nameof(DivorcePatcher.Farmer_doDivorce_Prefix))
                );
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add prefix divorce event with exception: {ex}", LogLevel.Error);
            }

            try
            {
                Monitor.Log("Adding Harmony postfix to doDivorce() in Farmer.cs", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(Farmer), nameof(Farmer.doDivorce)),
                    postfix: new HarmonyMethod(typeof(DivorcePatcher), nameof(DivorcePatcher.Farmer_doDivorce_Postfix))
                );
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add postfix divorce event with exception: {ex}", LogLevel.Error);
            }
        }

        // Method that is used to prefix
        private static void Farmer_doDivorce_Prefix(Farmer __instance, out DivorceState __state)
        {
            // Create a state variable to check the state in the prefix, used for logic in the postfix
            __state = DivorceState.UnAssigned;
            try
            {
                // Use the same logic as doDivorce() to decide which kind of divorce is happening and log for use in postfix
                if(!__instance.isMarriedOrRoommates())
                {
                    __state = DivorceState.UnMarriedNoDivorce;
                }
                else if(__instance.spouse != null)
                {
                    __state = DivorceState.NPCDivorce;
                }
                else if (__instance.team.GetSpouse(__instance.UniqueMultiplayerID).HasValue)
                {
                    __state = DivorceState.MultiplayerDivorce;
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to log divorce state in prefix with exception: {ex}", LogLevel.Error);
            }
        }

        // Method that is used to postfix
        private static void Farmer_doDivorce_Postfix(Farmer __instance, DivorceState __state)
        {
            switch (__state)
            {
                // If the prefix failed, don't do the postfix
                case DivorceState.UnAssigned:
                    Monitor.Log($"Failed to log divorce state in prefix, skipping divorce conversation topic postfix", LogLevel.Error);
                    break;
                // If the prefix logged that the player is not married, obviously they can't get divorced
                case DivorceState.UnMarriedNoDivorce:
                    Monitor.Log($"Player tried to get divorced when they were not married", LogLevel.Warn);
                    break;
                // If the prefix logged that the player is married to another player, add divorce conversation topics to both players
                case DivorceState.MultiplayerDivorce:
                    
                    // Add divorce conversation topic to current player's player spouse
                    try
                    {
                        // Get spouse
                        long? spouseID = __instance.team.GetSpouse(__instance.UniqueMultiplayerID);
                        Farmer spouse = Game1.getFarmerMaybeOffline(spouseID.Value);

                        // Check if spouse is offline or nonexistent, otherwise add divorce conversation topic to spouse
                        if (spouse is null)
                        {
                            Monitor.Log($"Player was married to multiplayer spouse in prefix but multiplayer spouse not found in postfix", LogLevel.Error);
                        }
                        else
                        {
                            MCTHelperFunctions.AddOrExtendCT(spouse, "divorce", Config.DivorceDuration);
                            if (!Game1.getOnlineFarmers().Contains(spouse))
                            {
                                Monitor.Log($"Added divorce conversation topic to offline multiplayer spouse, unknown behavior may result", LogLevel.Warn);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Monitor.Log($"Failed to add player's spouse divorce conversation topic with exception: {ex}", LogLevel.Error);
                    }

                    // Add divorce conversation topic to current player
                    goto case DivorceState.NPCDivorce;
                // If the prefix logged that the player is married to an NPC, only add divorce conversation topic to player
                case DivorceState.NPCDivorce:
                    try
                    {
                        MCTHelperFunctions.AddOrExtendCT(__instance, "divorce", Config.DivorceDuration);
                    }
                    catch (Exception ex)
                    {
                        Monitor.Log($"Failed to add player's divorce conversation topic with exception: {ex}", LogLevel.Error);
                    }
                    break;
            }

        }
    }

}
