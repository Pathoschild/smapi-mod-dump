/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chiccenFL/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.TerrainFeatures;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.GameData.WildTrees;
using StardewValley.Extensions;
using StardewValley.Internal;
using Object = StardewValley.Object;
using Context = StardewModdingAPI.Context;

namespace WildTreeTweaks
{
    public partial class ModEntry
    {

        public static bool canPlaceWildTreeSeed(Object tree, GameLocation location, Vector2 tile, out string deniedMessage)
        {
            /*if (!tree.IsWildTreeSapling())
            {
                deniedMessage = string.Empty;
                return false;
            }*/

            if (location.getBuildingAt(tile) is not null)
            {
                deniedMessage = "Tile is occupied by a building.";
                return false;
            }
            if (location.terrainFeatures.TryGetValue(tile, out var terrainFeature) && !(terrainFeature is HoeDirt { crop: null }))
            {
                deniedMessage = "Tile is occupied by crops.";
                return false;
            }
            if (location.IsTileOccupiedBy(tile, ignorePassables: CollisionMask.Farmers))
            {
                deniedMessage = "Tile is occupied.";
                return false;
            }
            if (terrainFeature is not null && !terrainFeature.isPassable())
            {
                deniedMessage = "Tile is blocked by terrain!";
                return false;
            }
            if (!location.isTilePlaceable(tile, true))
            {
                deniedMessage = "Tile is not placeable.";
                return false;
            }
            if (location.objects.ContainsKey(tile))
            {
                deniedMessage = "Tile is occupied by an object.";
                return false;
            }
            if(!location.IsOutdoors && (!location.treatAsOutdoors.Value && !location.IsGreenhouse))
            {
                deniedMessage = "Cannot place indoors.";
                return false;
            }
            if (location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Water", "Back") is not null)
            {
                deniedMessage = "Cannot plant in water";
                return false;
            }
            if ((location.IsGreenhouse && location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Type", "Back").Equals("Wood")) || location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Type", "Back").Equals("Stone"))
            {
                deniedMessage = "Invalid plant location.";
                return false;
            }
            if (location.getTileIndexAt((int)tile.X, (int)tile.Y, "Buildings") != -1)
            {
                deniedMessage = "Invalid plant location.";
                return false;
            }

            deniedMessage = string.Empty;
            return true;
        }

        private static int extraWoodCalculator(Vector2 tileLocation)
        {
            Random random = Utility.CreateRandom(Game1.uniqueIDForThisGame, Game1.stats.DaysPlayed, (double)tileLocation.X * 7.0, (double)tileLocation.Y * 11.0);
            int extraWood = 0;
            if (random.NextDouble() < Game1.player.DailyLuck)
            {
                extraWood++;
            }
            if (random.NextDouble() < (double)Game1.player.ForagingLevel / 12.5)
            {
                extraWood++;
            }
            if (random.NextDouble() < (double)Game1.player.ForagingLevel / 12.5)
            {
                extraWood++;
            }
            if (random.NextDouble() < (double)Game1.player.LuckLevel / 25.0)
            {
                extraWood++;
            }
            return extraWood;
        }

        public static Item TryGetDrop(Tree tree, WildTreeItemData drop, Random r, Farmer targetFarmer, string fieldName, Func<string, string> formatItemId = null, bool? isStump = null)
        {
            if (!r.NextBool(drop.Chance))
            {
                return null;
            }
            if (drop.Season.HasValue && drop.Season != tree.Location.GetSeason())
            {
                return null;
            }
            if (drop.Condition != null && !GameStateQuery.CheckConditions(drop.Condition, tree.Location, targetFarmer, null, null, r))
            {
                return null;
            }
            if (drop is WildTreeChopItemData chopItemData && !chopItemData.IsValidForGrowthStage(tree.growthStage.Value, isStump ?? tree.stump.Value))
            {
                return null;
            }
            return ItemQueryResolver.TryResolveRandomItem(drop, new ItemQueryContext(tree.Location, targetFarmer, r), avoidRepeat: false, null, formatItemId, null, delegate (string query, string error)
            {
                Log($"Wild tree '{tree.treeType.Value}' failed parsing item query '{query}' for {fieldName} entry '{drop.Id}': {error}", StardewModdingAPI.LogLevel.Error);
            });
        }

        public static bool performTreeFall(Tree tree, Tool t, int explosion, Vector2 tileLocation)
        {
            GameLocation location = tree.Location;
            WildTreeData data = tree.GetData();
            tree.Location.objects.Remove(tree.Tile);
            tree.tapped.Value = false;
            if (!tree.stump.Value)
            {
                if (t != null || explosion > 0)
                {
                    location.playSound("treecrack");
                }
                tree.stump.Value = true;
                tree.health.Value = 5f;
                tree.falling.Value = true;
                if (t != null && t.getLastFarmerToUse().IsLocalPlayer)
                {
                    t?.getLastFarmerToUse().gainExperience(2, 12);
                    if (t?.getLastFarmerToUse() == null)
                    {
                        tree.shakeLeft.Value = true;
                    }
                    else
                    {
                        tree.shakeLeft.Value = (float)t.getLastFarmerToUse().StandingPixel.X > (tileLocation.X + 0.5f) * 64f;
                    }
                    t.getLastFarmerToUse().stats.Increment("TreesChopped", 1);
                }
            }
            else
            {
                if (t != null && tree.health.Value != -100f && t.getLastFarmerToUse().IsLocalPlayer)
                {
                    t?.getLastFarmerToUse().gainExperience(2, 1);
                }
                tree.health.Value = -100f;
                if (data != null)
                {
                    Game1.createRadialDebris(location, 12, (int)tileLocation.X, (int)tileLocation.Y, Game1.random.Next(30, 40), resource: false, -1, item: false, tree.GetChopDebrisColor(data));
                    Random r;
                    if (Game1.IsMultiplayer)
                    {
                        Game1.recentMultiplayerRandom = Utility.CreateRandom((double)tileLocation.X * 2000.0, tileLocation.Y);
                        r = Game1.recentMultiplayerRandom;
                    }
                    else
                    {
                        r = Utility.CreateRandom(Game1.uniqueIDForThisGame, Game1.stats.DaysPlayed, (double)tileLocation.X * 7.0, (double)tileLocation.Y * 11.0);
                    }
                    if (t?.getLastFarmerToUse() == null)
                    {
                        if (location.Equals(Game1.currentLocation))
                        {
                            Game1.createMultipleObjectDebris("(O)92", (int)tileLocation.X, (int)tileLocation.Y, 2, location);
                        }
                        else
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Game1.createItemDebris(ItemRegistry.Create("(O)92"), tileLocation * 64f, 2, location);
                            }
                        }
                    }
                    else if (Game1.IsMultiplayer)
                    {
                        if (data.DropWoodOnChop)
                        {
                            Game1.createRadialDebris(location, 12, (int)tileLocation.X, (int)tileLocation.Y, (int)((Game1.getFarmer(tree.lastPlayerToHit.Value).professions.Contains(12) ? 1.25 : 1.0) * 4.0), resource: true);
                        }
                        List<WildTreeChopItemData> chopItems = data.ChopItems;
                        if (chopItems != null && chopItems.Count > 0)
                        {
                            Farmer targetFarmer2 = Game1.getFarmer(tree.lastPlayerToHit.Value);
                            foreach (WildTreeChopItemData drop in data.ChopItems)
                            {
                                Item item = TryGetDrop(tree , drop, r, targetFarmer2, "ChopItems");
                                if (item != null)
                                {
                                    if (item.QualifiedItemId == "(O)420" && tileLocation.X % 7f == 0f)
                                    {
                                        item = ItemRegistry.Create("(O)422", item.Stack, item.Quality);
                                    }
                                    Game1.createMultipleItemDebris(item, tileLocation * 64f, -2, location);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (data.DropWoodOnChop)
                        {
                            Game1.createRadialDebris(location, 12, (int)tileLocation.X, (int)tileLocation.Y, (int)((Game1.getFarmer(tree.lastPlayerToHit.Value).professions.Contains(12) ? 1.25 : 1.0) * (double)(5 + extraWoodCalculator(tileLocation))), resource: true);
                        }
                        List<WildTreeChopItemData> chopItems2 = data.ChopItems;
                        if (chopItems2 != null && chopItems2.Count > 0)
                        {
                            Farmer targetFarmer = Game1.getFarmer(tree.lastPlayerToHit.Value);
                            foreach (WildTreeChopItemData drop2 in data.ChopItems)
                            {
                                Item item2 = TryGetDrop(tree, drop2, r, targetFarmer, "ChopItems");
                                if (item2 != null)
                                {
                                    if (item2.QualifiedItemId == "(O)420" && tileLocation.X % 7f == 0f)
                                    {
                                        item2 = ItemRegistry.Create("(O)422", item2.Stack, item2.Quality);
                                    }
                                    Game1.createMultipleItemDebris(item2, tileLocation * 64f, -2, location);
                                }
                            }
                        }
                    }
                    if (Game1.random.NextDouble() <= 0.25 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
                    {
                        Game1.createObjectDebris("(O)890", (int)tileLocation.X, (int)tileLocation.Y - 3, ((int)tileLocation.Y + 1) * 64, 0, 1f, location);
                    }
                    location.playSound("treethud");
                }
                if (!tree.falling.Value)
                {
                    return true;
                }
            }
            return false;
        }

        private void Player_Warped(object sender, WarpedEventArgs e)
        {
            GameLocation location = e.NewLocation;
            if (!Config.EnableMod || !updateTrees || (!location.IsFarm && Config.OnlyOnFarm) || (!location.IsOutdoors && !location.treatAsOutdoors.Value)) return;
            Log($"Updating location {e.NewLocation.NameOrUniqueName}", debugOnly: true);

            updateLocations.Add(location);
            updateTrees = (updateLocations.Count < Game1.locations.Count || (location.IsFarm && Config.OnlyOnFarm));

            foreach (TerrainFeature feature in location._activeTerrainFeatures)
            {
                if (feature is not Tree) continue;
                Tree tree = (Tree)feature;
                tree.GetData();
                tree.health.Value = Config.Health;

            }

            leaves.Clear();
        }

        /// <summary>
        /// Resets all stumps in a location to regular, fully grown trees. Should exclusively be used for debugging or bug fixing.
        /// </summary>
        private void FixStumps(string command, string[] args)
        {
            if (!Context.IsPlayerFree)
            {
                Log("Cannot run command unless player is free. Try again when game is loaded and player is not in any cutscene or dialogue.", LogLevel.Warn);
            }
            Log("Read command \"fix_stumps\". Resetting stumps back to false. Warning: this may potentially cause some performance drop until this action is completed.", LogLevel.Alert);
            foreach (GameLocation l in Game1.locations)
            {
                foreach (TerrainFeature feature in l._activeTerrainFeatures)
                {
                    if (feature is not Tree) continue;
                    Tree tree = (Tree)feature;
                    if (tree.stump.Value)
                        tree.stump.Value = false;
                }
            }
            Log("Stump fix completed!", LogLevel.Alert);
        }
    }
}
