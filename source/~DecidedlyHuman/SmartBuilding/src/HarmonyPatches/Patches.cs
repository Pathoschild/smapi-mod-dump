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
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Objects;
using xTile.Dimensions;
// ReSharper disable InconsistentNaming

namespace SmartBuilding.HarmonyPatches
{
    public static class Patches
    {
        private static bool currentlyInBuildMode;
        private static bool allowPlacement;

        public static bool CurrentlyInBuildMode
        {
            get { return currentlyInBuildMode; }
            set { currentlyInBuildMode = value; }
        }

        public static bool AllowPlacement
        {
            get { return allowPlacement; }
            set { allowPlacement = value; }
        }

        private static bool ShouldPerformAction()
        {
            if (currentlyInBuildMode)
            {
                return allowPlacement;
            }
            
            // If we're not in build mode, we always want to continue on to the regular methods.
            return true;
        }

        public static bool PlacementAction_Prefix(Object __instance, GameLocation location, int x, int y, Farmer who)
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

        public static bool StorageFurniture_DoAction_Prefix(StorageFurniture __instance, Farmer who, bool justCheckingForActivity)
        {
            return ShouldPerformAction();
        }
    }
}