/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/FarmCaveSpawn
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FarmCaveSpawn
{

    class ModConfig
    {
        public int MaxDailySpawns { get; set; } = 6; //Maximum number of spawns.

        public float SpawnChance { get; set; } = 5f; //probability of any tile spawning a thing.
        public float TreeFruitChance { get; set; } = 50f; //probability of the spawn being a tree fruit.
        public bool IgnoreFarmCaveType { get; set; } = false; //should I spawn fruits regardless of the farm cave type?
        public bool EarlyFarmCave { get; set; } = false; //allow spawn of fruits even before Demetrius shows up.
        public bool UseMineCave { get; set; } = false; //allow spawn of fruits into the mine cave (after fruit cave)
        public bool SeasonalOnly { get; set; } = false; //limit to just seasonal tree fruit.
        public bool AllowAnyTreeProduct { get; set; } = true;
        public bool EdiblesOnly { get; set; } = true;
        public bool NoBananasBeforeShrine { get; set; } = true;
    }
    public class ModEntry: Mod
    {
        private ModConfig config;
        private readonly List<int> BaseFruit = new() { 296, 396, 406, 410 };
        private List<int> TreeFruit;
        private Random random;
        public override void Entry(IModHelper helper)
        {
            config = Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.DayStarted += SpawnFruit;
            helper.Events.GameLoop.GameLaunched += SetUpConfig;
            helper.ConsoleCommands.Add("list_fruits", helper.Translation.Get("list-fruits.description"), this.ListFruits);
        }

        private void SetUpConfig(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: ModManifest,
                reset: () => config = new ModConfig(),
                save: () => Helper.WriteConfig(config));

            configMenu.AddParagraph(
                mod:ModManifest,
                text: () => Helper.Translation.Get("mod.description")
                );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => config.MaxDailySpawns,
                setValue: value => config.MaxDailySpawns = value,
                name: () => Helper.Translation.Get("max-spawns.title"),
                tooltip: () => Helper.Translation.Get("max-spawns.description"),
                min: 0,
                interval: 1
                );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => config.SpawnChance,
                setValue: value => config.SpawnChance = value,
                name: () => Helper.Translation.Get("spawn-chance.title"),
                tooltip: () => Helper.Translation.Get("spawn-chance.description"),
                min: 0.0f
                );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => config.TreeFruitChance,
                setValue: value => config.TreeFruitChance = value,
                name: () => Helper.Translation.Get("tree-fruit-chance.title"),
                tooltip: () => Helper.Translation.Get("tree-fruit-chance.description"),
                min: 0.0f
                );

            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => config.IgnoreFarmCaveType,
                setValue: value => config.IgnoreFarmCaveType = value,
                name: () => Helper.Translation.Get("cave-type.title"),
                tooltip: () => Helper.Translation.Get("cave-type.description")
                );

            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => config.EarlyFarmCave,
                setValue: value => config.EarlyFarmCave = value,
                name: () => Helper.Translation.Get("early-cave.title"),
                tooltip: ()=> Helper.Translation.Get("early-cave.description")
                ) ;

            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => config.UseMineCave,
                setValue: value => config.UseMineCave = value,
                name: () => Helper.Translation.Get("use-mine.title"),
                tooltip: () => Helper.Translation.Get("use-mine.description")
                );

            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => config.SeasonalOnly,
                setValue: value => config.SeasonalOnly = value,
                name: () => Helper.Translation.Get("seasonal-only.title"),
                tooltip: () => Helper.Translation.Get("seasonal-only.dsecription")
                );

            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => config.AllowAnyTreeProduct,
                setValue: value => config.AllowAnyTreeProduct = value,
                name: () => Helper.Translation.Get("any-category.title"),
                tooltip: () => Helper.Translation.Get("any-category.dsecription")
                );

            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () =>config.EdiblesOnly,
                setValue: value => config.EdiblesOnly = value,
                name: () => Helper.Translation.Get("edibles.title"),
                tooltip: () => Helper.Translation.Get("edibles.description")
                );

            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => config.NoBananasBeforeShrine,
                setValue: value => config.NoBananasBeforeShrine = value,
                name: () => Helper.Translation.Get("nobananas.title"),
                tooltip: () => Helper.Translation.Get("nobananas.description")
                );
        }

        private void SpawnFruit(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            if (!Game1.IsMasterGame) { return; }
            if (!config.EarlyFarmCave && (Game1.MasterPlayer.caveChoice?.Value == null || Game1.MasterPlayer.caveChoice.Value <= 0)) {return;}
            int count = 0;
            TreeFruit = GetTreeFruits();
            random = new((int)Game1.uniqueIDForThisGame * 2 + (int)Game1.stats.DaysPlayed * 7);
            FarmCave farmcave = Game1.getLocationFromName("FarmCave") as FarmCave;

            foreach (Vector2 v in IterateTiles(farmcave))
            {
                PlaceFruit(farmcave, v);
                count++;
                if (count >= config.MaxDailySpawns) {break;}
            }
            farmcave.UpdateReadyFlag();
            if (count>=config.MaxDailySpawns) { return; }

            //For SVE:
            if (Helper.ModRegistry.IsLoaded("FlashShifter.SVECode"))
            {
                GameLocation minecartCave = Game1.getLocationFromName("Custom_MinecartCave");
                if (minecartCave != null)
                {
                    Monitor.Log("Found SVE minecart cave.");
                    foreach (Vector2 v in IterateTiles(minecartCave))
                    {
                        PlaceFruit(minecartCave, v);
                        count++;
                        if (count>=config.MaxDailySpawns) { return; }
                    }
                }

                GameLocation deepCave = Game1.getLocationFromName("Custom_DeepCave");
                if (deepCave !=null)
                {
                    Monitor.Log("Found SVE deep cave.");
                    foreach (Vector2 v in IterateTiles(deepCave))
                    {
                        PlaceFruit(deepCave, v);
                        count++;
                        if (count >= config.MaxDailySpawns) { return; }
                    }
                }
            }

            if (config.UseMineCave && Game1.getLocationFromName("Mine") is Mine mine)
            {
                foreach (Vector2 v in IterateTiles(mine, xstart: 11))
                {
                    PlaceFruit(mine, v);
                    count++;
                    if (count >= config.MaxDailySpawns) { return; }
                }
            }
        }

        public void PlaceFruit(GameLocation location, Vector2 tile)
        {
            int fruitToPlace;
            if (random.NextDouble() < (config.TreeFruitChance / 100f))
            {
                fruitToPlace = Utility.GetRandom<int>(TreeFruit, random);
            }
            else
            {
                fruitToPlace = Utility.GetRandom<int>(BaseFruit, random);
            }
            location.setObject(tile, new StardewValley.Object(fruitToPlace, 1)
            {
                IsSpawnedObject = true
            });
            Monitor.Log($"Spawning item {fruitToPlace} at {location.Name}:{tile.X},{tile.Y}");
        }

        public IEnumerable<Vector2> IterateTiles(GameLocation location, int xstart = 1, int xend = -1, int ystart = 1, int yend = -1)
        {
            if (config.IgnoreFarmCaveType || Game1.MasterPlayer.caveChoice?.Value == 1)
            {
                foreach (int x in Enumerable.Range(xstart, Math.Max(location.Map.Layers[0].LayerWidth - xend-1, xstart)).OrderBy((x) => random.Next()))
                {
                    foreach (int y in Enumerable.Range(ystart, Math.Max(location.Map.Layers[0].LayerHeight - yend-1, ystart)).OrderBy((x) => random.Next()))
                    {
                        Vector2 v = new(x, y);
                        if (random.NextDouble() < (config.SpawnChance / 100f) && location.isTileLocationTotallyClearAndPlaceableIgnoreFloors(v))
                        {
                            yield return v;
                        }
                    }
                }
            }
        }

        private void ListFruits(string command, string[] args)
        {
            if (!Context.IsWorldReady)
            {
                Monitor.Log("World is not ready. Please load save first.");
                return;
            }
            List<string> FruitNames = new();
            foreach (int objectID in GetTreeFruits())
            {
                StardewValley.Object obj = new(objectID, 1);
                FruitNames.Add(obj.DisplayName);
            }
            LocalizedContentManager contextManager = Game1.content;
            string langcode = contextManager.LanguageCodeString(contextManager.GetCurrentLanguage());
            FruitNames.Sort(StringComparer.Create(new CultureInfo(langcode), true));
            Monitor.Log($"Possible fruits: {String.Join(", ", FruitNames)}", LogLevel.Info);
        }

        private List<int> GetTreeFruits()
        {
            List<int> TreeFruits = new();
            Dictionary<int, string> fruittrees= Helper.Content.Load<Dictionary<int, string>>("Data/fruitTrees", ContentSource.GameContent);
            string currentseason = Game1.currentSeason.ToLower().Trim();
            foreach (string tree in fruittrees.Values)
            {
                string[] treedata = tree.Split('/');
                if (config.SeasonalOnly && Context.IsWorldReady)
                {
                    if(!treedata[1].Contains(currentseason))
                    {
                        if(!currentseason.Contains("summer")||!treedata[1].Contains("island"))
                        {
                            continue;
                        }
                    }
                }

                bool success = int.TryParse(treedata[2].Trim(), out int objectIndex);
                if (success)
                {
                    try
                    {
                        StardewValley.Object fruit = new(objectIndex, 1);
                        if (!config.AllowAnyTreeProduct && fruit.Category != StardewValley.Object.FruitsCategory)
                        {
                            continue;
                        }
                        if (config.EdiblesOnly && fruit.Edibility < 0)
                        {
                            continue;
                        }
                        if (config.NoBananasBeforeShrine && fruit.Name.Equals("Banana"))
                        {
                            if (!Context.IsWorldReady) { continue; }
                            IslandEast islandeast = Game1.getLocationFromName("IslandEast") as IslandEast;
                            if (!islandeast.bananaShrineComplete.Value) { continue; }
                        }
                        TreeFruits.Add(objectIndex);
                    }
                    catch (Exception ex)
                    {
                        Monitor.Log($"Ran into issue looking up item {objectIndex}\n{ex}", LogLevel.Warn);
                    }
                }
            }
            return TreeFruits;
        }
    }
}
