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
    // Applies Harmony patches to Utility.cs to add a conversation topic for weddings.
    public class WeddingPatcher
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
                Monitor.Log("Adding Harmony postfix to getWeddingEvent() in Utility.cs", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(Utility), nameof(Utility.getWeddingEvent)),
                    postfix: new HarmonyMethod(typeof(WeddingPatcher), nameof(WeddingPatcher.Utility_getWeddingEvent_Postfix))
                );
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add postfix wedding event with exception: {ex}", LogLevel.Error);
            }

            try
            {
                Monitor.Log("Adding Harmony postfix to getPlayerWeddingEvent() in Utility.cs", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(Utility), nameof(Utility.getPlayerWeddingEvent)),
                    postfix: new HarmonyMethod(typeof(WeddingPatcher), nameof(WeddingPatcher.Utility_getPlayerWeddingEvent_Postfix))
                );
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add postfix player wedding event with exception: {ex}", LogLevel.Error);
            }

            try
            {
                Monitor.Log("Adding Harmony postfix to engagementResponse() in NPC.cs", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(NPC), "engagementResponse"),
                    postfix: new HarmonyMethod(typeof(WeddingPatcher), nameof(NPC_engagementResponse_Postfix))
                );
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add postfix player to NPC engagement with exception: {ex}", LogLevel.Error);
            }

            try
            {
                Monitor.Log("Adding Harmony postfix to handleIncomingProposal() in FarmerTeam.cs", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(FarmerTeam), "handleIncomingProposal"),
                    postfix: new HarmonyMethod(typeof(WeddingPatcher), nameof(FarmerTeam_handleIncomingProposal_Postfix))
                );
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add postfix multiplayer engagement with exception: {ex}", LogLevel.Error);
            }
        }

        // Methods that are used to postfix
        private static void Utility_getWeddingEvent_Postfix(Farmer farmer)
        {
            try
            {
                MCTHelperFunctions.AddOrExtendCT(farmer, "wedding", Config.WeddingDuration);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add wedding conversation topic with exception: {ex}", LogLevel.Error);
            }
        }

        private static void Utility_getPlayerWeddingEvent_Postfix(Farmer farmer, Farmer spouse)
        {
            try
            {
                MCTHelperFunctions.AddOrExtendCT(farmer, "wedding", Config.WeddingDuration);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add player's wedding conversation topic with exception: {ex}", LogLevel.Error);
            }

            try
            {
                MCTHelperFunctions.AddOrExtendCT(spouse, "wedding", Config.WeddingDuration);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add player's spouse wedding conversation topic with exception: {ex}", LogLevel.Error);
            }
        }

        private static void NPC_engagementResponse_Postfix(NPC __instance, Farmer who, bool asRoommate)
        {
            if (asRoommate)
            {
                try
                {
                    MCTHelperFunctions.AddOrExtendCT(who, "newRoommate", Config.EngagementDuration);
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Failed to add player's wedding conversation topic with exception: {ex}", LogLevel.Error);
                }
            }
            else
            {
                try
                {
                    MCTHelperFunctions.AddOrExtendCT(who, "engaged", Config.EngagementDuration);
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Failed to add player's wedding conversation topic with exception: {ex}", LogLevel.Error);
                }
            }
        }

        private static void FarmerTeam_handleIncomingProposal_Postfix(Proposal proposal)
        {
            try
            {
                // Only handle accepted proposals of marriage
                if (proposal.response.Value == ProposalResponse.Accepted && proposal.proposalType.Value == ProposalType.Marriage)
                {
                    MCTHelperFunctions.AddOrExtendCT(Game1.player, "engaged", Config.EngagementDuration);

                    Farmer spouse = Game1.getFarmerMaybeOffline(proposal.sender.Value.UniqueMultiplayerID);
                    MCTHelperFunctions.AddOrExtendCT(spouse, "engaged", Config.EngagementDuration);
                    if (!Game1.getOnlineFarmers().Contains(spouse))
                    {
                        Monitor.Log($"Added engaged conversation topic to offline multiplayer spouse, unknown behavior may result", LogLevel.Warn);
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add multiplayer engagement conversation topics with exception: {ex}", LogLevel.Error);
            }
        }
    }

}
