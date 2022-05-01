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
        private static bool currentlyDrawing;
        private static bool currentlyErasing;
        private static bool currentlyPlacing;

        public static bool CurrentlyInBuildMode
        {
            get { return currentlyInBuildMode; }
            set { currentlyInBuildMode = value; }
        }

        public static bool CurrentlyDrawing
        {
            get { return currentlyDrawing; }
            set { currentlyDrawing = value; }
        }

        public static bool CurrentlyErasing
        {
            get { return currentlyErasing; }
            set { currentlyErasing = value; }
        }

        public static bool CurrentlyPlacing
        {
            get { return currentlyPlacing; }
            set { currentlyPlacing = value; }
        }

        private static bool ShouldCancel()
        {
            if (currentlyInBuildMode)
            {
                // Yes, this could be easily simplified, but I prefer the readability.
                if (currentlyErasing || currentlyDrawing || currentlyPlacing)
                    return false;
                else
                    return true;
            }

            return true;
        }

        public static bool PlacementAction_Prefix(Object __instance, GameLocation location, int x, int y, Farmer who)
        {
            return ShouldCancel();
        }

        public static bool Chest_CheckForAction_Prefix(Chest __instance, Farmer who, bool justCheckingForActivity)
        {
            return ShouldCancel();
        }

        public static bool FishPond_DoAction_Prefix(FishPond __instance, Vector2 tileLocation, Farmer who)
        {
            return ShouldCancel();
        }

        public static bool StorageFurniture_DoAction_Prefix(StorageFurniture __instance, Farmer who, bool justCheckingForActivity)
        {
            return ShouldCancel();
        }
    }
}