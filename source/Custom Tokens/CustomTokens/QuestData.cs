/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/CustomTokens
**
*************************************************/

using System.Collections.Generic;
using StardewValley.Quests;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using Harmony;
using System;

namespace CustomTokens
{
    public class QuestData
    {
        private static IMonitor monitor;

        public static void Hook(HarmonyInstance harmony, IMonitor monitor)
        {
            QuestData.monitor = monitor;
            monitor.Log("Initialising harmony patches...");

            harmony.Patch(
                original: AccessTools.Method(typeof(Quest), nameof(Quest.questComplete)),
                postfix: new HarmonyMethod(typeof(QuestData), nameof(QuestData.questComplete_Postfix))
            );

        }

        public static void questComplete_Postfix(Quest __instance)
        {
            try
            {
                if (ModEntry.perScreen.Value.QuestsCompleted.Contains(__instance.id.Value) == false && __instance.id.Value != 0)
                {
                    ModEntry.perScreen.Value.QuestsCompleted.Add(__instance.id.Value);
                    monitor.Log($"Quest with id {__instance.id.Value} has been completed");
                }
            }
            catch (Exception ex)
            {
                monitor.Log($"Failed in {nameof(questComplete_Postfix)}, quest tokens may not work:\n{ex}", LogLevel.Error);
            }                      
        }

        internal void CheckForCompletedSpecialOrders(PerScreen<PlayerData> data, IMonitor monitor)
        {
            var order = Game1.player.team.completedSpecialOrders;

            // Check for completed special orders
            if (data.Value.SpecialOrdersCompleted.Count < order.Count())
            {
                foreach (string questkey in new List<string>(order.Keys))
                {
                    if (data.Value.SpecialOrdersCompleted.Contains(questkey) == false)
                    {
                        data.Value.SpecialOrdersCompleted.Add(questkey);
                        monitor.Log($"Special Order with key {questkey} has been completed");
                    }
                }
            }
        }
    }

    
}