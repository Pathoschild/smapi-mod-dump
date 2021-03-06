/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace CropTransplantMod
{
    public class TransplantController
    {
        public static bool IsGardenPot(Object o)
        {
            return o is Object object1 && object1.ParentSheetIndex == 62 && object1.bigCraftable.Value;
        }

        public static bool IsTapper(Object obj)
        {
            return obj.ParentSheetIndex == 105 && obj.bigCraftable.Value;
        }

        public static bool IsNextToOtherTrees(GameLocation location, int x,  int y)
        {
            Vector2 key = new Vector2();
            for (int index2 = x / 64 - 2; index2 <= x / 64 + 2; ++index2)
            {
                for (int index3 = y / 64 - 2; index3 <= y / 64 + 2; ++index3)
                {
                    key.X = (float)index2;
                    key.Y = (float)index3;
                    if (location.terrainFeatures.ContainsKey(key) && (location.terrainFeatures[key] is Tree || location.terrainFeatures[key] is FruitTree))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal static Bush PrepareBushForPlacement(Bush bush, Vector2 tileLocation)
        {
            if (!Game1.player.currentLocation.IsOutdoors && !bush.greenhouseBush.Value)
            {
                bush.greenhouseBush.Value = true;
            }
            else if (Game1.player.currentLocation.IsOutdoors && bush.greenhouseBush.Value)
            {
                bush.greenhouseBush.Value = false;
            }
            bush.tilePosition.Value = tileLocation;
            bush.currentTileLocation = tileLocation;
            bush.currentLocation = Game1.currentLocation;
            ShakeBush(bush, tileLocation);
            bush.loadSprite();
            bush.tileSheetOffset.Value = 0;
            bush.setUpSourceRect();
            return bush;
        }

        public static bool CanTransplantTerrainFeature(TerrainFeature terrainFeature)
        {
            return IsValidCrop(terrainFeature) || IsValidTree(terrainFeature) || IsValidBush(terrainFeature);
        }

        public static bool IsValidCrop(TerrainFeature terrainFeature)
        {
            return terrainFeature is HoeDirt hoeDirt && hoeDirt.crop != null && !hoeDirt.crop.dead.Value && (!hoeDirt.crop.forageCrop.Value || hoeDirt.crop.whichForageCrop.Value > 2);
        }

        public static bool IsValidTree(TerrainFeature terrainFeature)
        {
            bool result = false;
            if (terrainFeature is Tree tree)
            {
                result = Math.Min(5, tree.growthStage.Value) <= DataLoader.ModConfig.TreeTransplantMaxStage - (DataLoader.ModConfig.TreeTransplantMaxStage < 4 ? 1 : 0);
            }
            else if (terrainFeature is FruitTree fruitTree)
            {
                result = Math.Min(4, fruitTree.growthStage.Value) <= DataLoader.ModConfig.FruitTreeTransplantMaxStage - 1;
            }
            return result;
        }

        public static bool IsValidBush(TerrainFeature terrainFeature)
        {
            bool result = false;
            if (terrainFeature is Bush bush)
            {
                result = bush.size.Value == Bush.greenTeaBush;
            }
            return result;
        }

        public static bool IsTreeNoSpawnTile(GameLocation location, int x, int y)
        {
            string str = location.doesTileHaveProperty(x, y, "NoSpawn", "Back");
            return str != null && (str.Equals("Tree") || str.Equals("All") || str.Equals("True"));
        }

        public static bool IsValidTileForFruitTree(GameLocation location, int saplingIndex, int x, int y)
        {
            return DataLoader.ModConfig.EnablePlacementOfFruitTreesOnAnyTileType
                   || (
                       (location is Farm || DataLoader.ModConfig.EnablePlacementOfFruitTreesOutOfTheFarm)
                       && (
                           location.doesTileHaveProperty(x, y, "Diggable", "Back") != null
                           || location.doesTileHavePropertyNoNull(x, y, "Type", "Back").Equals("Grass")
                           || location.doesTileHavePropertyNoNull(x, y, "Type", "Back").Equals("Dirt"))
                       && !location.doesTileHavePropertyNoNull(x, y, "NoSpawn", "Back").Equals("Tree"))
                   || (
                       location.CanPlantTreesHere(saplingIndex, x, y)
                       && (
                           location.doesTileHaveProperty(x, y, "Diggable", "Back") != null 
                           || location.doesTileHavePropertyNoNull(x, y, "Type", "Back").Equals("Stone")));
        }

        public static bool IsValidTileForBush(GameLocation location, int tileLocationX, int tileLocationY)
        {
            return DataLoader.ModConfig.EnableToPlantTeaBushesOnAnyTileType
                    || (
                        (location.doesTileHaveProperty(tileLocationX, tileLocationY, "Diggable", "Back") != null 
                            || location.doesTileHavePropertyNoNull(tileLocationX, tileLocationY, "Type", "Back").Equals("Grass") 
                            || location.doesTileHavePropertyNoNull(tileLocationX, tileLocationY, "Type", "Back").Equals("Dirt"))
                        && !location.doesTileHavePropertyNoNull(tileLocationX, tileLocationY, "NoSpawn", "Back").Equals("Tree"));
        }

        internal static void ShakeCrop(HoeDirt hoeDirt, Vector2? tileLocation = null)
        {
            var maxShake = hoeDirt.getMaxShake();
            if (hoeDirt.crop != null && hoeDirt.crop.currentPhase.Value != 0 && (double) maxShake == 0.0)
            {
                Grass.grassSound = Game1.soundBank.GetCue("grassyStep");
                Grass.grassSound.Play();

                var farmer = Game1.player;
                int speedOfCollision = 2;
                
                hoeDirt.shake(
                    (float)(0.392699092626572 / (double)((5 + farmer.addedSpeed) / speedOfCollision) -(speedOfCollision > 2 ? (double)hoeDirt.crop.currentPhase.Value * 3.14159274101257 / 64.0: 0.0))
                    , (float)Math.PI / 80f / (float)((5 + farmer.addedSpeed) / speedOfCollision)
                    ,tileLocation.HasValue ? (double)farmer.lastPosition.X > (double)tileLocation.Value.X * 64.0 + 32.0 : farmer.FacingDirection == 1 ? true : farmer.FacingDirection == 3 ? false : TransplantOverrides.ShakeFlag = !TransplantOverrides.ShakeFlag);
            }
        }

        internal static void ShakeTree(Tree tree, Vector2? tileLocation = null)
        {
            tree.performUseAction(tileLocation ?? Game1.player.getTileLocation() + new Vector2(0, -2), Game1.player.currentLocation);
        }

        internal static void ShakeTree(FruitTree tree, Vector2? tileLocation = null)
        {
            tree.performUseAction(tileLocation ?? Game1.player.getTileLocation() + new Vector2(0, -2), Game1.player.currentLocation);
        }

        internal static void ShakeBush(Bush bush, Vector2? tileLocation = null)
        {
            bush.performUseAction(tileLocation ?? Game1.player.getTileLocation() + new Vector2(0, -2), Game1.player.currentLocation);
        }

        public static bool CheckFruitTreeGrowth(GameLocation location, int x, int y)
        {
            if (!DataLoader.ModConfig.EnablePlacementOfFruitTreesBlockedGrowth)
            {
                Vector2 v = default(Vector2);
                for (int j = x - 1; j <= x + 1; j++)
                {
                    for (int l = y - 1; l <= y + 1; l++)
                    {
                        v.X = j;
                        v.Y = l;
                        if (location.terrainFeatures.ContainsKey(v) && location.terrainFeatures[v] is FruitTree fruitTree &&
                            fruitTree.growthStage.Value < FruitTree.treeStage)
                        {
                            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13060_Fruit"));
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public static bool IntersectWithFarmer(GameLocation location, int x, int y)
        {
            return location.farmers.Any(fs => fs.GetBoundingBox().Intersects(new Microsoft.Xna.Framework.Rectangle(x, y, 64, 64)));
        }
    }
}
