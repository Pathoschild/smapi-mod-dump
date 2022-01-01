/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/FarmCaveSpawn
**
*************************************************/

using System.Globalization;
using Microsoft.Xna.Framework;

using StardewValley.Locations;


namespace FarmCaveSpawn;

public class ModEntry : Mod
{
    private ModConfig config;
    private readonly List<int> BaseFruit = new() { 296, 396, 406, 410 };
    private List<int> TreeFruit;
    private Random random;
    private AssetManager assetManager;


    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        config = Helper.ReadConfig<ModConfig>();
        assetManager = new();
        helper.Events.GameLoop.DayStarted += SpawnFruit;
        helper.Events.GameLoop.GameLaunched += SetUpConfig;
        helper.ConsoleCommands.Add(
            name: "list_fruits",
            documentation: I18n.ListFruits_Description(),
            callback: this.ListFruits
            );
        helper.Content.AssetLoaders.Add(assetManager);
    }

    /// <summary>
    /// Remove the list TreeFruit when no longer necessary, delete the Random as well
    /// </summary>
    private void Cleanup()
    {
        TreeFruit.Clear();
        TreeFruit.TrimExcess();
        random = null;
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
            mod: ModManifest,
            text: I18n.Mod_Description
            );

        foreach (System.Reflection.PropertyInfo property in typeof(ModConfig).GetProperties())
        {
            if (property.PropertyType.Equals(typeof(bool)))
            {
                configMenu.AddBoolOption(
                    mod: ModManifest,
                    getValue: () => (bool)property.GetValue(config),
                    setValue: (bool value) => property.SetValue(config, value),
                    name: () => I18n.GetByKey($"{property.Name}.title"),
                    tooltip: () => I18n.GetByKey($"{property.Name}.description")
                   );
            }
            else if (property.PropertyType.Equals(typeof(int)))
            {
                configMenu.AddNumberOption(
                    mod: ModManifest,
                    getValue: () => (int)property.GetValue(config),
                    setValue: (int value) => property.SetValue(config, value),
                    name: () => I18n.GetByKey($"{property.Name}.title"),
                    tooltip: () => I18n.GetByKey($"{property.Name}.description"),
                    min: 0,
                    interval: 1
                );
            }
            else if (property.PropertyType.Equals(typeof(float)))
            {
                configMenu.AddNumberOption(
                    mod: ModManifest,
                    getValue: () => (float)property.GetValue(config),
                    setValue: (float value) => property.SetValue(config, value),
                    name: () => I18n.GetByKey($"{property.Name}.title"),
                    tooltip: () => I18n.GetByKey($"{property.Name}.description"),
                    min: 0.0f
                );
            }
            else { Monitor.Log($"{property.Name} unaccounted for.", LogLevel.Trace); }
        }
    }

    private void SpawnFruit(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
    {
        if (!Context.IsMainPlayer) { return; }
        if (!config.EarlyFarmCave && (Game1.MasterPlayer.caveChoice?.Value is null || Game1.MasterPlayer.caveChoice.Value <= 0)) { return; }
        if (!config.IgnoreFarmCaveType && (Game1.MasterPlayer.caveChoice?.Value is null || Game1.MasterPlayer.caveChoice.Value != 1)) { return; }
        int count = 0;
        TreeFruit = GetTreeFruits();
        random = new((int)Game1.uniqueIDForThisGame * 2 + (int)Game1.stats.DaysPlayed * 7);
        FarmCave farmcave = Game1.getLocationFromName("FarmCave") as FarmCave;

        foreach (Vector2 v in IterateTiles(farmcave))
        {
            PlaceFruit(farmcave, v);
            if (++count >= config.MaxDailySpawns) { break; }
        }
        farmcave.UpdateReadyFlag();
        if (count >= config.MaxDailySpawns) { Cleanup(); return; }

        foreach (string location in GetData(assetManager.additionalLocationsLocation))
        {
            GameLocation gameLocation = Game1.getLocationFromName(location);
            if (gameLocation is not null)
            {
                Monitor.Log($"Found {gameLocation}");
                foreach (Vector2 v in IterateTiles(gameLocation))
                {
                    PlaceFruit(gameLocation, v);
                    if (++count >= config.MaxDailySpawns) { Cleanup(); return; }
                }
            }
        }

        if (config.UseMineCave && Game1.getLocationFromName("Mine") is Mine mine)
        {
            foreach (Vector2 v in IterateTiles(mine, xstart: 11))
            {
                PlaceFruit(mine, v);
                if (++count >= config.MaxDailySpawns) { Cleanup(); return; }
            }
        }
    }

    public void PlaceFruit(GameLocation location, Vector2 tile)
    {
        int fruitToPlace = Utility.GetRandom<int>(random.NextDouble() < (config.TreeFruitChance / 100f) ? TreeFruit : BaseFruit, random);
        location.setObject(tile, new StardewValley.Object(fruitToPlace, 1)
        {
            IsSpawnedObject = true
        });
        Monitor.Log($"Spawning item {fruitToPlace} at {location.Name}:{tile.X},{tile.Y}");
    }

    public IEnumerable<Vector2> IterateTiles(GameLocation location, int xstart = 1, int xend = int.MaxValue, int ystart = 1, int yend = int.MaxValue)
    {
        foreach (int x in Enumerable.Range(xstart, Math.Clamp(xend, xstart, location.Map.Layers[0].LayerWidth - 2)).OrderBy((x) => random.Next()))
        {
            foreach (int y in Enumerable.Range(ystart, Math.Clamp(yend, ystart, location.Map.Layers[0].LayerHeight - 2)).OrderBy((x) => random.Next()))
            {
                Vector2 v = new(x, y);
                if (random.NextDouble() < (config.SpawnChance / 100f) && location.isTileLocationTotallyClearAndPlaceableIgnoreFloors(v))
                {
                    yield return v;
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

    private List<string> GetData(string datalocation)
    {
        IDictionary<string, string> rawlist = Helper.Content.Load<Dictionary<string, string>>(datalocation, ContentSource.GameContent);
        List<string> datalist = new();

        foreach (string uniqueID in rawlist.Keys)
        {
            if (Helper.ModRegistry.IsLoaded(uniqueID))
            {
                datalist.AddRange(rawlist[uniqueID].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
            }
        }
        return datalist;
    }

    private List<int> GetTreeFruits()
    {

        List<string> denylist = GetData(assetManager.denylistLocation);

        List<int> TreeFruits = new();
        Dictionary<int, string> fruittrees = Helper.Content.Load<Dictionary<int, string>>("Data/fruitTrees", ContentSource.GameContent);
        string currentseason = Game1.currentSeason.ToLower().Trim();
        foreach (string tree in fruittrees.Values)
        {
            string[] treedata = tree.Split('/');
            if (config.SeasonalOnly && Context.IsWorldReady)
            {
                if (!treedata[1].Contains(currentseason))
                {
                    if (!currentseason.Contains("summer") || !treedata[1].Contains("island"))
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
                    if (fruit.Price > config.PriceCap)
                    {
                        continue;
                    }
                    if (denylist.Contains(fruit.Name))
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
