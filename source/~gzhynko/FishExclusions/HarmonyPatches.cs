/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/StardewMods
**
*************************************************/

using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using Object = StardewValley.Object;
// ReSharper disable InconsistentNaming

namespace FishExclusions
{
    public class HarmonyPatches
    {
        /// <summary> Patch for the GameLocation.getFish method. </summary>
        public static void GetFish(GameLocation __instance, float millisecondsAfterNibble, string bait, int waterDepth, Farmer who,
            double baitPotency, Vector2 bobberTile, ref Item __result, string locationName = null)
        {
            if (!ModEntry.ExclusionsEnabled) return;
            
#if DEBUG
            ModEntry.ModMonitor.Log(
                $"Get Fish: locationName: {__instance.Name}, season: {Game1.currentSeason}, raining: {Game1.IsRainingHere(__instance)}",
                LogLevel.Debug);
#endif
            
            var bannedIds = Utils.GetExcludedFish(ModEntry.Config, Game1.currentSeason, __instance.Name, Game1.IsRainingHere(__instance));

            // This method has a neat unused (yet?) parameter 'baitPotency'. Why not to use it to avoid recursion?
            if ((int) baitPotency == 909 || !bannedIds.Contains(__result.ParentSheetIndex)) return;
            
            var numberOfAttempts = 0;
            
            // Retry x times before giving up.
            var maxAttempts = ModEntry.Config.TimesToRetry;

            var lastResult = __result;

            while (numberOfAttempts < maxAttempts && bannedIds.Contains(lastResult.ParentSheetIndex))
            {
                lastResult = __instance.getFish(millisecondsAfterNibble, bait, waterDepth, who, 909, bobberTile,
                    locationName);
                
                numberOfAttempts++;
            }

            var itemToCatchIfNoVariantsLeft = ModEntry.Config.ItemToCatchIfAllFishIsExcluded == 0
                ? 168
                : ModEntry.Config.ItemToCatchIfAllFishIsExcluded;
            
            // Return Trash or the item specified in config in case all possible
            // fish for this water body / season / weather is excluded.
            if(bannedIds.Contains(lastResult.ParentSheetIndex)) lastResult = ItemRegistry.Create(itemToCatchIfNoVariantsLeft.ToString());

            __result = lastResult;
        }
    }
}
