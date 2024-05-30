/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chiccenFL/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley.ItemTypeDefinitions;

namespace FruitTreeTweaks
{
    public partial class ModEntry
    {
        private static Dictionary<GameLocation, Dictionary<Vector2, (List<Color> colors, List<float> sizes, List<Vector2> offsets)>> fruitData = new();
        private static Dictionary<string, (Texture2D texture, Rectangle sourceRect)> textures = new();
        private static int fruitToday;

        private static float GetTreeBottomOffset(FruitTree tree)
        {
            if (!Config.EnableMod)
                return 1E-07f;
            return 1E-07f + Game1.getFarm().terrainFeatures.Pairs.FirstOrDefault(pair => pair.Value == tree).Key.X / 100000f;
        }
        private static bool CanPlantAnywhere()
        {
            return Config.PlantAnywhere;
        }
        private static int GetMaxFruit()
        {
            return !Config.EnableMod ? 3 : Math.Max(1, Config.MaxFruitPerTree);
        }
        private static int GetFruitPerDay()
        {
            return !Config.EnableMod ? 1 : Game1.random.Next(Config.MinFruitPerDay, Math.Max(Config.MinFruitPerDay, Config.MaxFruitPerDay + 1));
        }
        private static int ChangeDaysToMatureCheck(int oldValue)
        {
            if (!Config.EnableMod)
                return oldValue;
            switch (oldValue)
            {
                case 0:
                    return 0;
                case 7:
                    return Config.DaysUntilMature / 4;
                case 14:
                    return Config.DaysUntilMature / 2;
                case 21:
                    return Config.DaysUntilMature * 3 / 4;
            }
            return oldValue;
        }
        private static bool CanItemBePlacedHere(GameLocation location, Vector2 tile, out string deniedMessage)
        {
            deniedMessage = string.Empty;
            if (location is not Farm && !CanPlantAnywhere())
                deniedMessage = "You must enable \"plant anywhere\" to plant trees outside the farm!";
            if (location.getBuildingAt(tile) is not null)
                deniedMessage = "Tile is occupied by a building.";
            if (location.terrainFeatures.TryGetValue(tile, out var terrainFeature) && !(terrainFeature is HoeDirt { crop: null }))
                deniedMessage = $"Tile is occupied by {terrainFeature.GetType()}.";
            if (location.IsTileOccupiedBy(tile, ignorePassables: CollisionMask.Farmers))
                deniedMessage = "Tile is occupied.";
            if (terrainFeature is not null)
                deniedMessage = "Tile is blocked by terrain!";
            if (!location.isTilePlaceable(tile, true))
                deniedMessage = "Tile is not placeable.";
            if (location.objects.ContainsKey(tile))
                deniedMessage = "Tile is occupied by an object.";
            if (!location.IsOutdoors && (!location.treatAsOutdoors.Value && !location.IsGreenhouse))
                deniedMessage = "Cannot place indoors.";
            if (location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Water", "Back") is not null)
                deniedMessage = "Cannot plant in water";
            if (location.IsGreenhouse && location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Type", "Back").Equals("Wood"))
                deniedMessage = "Invalid plant location.";
            if (location.getTileIndexAt((int)tile.X, (int)tile.Y, "Buildings") != -1)
                deniedMessage = "Invalid plant location.";

            return (!string.IsNullOrEmpty(deniedMessage) ? false : true);
        }
        private static Texture2D GetTexture(FruitTree tree, out Rectangle sourceRect)
        {
            if (!textures.TryGetValue(tree.fruit.Name, out var data))
            {
                ParsedItemData fruit = (((int)tree.struckByLightningCountdown.Value > 0) ? ItemRegistry.GetDataOrErrorItem("(O)382") : ItemRegistry.GetDataOrErrorItem(tree.fruit[0].QualifiedItemId));
                textures.Add(tree.fruit.Name, (fruit.GetTexture(), fruit.GetSourceRect()));
            }
            textures.TryGetValue(tree.fruit.Name, out data);
            sourceRect = data.sourceRect;
            return data.texture;
        }
        private static Color GetFruitColor(FruitTree tree, int index)
        {
            if (!Config.EnableMod)
                return Color.White;
            if (!fruitData.TryGetValue(tree.Location, out var dict) || !dict.TryGetValue(tree.Tile, out var data) || data.colors?.Count < tree.fruit.Count)
            {
                ReloadFruit(tree.Location, tree.Tile, tree.fruit.Count);
            }
            fruitData.TryGetValue(tree.Location, out dict);
            dict.TryGetValue(tree.Tile, out data);
            return data.colors[index]; // notes for tomorrow: this is reading null. find out why. and see if texture optimization will let us have 700+ unique draw configs again
        }
        private static float GetFruitScale(FruitTree tree, int index)
        {
            if (!Config.EnableMod)
                return 4;
            if (!fruitData.TryGetValue(tree.Location, out var dict) || !dict.TryGetValue(tree.Tile, out var data) || data.sizes?.Count < tree.fruit.Count)
            {
                ReloadFruit(tree.Location, tree.Tile, tree.fruit.Count);
            }
            fruitData.TryGetValue(tree.Location, out dict);
            dict.TryGetValue(tree.Tile, out data);
            return data.sizes[index];
        }
        private static Vector2 GetFruitOffsetForShake(FruitTree tree, int index)
        {
            if (!Config.EnableMod || index < 2)
                return Vector2.Zero;
            return GetFruitOffset(tree, index);
        }

        private static Vector2 GetFruitOffset(FruitTree tree, int index)
        {
            if (!fruitData.TryGetValue(tree.Location, out var dict) || !dict.TryGetValue(tree.Tile, out var data) || data.offsets?.Count < tree.fruit.Count)
            {
                ReloadFruit(tree.Location, tree.Tile, tree.fruit.Count);
            }
            fruitData.TryGetValue(tree.Location, out dict);
            dict.TryGetValue(tree.Tile, out data);
            return data.offsets[index];
        }

        private static void ReloadFruit(GameLocation location, Vector2 tile, int max)
        {
            
            // init fruit data
            if (!fruitData.ContainsKey(location))
                fruitData.Add(location, new Dictionary<Vector2, (List<Color>, List<float>, List<Vector2>)>());
            if (!fruitData[location].TryGetValue(tile, out var data))
            {
                data.colors = new List<Color>();
                data.sizes = new List<float>();
                data.offsets = new List<Vector2>();
                fruitData[location][tile] = (data.colors, data.sizes, data.offsets);
            }

            // fruit colors
            if (data.colors.Count < max)
            {
                data.colors.Clear();
                for (int i = 0; i < max; i++)
                {
                    var color = Color.White;
                    color.R -= (byte)(Game1.random.NextDouble() * Config.ColorVariation);
                    color.G -= (byte)(Game1.random.NextDouble() * Config.ColorVariation);
                    color.B -= (byte)(Game1.random.NextDouble() * Config.ColorVariation);
                    data.colors.Add(color);
                }
            }
            // fruit sizes
            if (data.sizes.Count < max)
            {
                data.sizes.Clear();
                for (int i = 0; i < max; i++)
                {
                    data.sizes.Add(4 * (float)(1 + ((Game1.random.NextDouble() * 2 - 1) * Config.SizeVariation / 100)));
                }
            }
            // fruit offsets
            if (data.offsets.Count != max)
            {
                data.offsets.Clear();
                SMonitor.Log($"Resetting fruit offsets in {location.Name}");
                for (int i = 0; i < max; i++)
                {

                    if (i < 3)
                    {
                        data.offsets.Add(Vector2.Zero);
                        continue;
                    }
                    bool gotSpot = false;
                    Vector2 offset;
                    while (!gotSpot)
                    {
                        double distance = 24;
                        for (int j = 0; j < 100; j++)
                        {
                            gotSpot = true;
                            offset = new Vector2(Config.FruitSpawnBufferX + Game1.random.Next(34 * 4 - Config.FruitSpawnBufferX), Config.FruitSpawnBufferY + Game1.random.Next(58 * 4 - Config.FruitSpawnBufferY));
                            for (int k = 0; k < data.offsets.Count; k++)
                            {
                                if (Vector2.Distance(data.offsets[k], offset) < distance)
                                {
                                    distance--;
                                    gotSpot = false;
                                    break;
                                }
                            }
                            if (gotSpot)
                            {
                                data.offsets.Add(offset);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}