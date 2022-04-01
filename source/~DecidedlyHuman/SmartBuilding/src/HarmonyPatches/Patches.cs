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

namespace SmartBuilding.HarmonyPatches
{
    public static class Patches
    {
        private static bool _currentlyInBuildMode;
        private static bool _currentlyDrawing;
        private static bool _currentlyErasing;
        private static bool _currentlyPlacing;

        public static bool CurrentlyInBuildMode
        {
            get { return _currentlyInBuildMode; }
            set { _currentlyInBuildMode = value; }
        }

        public static bool CurrentlyDrawing
        {
            get { return _currentlyDrawing; }
            set { _currentlyDrawing = value; }
        }

        public static bool CurrentlyErasing
        {
            get { return _currentlyErasing; }
            set { _currentlyErasing = value; }
        }

        public static bool CurrentlyPlacing
        {
            get { return _currentlyPlacing; }
            set { _currentlyPlacing = value; }
        }

        private static bool ShouldCancel()
        {
            if (_currentlyInBuildMode)
            {
                if (_currentlyErasing || _currentlyDrawing || _currentlyPlacing)
                    return false;
                else
                    return true;

                // if (_currentlyDrawing) return false;
                // if (_currentlyErasing) return false;
                // if (_currentlyPlacing) return false;
                //
                // if (!_currentlyDrawing) return true;
                // if (!_currentlyErasing) return true;
                // if (!_currentlyPlacing) return true;
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