/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Objects;

// ReSharper disable InconsistentNaming

namespace SmartBuilding.HarmonyPatches
{
    public static class Patches
    {
        public static bool CurrentlyInBuildMode { get; set; }

        public static bool AllowPlacement { get; set; }

        private static bool ShouldPerformAction()
        {
            if (CurrentlyInBuildMode) return AllowPlacement;

            // If we're not in build mode, we always want to continue on to the regular methods.
            return true;
        }

        public static bool PlacementAction_Prefix(SObject __instance, GameLocation location, int x, int y, Farmer who)
        {
            return ShouldPerformAction();
        }

        public static bool Chest_CheckForAction_Prefix(Chest __instance, Farmer who, bool justCheckingForActivity)
        {
            return ShouldPerformAction();
        }

        public static bool FishPond_DoAction_Prefix(FishPond __instance, Vector2 tileLocation, Farmer who)
        {
            return ShouldPerformAction();
        }

        public static bool StorageFurniture_DoAction_Prefix(StorageFurniture __instance, Farmer who,
            bool justCheckingForActivity)
        {
            return ShouldPerformAction();
        }
    }
}
