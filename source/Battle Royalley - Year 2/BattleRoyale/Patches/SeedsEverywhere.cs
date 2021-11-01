/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace BattleRoyale.Patches
{
    class PlantSeedsAnywhere : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(HoeDirt), "plant");

        public static bool Prefix(HoeDirt __instance, ref bool __result, int index, int tileX, int tileY, Farmer who, GameLocation location)
        {
            if (location.IsFarm)
                return true;

            Crop c = new Crop(index, tileX, tileY);
            __instance.crop = c;
            if ((bool)c.raisedSeeds)
            {
                location.playSound("stoneStep");
            }
            location.playSound("dirtyHit");
            __instance.nearWaterForPaddy.Value = -1;
            if (__instance.hasPaddyCrop() && __instance.paddyWaterCheck(location, new Vector2(tileX, tileY)))
            {
                __instance.state.Value = 1;
                __instance.updateNeighbors(location, new Vector2(tileX, tileY));
            }
            __result = true;
            return false;
        }
    }

    class PlantTreesAnywhere : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Object), "placementAction");

        public static bool Prefix(ref bool __result, Object __instance, GameLocation location, int x, int y)
        {
            if (__instance.isSapling())
            {
                Vector2 placementTile = new Vector2(x / 64, y / 64);
                location.playSound("dirtyHit");
                DelayedAction.playSoundAfterDelay("coin", 100);
                if ((int)__instance.parentSheetIndex == 251)
                {
                    location.terrainFeatures.Add(placementTile, new Bush(placementTile, 3, location));
                    __result = true;
                    return false;
                }
                bool actAsGreenhouse = location.IsGreenhouse || (((int)__instance.parentSheetIndex == 69 || (int)__instance.parentSheetIndex == 835) && location is IslandWest);
                location.terrainFeatures.Add(placementTile, new FruitTree(__instance.parentSheetIndex)
                {
                    GreenHouseTree = actAsGreenhouse,
                    GreenHouseTileTree = location.doesTileHavePropertyNoNull((int)placementTile.X, (int)placementTile.Y, "Type", "Back").Equals("Stone")
                });
                __result = true;
                return false; ;
            }
            return true;
        }
    }
}
