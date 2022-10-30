/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NeroYuki/StardewSurvivalProject
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewValley;

namespace StardewSurvivalProject.source.harmony_patches
{
    class NPCPatches
    {
        private static IMonitor Monitor = null;
        // call this method from your Entry class
        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        public static void GiftGiving_PostFix(NPC __instance, StardewValley.Object o, Farmer giver, bool updateGiftLimitInfo = true, float friendshipChangeMultiplier = 1f, bool showResponse = true)
        {
            try
            {
                if (o == null || giver == null)
                    return;

                events.CustomEvents.InvokeOnGiftGiven(__instance, o, giver);
                return;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(GiftGiving_PostFix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }
    }
}
