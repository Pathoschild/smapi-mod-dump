/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using System;
using HarmonyLib;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace Circuit.Patches
{
    [HarmonyPatch(typeof(Train), nameof(Train.Update))]
    internal class TrainUpdatePatch
    {
        public static void Postfix(Train __instance, GameLocation location)
        {
            if (!ModEntry.ShouldPatch())
                return;

            if (!Game1.eventUp && location.Equals(Game1.currentLocation))
            {
                if (Game1.player.GetBoundingBox().Intersects(__instance.getBoundingBox()))
                    ModEntry.Instance.TaskManager?.OnHitByTrain();
            }
        }
    }
}
