/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/FarmCaveSpawn
**
*************************************************/

using System.Reflection;
using System.Text.RegularExpressions;
using AtraShared.Integrations;
using AtraShared.Utils.Extensions;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Locations;
using AtraUtils = AtraShared.Utils.Utils;

namespace FarmCaveSpawn;

/// <inheritdoc />
public class ModEntry : Mod
{
    private readonly AssetManager assetManager = new();

    /// <summary>
    /// Sublocation-parsing regex.
    /// </summary>
    private readonly Regex regex = new(
        // ":[(x1;y1);(x2;y2)]"
        pattern: @":\[\((?<x1>[0-9]+);(?<y1>[0-9]+)\);\((?<x2>[0-9]+);(?<y2>[0-9]+)\)\]$",
        options: RegexOptions.CultureInvariant | RegexOptions.Compiled,
        matchTimeout: TimeSpan.FromMilliseconds(250));

    /// <summary>
    /// The item IDs for the four basic forage fruit.
    /// </summary>
    private readonly List<int> BASE_FRUIT = new() { 296, 396, 406, 410 };

    /// <summary>
    /// A list of vanilla fruit.
    /// </summary>
    private readonly List<int> VANILLA_FRUIT = new() { 613, 634, 635, 636, 637, 638 };

    /// <summary>
    /// Item IDs for items produced by trees.
    /// </summary>
    private List<int> TreeFruit = new();

    // The config is set by the Entry method, so it should never realistically be null
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ModConfig config;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <summary>
    /// Location to temporarily store the seeded random.
    /// </summary>
    private Random? random;

    /// <summary>
    /// Gets the seeded random for this mod.
    /// </summary>
    internal Random Random
    {
        get
        {
            if (this.random is null)
            {
                this.random = new Random(((int)Game1.uniqueIDForThisGame * 2) + ((int)Game1.stats.DaysPlayed * 7));
            }
            return this.random;
        }
    }

    /// <summary>
    /// Gets a value indicating whether or not I've spawned fruit today.
    /// </summary>
    internal bool SpawnedFruitToday { get; private set; }

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
#if DEBUG
        this.Monitor.Log("FarmCaveSpawn initializing, DEBUG mode. Do not release this version", LogLevel.Warn);
#endif
        I18n.Init(helper.Translation);
        try
        {
            this.config = this.Helper.ReadConfig<ModConfig>();
        }
        catch
        {
            this.Monitor.Log(I18n.IllFormatedConfig(), LogLevel.Warn);
            this.config = new();
        }

        helper.Events.GameLoop.DayStarted += this.SpawnFruit;
        helper.Events.GameLoop.GameLaunched += this.SetUpConfig;
        helper.Events.GameLoop.OneSecondUpdateTicking += this.BellsAndWhistles;
        helper.ConsoleCommands.Add(
            name: "av.fcs.list_fruits",
            documentation: I18n.ListFruits_Description(),
            callback: this.ListFruits);

        helper.Content.AssetLoaders.Add(this.assetManager);
    }

    /// <summary>
    /// Remove the list TreeFruit when no longer necessary, delete the Random as well.
    /// </summary>
    private void Cleanup()
    {
        this.TreeFruit.Clear();
        this.TreeFruit.TrimExcess();
        this.random = null;
    }

    /// <summary>
    /// Generates the GMCM for this mod by looking at the structure of the config class.
    /// </summary>
    /// <param name="sender">Unknown, expected by SMAPI.</param>
    /// <param name="e">Arguments for eevnt.</param>
    /// <remarks>To add a new setting, add the details to the i18n file. Currently handles: bool, int, float.</remarks>
    private void SetUpConfig(object? sender, GameLaunchedEventArgs e)
    {
        GMCMHelper helper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, this.ModManifest);
        if (!helper.TryGetAPI())
        {
            return;
        }

        helper.Register(
                reset: () => this.config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.config))
            .AddParagraph(I18n.Mod_Description);

        foreach (PropertyInfo property in typeof(ModConfig).GetProperties())
        {
            if (property.PropertyType.Equals(typeof(bool)))
            {
                helper.AddBoolOption(property, () => this.config);
            }
            else if (property.PropertyType.Equals(typeof(int)))
            {
                helper.AddIntOption(
                    property: property,
                    getConfig: () => this.config,
                    min: 0,
                    max: property.Name == "MaxDailySpawns" ? 100 : 1000);
            }
            else if (property.PropertyType.Equals(typeof(float)))
            {
                helper.AddFloatOption(
                    property: property,
                    getConfig: () => this.config,
                    min: 0f,
                    max: 100f);
            }
            else if (property.PropertyType.Equals(typeof(SeasonalBehavior)))
            {
                helper.AddEnumOption<ModConfig, SeasonalBehavior>(
                    property: property,
                    getConfig: () => this.config);
            }
            else
            {
                this.Monitor.DebugLog($"{property.Name} unaccounted for.", LogLevel.Warn);
            }
        }
    }

    /// <summary>
    /// Whether or not I should spawn fruit (according to config + game state).
    /// </summary>
    /// <returns>True if I should spawn fruit, false otherwise.</returns>
    private bool ShouldSpawnFruit()
    {
        // Compat for Farm Cave Framework: https://www.nexusmods.com/stardewvalley/mods/10506
        // Which saves the farm cave choice to their own SaveData, and doesn't update the MasterPlayer.caveChoice
        bool hasFCFbatcave = false;
        if (Game1.CustomData.TryGetValue("smapi/mod-data/aedenthorn.farmcaveframework/farm-cave-framework-choice", out string? farmcavechoice))
        {
            // Crosscheck this = probably better to just use the actual value, maybe...
            hasFCFbatcave = (farmcavechoice is not null) && (farmcavechoice.ToLowerInvariant().Contains("bat") || farmcavechoice.ToLowerInvariant().Contains("fruit"));
            this.Monitor.DebugLog(hasFCFbatcave ? "FarmCaveFramework fruit bat cave detected." : "FarmCaveFramework fruit bat cave not detected.");
        }

        if (!this.config.EarlyFarmCave
            && (Game1.MasterPlayer.caveChoice?.Value is null || Game1.MasterPlayer.caveChoice.Value <= Farmer.caveNothing)
            && string.IsNullOrWhiteSpace(farmcavechoice))
        {
            this.Monitor.DebugLog("Demetrius cutscene not seen and config not set to early, skip spawning for today.");
            return false;
        }
        if (!this.config.IgnoreFarmCaveType && !this.config.EarlyFarmCave
            && (Game1.MasterPlayer.caveChoice?.Value is null || Game1.MasterPlayer.caveChoice.Value != Farmer.caveBats)
            && !hasFCFbatcave)
        {
            this.Monitor.DebugLog("Fruit bat cave not selected and config not set to ignore that, skip spawning for today.");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Handle spawning fruit at the start of each day.
    /// </summary>
    /// <param name="sender">Unknown, unused.</param>
    /// <param name="e">Arguments.</param>
    private void SpawnFruit(object? sender, DayStartedEventArgs e)
    {
        if (!this.ShouldSpawnFruit())
        {
            this.SpawnedFruitToday = false;
            return;
        }

        this.SpawnedFruitToday = true;

        if (!Context.IsMainPlayer)
        {
            return;
        }

        int count = 0;
        this.TreeFruit = this.GetTreeFruits();

        if (Game1.getLocationFromName("FarmCave") is FarmCave farmcave)
        {
            this.Monitor.DebugLog($"Spawning in the farmcave");
            foreach (Vector2 v in this.IterateTiles(farmcave))
            {
                this.PlaceFruit(farmcave, v);
                if (++count >= this.config.MaxDailySpawns)
                {
                    break;
                }
            }
            farmcave.UpdateReadyFlag();
            if (count >= this.config.MaxDailySpawns)
            {
                this.Cleanup();
                return;
            }
        }

        if (this.config.UseModCaves)
        {
            foreach (string location in this.GetData(this.assetManager.ADDITIONAL_LOCATIONS_LOCATION))
            {
                string parseloc = location;
                // initialize default limits
                Dictionary<string, int> locLimits = new()
                {
                    ["x1"] = 1,
                    ["x2"] = int.MaxValue,
                    ["y1"] = 1,
                    ["y2"] = int.MaxValue,
                };
                try
                {
                    MatchCollection matches = this.regex.Matches(location);
                    if (matches.Count == 1)
                    {
                        Match match = matches[0];
                        parseloc = location[..^match.Value.Length];
                        foreach (Group group in match.Groups)
                        {
                            if (int.TryParse(group.Value, out int result))
                            {
                                locLimits[group.Name] = result;
                            }
                        }
                        this.Monitor.DebugLog($"Found and parsed sublocation: {parseloc} + ({locLimits["x1"]};{locLimits["y1"]});({locLimits["x2"]};{locLimits["y2"]})");
                    }
                    else if (matches.Count >= 2)
                    {
                        this.Monitor.Log(I18n.ExcessRegexMatches(loc: location), LogLevel.Warn);
                        continue;
                    }
                }
                catch (RegexMatchTimeoutException ex)
                {
                    this.Monitor.Log(I18n.RegexTimeout(loc: location, ex: ex), LogLevel.Warn);
                }

                if (Game1.getLocationFromName(parseloc) is GameLocation gameLocation)
                {
                    this.Monitor.Log($"Found {gameLocation}");
                    foreach (Vector2 v in this.IterateTiles(gameLocation, xstart: locLimits["x1"], xend: locLimits["x2"], ystart: locLimits["y1"], yend: locLimits["y2"]))
                    {
                        this.PlaceFruit(gameLocation, v);
                        if (++count >= this.config.MaxDailySpawns)
                        {
                            this.Cleanup();
                            return;
                        }
                    }
                }
                else
                {
                    this.Monitor.Log(I18n.LocationMissing(loc: location), LogLevel.Debug);
                }
            }
        }

        if (this.config.UseMineCave && Game1.getLocationFromName("Mine") is Mine mine)
        {
            foreach (Vector2 v in this.IterateTiles(mine, xstart: 11))
            {
                this.PlaceFruit(mine, v);
                if (++count >= this.config.MaxDailySpawns)
                {
                    this.Cleanup();
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Place a fruit on a specific tile.
    /// </summary>
    /// <param name="location">Map to place fruit on.</param>
    /// <param name="tile">Tile to place fruit on.</param>
    private void PlaceFruit(GameLocation location, Vector2 tile)
    {
        int fruitToPlace = Utility.GetRandom(this.Random.NextDouble() < (this.config.TreeFruitChance / 100f) && this.TreeFruit.Count > 0 ? this.TreeFruit : this.BASE_FRUIT, this.Random);
        location.setObject(tile, new SObject(fruitToPlace, 1)
        {
            IsSpawnedObject = true,
        });
        this.Monitor.DebugLog($"Spawning item {fruitToPlace} at {location.Name}:{tile.X},{tile.Y}", LogLevel.Debug);
    }

    /// <summary>
    /// Iterate over tiles in a map, with a random chance to pick each tile.
    /// Will only return clear and placable tiles.
    /// </summary>
    /// <param name="location">Map to iterate over.</param>
    /// <param name="xstart">X coordinate to start.</param>
    /// <param name="xend">X coordinate to end.</param>
    /// <param name="ystart">Y coordinate to start.</param>
    /// <param name="yend">Y coordinte to end.</param>
    /// <returns>Enumerable of tiles for which to place fruit.</returns>
    /// <remarks>The start and end coordinates are clamped to the size of the map, so there shouldn't be a way to give this function invalid values.</remarks>
    private IEnumerable<Vector2> IterateTiles(GameLocation location, int xstart = 1, int xend = int.MaxValue, int ystart = 1, int yend = int.MaxValue)
    {
        List<Vector2> points = Enumerable.Range(Math.Max(xstart, 1), Math.Clamp(xend, xstart, location.Map.Layers[0].LayerWidth - 2))
            .SelectMany(x => Enumerable.Range(Math.Max(ystart, 1), Math.Clamp(yend, ystart, location.Map.Layers[0].LayerHeight - 2)), (x, y) => new Vector2(x, y)).ToList();
        Utility.Shuffle(this.Random, points);
        foreach (Vector2 v in points)
        {
            if (this.Random.NextDouble() < (this.config.SpawnChance / 100f) && location.isTileLocationTotallyClearAndPlaceableIgnoreFloors(v))
            {
                yield return v;
            }
        }
    }

    /// <summary>
    /// Console command to list valid fruits for spawning.
    /// </summary>
    /// <param name="command">Name of command.</param>
    /// <param name="args">Arguments for command.</param>
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Console command format.")]
    private void ListFruits(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            this.Monitor.Log("World is not ready. Please load save first.", LogLevel.Info);
            return;
        }

        List<string> fruitNames = new();
        foreach (int objectID in this.GetTreeFruits())
        {
            SObject obj = new(objectID, 1);
            fruitNames.Add(obj.DisplayName);
        }

        this.Monitor.Log($"Possible fruits: {string.Join(", ", AtraUtils.ContextSort(fruitNames))}", LogLevel.Info);
    }

    /// <summary>
    /// Get data from assets, based on which mods are installed.
    /// </summary>
    /// <param name="datalocation">asset name.</param>
    /// <returns>List of data, split by commas.</returns>
    private List<string> GetData(string datalocation)
    {
        this.Helper.Content.InvalidateCache(datalocation);
        IDictionary<string, string> rawlist = this.Helper.Content.Load<Dictionary<string, string>>(datalocation, ContentSource.GameContent);
        List<string> datalist = new();

        foreach (string uniqueID in rawlist.Keys)
        {
            if (this.Helper.ModRegistry.IsLoaded(uniqueID))
            {
                datalist.AddRange(rawlist[uniqueID].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
            }
        }
        return datalist;
    }

    /// <summary>
    /// Generate list of tree fruits valid for spawning, based on user config/denylist/data in Data/fruitTrees.
    /// </summary>
    /// <returns>A list of tree fruit.</returns>
    private List<int> GetTreeFruits()
    {
        if (this.config.UseVanillaFruitOnly)
        {
            return this.VANILLA_FRUIT;
        }

        List<string> denylist = this.GetData(this.assetManager.DENYLIST_LOCATION);
        List<int> treeFruits = new();

        Dictionary<int, string> fruittrees = this.Helper.Content.Load<Dictionary<int, string>>("Data/fruitTrees", ContentSource.GameContent);
        string currentseason = Game1.currentSeason.ToLowerInvariant().Trim();
        foreach (string tree in fruittrees.Values)
        {
            string[] treedata = tree.Split('/', StringSplitOptions.TrimEntries);

            if ((this.config.SeasonalOnly == SeasonalBehavior.SeasonalOnly
                    || (this.config.SeasonalOnly == SeasonalBehavior.SeasonalExceptWinter && !currentseason.Contains("winter")))
                && !treedata[1].Contains(currentseason)
                && (!currentseason.Contains("summer") || !treedata[1].Contains("island")))
            {
                continue;
            }

            if (int.TryParse(treedata[2], out int objectIndex))
            {
                try
                {
                    SObject fruit = new(objectIndex, 1);
                    if ((!this.config.AllowAnyTreeProduct && fruit.Category != SObject.FruitsCategory)
                        || (this.config.EdiblesOnly && fruit.Edibility < 0)
                        || fruit.Price > this.config.PriceCap
                        || denylist.Contains(fruit.Name))
                    {
                        continue;
                    }
                    if (this.config.NoBananasBeforeShrine && fruit.Name.Equals("Banana"))
                    {
                        if (!Context.IsWorldReady && Game1.getLocationFromName("IslandEast") is IslandEast islandeast && !islandeast.bananaShrineComplete.Value)
                        {
                            continue;
                        }
                    }
                    treeFruits.Add(objectIndex);
                }
                catch (Exception ex)
                {
                    this.Monitor.Log($"Ran into issue looking up item {objectIndex}\n{ex}", LogLevel.Warn);
                }
            }
        }
        return treeFruits;
    }

    private void BellsAndWhistles(object? sender, OneSecondUpdateTickingEventArgs e)
    {
        if (Game1.currentLocation is Mine mine
            && this.SpawnedFruitToday
            && this.config.UseMineCave)
        { // The following code is copied out of the game and adds the bat sprites to the mines.
            if (Game1.random.NextDouble() < 0.12)
            {
                TemporaryAnimatedSprite redbat = new(
                    textureName: @"LooseSprites\Cursors",
                    sourceRect: new Rectangle(640, 1644, 16, 16),
                    animationInterval: 80f,
                    animationLength: 4,
                    numberOfLoops: 9999,
                    position: new Vector2(Game1.random.Next(mine.map.Layers[0].LayerWidth), Game1.random.Next(mine.map.Layers[0].LayerHeight)),
                    flicker: false,
                    flipped: false,
                    layerDepth: 1f,
                    alphaFade: 0f,
                    color: Color.Black,
                    scale: 4f,
                    scaleChange: 0f,
                    rotation: 0f,
                    rotationChange: 0f)
                {
                    xPeriodic = true,
                    xPeriodicLoopTime = 2000f,
                    xPeriodicRange = 64f,
                    motion = new Vector2(0f, -8f),
                };
                mine.TemporarySprites.Add(redbat);
                if (Game1.random.NextDouble() < 0.15)
                {
                    mine.localSound("batScreech");
                }
                for (int i = 0; i < 4; i++)
                {
                    DelayedAction.playSoundAfterDelay("batFlap", (320 * i) + 240);
                }
            }
            else if (Game1.random.NextDouble() < 0.24)
            {
                BatTemporarySprite batsprite = new(
                    new Vector2(
                        Game1.random.NextDouble() < 0.5 ? 0 : mine.map.DisplayWidth - 64,
                        mine.map.DisplayHeight - 64));
                mine.TemporarySprites.Add(batsprite);
            }
        }
    }
}
