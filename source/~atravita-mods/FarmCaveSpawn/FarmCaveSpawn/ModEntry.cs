/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

using AtraBase.Models.RentedArrayHelpers;
using AtraBase.Toolkit;
using AtraBase.Toolkit.Extensions;
using AtraBase.Toolkit.StringHandler;

using AtraCore.Framework.ItemManagement;

using AtraShared.ConstantsAndEnums;
using AtraShared.Integrations;
using AtraShared.MigrationManager;
using AtraShared.Utils;
using AtraShared.Utils.Extensions;
using AtraShared.Wrappers;

using Microsoft.Xna.Framework;

using StardewModdingAPI.Events;

using StardewValley.Locations;

using AtraUtils = AtraShared.Utils.Utils;
using XLocation = xTile.Dimensions.Location;

namespace FarmCaveSpawn;

/// <inheritdoc />
internal sealed class ModEntry : Mod
{
    /// <summary>
    /// Sublocation-parsing regex.
    /// </summary>
    private static readonly Regex Regex = new(
        // ":[(x1;y1);(x2;y2)]"
        pattern: @":\[\((?<x1>[0-9]+);(?<y1>[0-9]+)\);\((?<x2>[0-9]+);(?<y2>[0-9]+)\)\]$",
        options: RegexOptions.CultureInvariant | RegexOptions.Compiled,
        matchTimeout: TimeSpan.FromMilliseconds(250));

    private static bool ShouldResetFruitList = true;

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

    private StardewSeasons season = StardewSeasons.None;

    private MigrationManager? migrator;

    private ModConfig config = null!;

    /// <summary>
    /// Location to temporarily store the seeded random.
    /// </summary>
    private Random? random;

    /// <summary>
    /// Gets the seeded random for this mod.
    /// </summary>
    private Random Random
        => this.random ??= RandomUtils.GetSeededRandom(7, "atravita.FarmCaveSpawn.CaveRandom");

    /// <summary>
    /// Gets or sets a value indicating whether or not I've spawned fruit today.
    /// </summary>
    private bool SpawnedFruitToday { get; set; }

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        AssetManager.Initialize(helper.GameContent);

        this.config = AtraUtils.GetConfigOrDefault<ModConfig>(helper, this.Monitor);
        this.Monitor.Log($"Starting up: {this.ModManifest.UniqueID} - {typeof(ModEntry).Assembly.FullName}");

        helper.Events.GameLoop.DayStarted += this.SpawnFruit;
        helper.Events.GameLoop.GameLaunched += this.SetUpConfig;
        helper.Events.GameLoop.OneSecondUpdateTicking += this.BellsAndWhistles;
        helper.Events.GameLoop.SaveLoaded += this.SaveLoaded;

        helper.Events.Content.AssetRequested += static (_, e) => AssetManager.Load(e);

        // inventory watching
        InventoryWatcher.Initialize(this.ModManifest.UniqueID);
        helper.Events.GameLoop.SaveLoaded += (_, _) => InventoryWatcher.Load(helper.Multiplayer, helper.Data);
        helper.Events.Player.InventoryChanged += (_, e) => InventoryWatcher.Watch(e, helper.Multiplayer);
        helper.Events.Multiplayer.PeerConnected += (_, e) => InventoryWatcher.OnPeerConnected(e, helper.Multiplayer);
        helper.Events.Multiplayer.ModMessageReceived += static (_, e) => InventoryWatcher.OnModMessageRecieved(e);
        helper.Events.GameLoop.Saving += (_, _) => InventoryWatcher.Saving(helper.Data);

        helper.ConsoleCommands.Add(
            name: "av.fcs.list_fruits",
            documentation: I18n.ListFruits_Description(),
            callback: this.ListFruits);
    }

    /// <summary>
    /// Request the fruit list be reset the next time it's used.
    /// </summary>
    internal static void RequestFruitListReset() => ShouldResetFruitList = true;

    /// <summary>
    /// Remove the list TreeFruit when no longer necessary, delete the Random as well.
    /// </summary>
    private void Cleanup()
    {
        this.random = null;
    }

    /// <summary>
    /// Generates the GMCM for this mod by looking at the structure of the config class.
    /// </summary>
    /// <param name="sender">Unknown, expected by SMAPI.</param>
    /// <param name="e">event args.</param>
    private void SetUpConfig(object? sender, GameLaunchedEventArgs e)
    {
        GMCMHelper helper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, this.ModManifest);
        if (helper.TryGetAPI())
        {
            helper.Register(
                reset: () => this.config = new ModConfig(),
                save: () =>
                {
                    this.Helper.AsyncWriteConfig(this.Monitor, this.config);
                    ShouldResetFruitList = true;
                })
            .AddParagraph(I18n.Mod_Description)
            .GenerateDefaultGMCM(() => this.config);
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
            hasFCFbatcave = (farmcavechoice is not null) && (farmcavechoice.Contains("bat", StringComparison.OrdinalIgnoreCase) || farmcavechoice.Contains("fruit", StringComparison.OrdinalIgnoreCase));
            this.Monitor.DebugOnlyLog(hasFCFbatcave ? "FarmCaveFramework fruit bat cave detected." : "FarmCaveFramework fruit bat cave not detected.");
        }

        if (!this.config.EarlyFarmCave
            && (Game1.MasterPlayer.caveChoice?.Value is null || Game1.MasterPlayer.caveChoice.Value <= Farmer.caveNothing)
            && string.IsNullOrWhiteSpace(farmcavechoice))
        {
            this.Monitor.DebugOnlyLog("Demetrius cutscene not seen and config not set to early, skip spawning for today.");
            return false;
        }
        if (!this.config.IgnoreFarmCaveType && !this.config.EarlyFarmCave
            && (Game1.MasterPlayer.caveChoice?.Value is null || Game1.MasterPlayer.caveChoice.Value != Farmer.caveBats)
            && !hasFCFbatcave)
        {
            this.Monitor.DebugOnlyLog("Fruit bat cave not selected and config not set to ignore that, skip spawning for today.");
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
        this.SpawnedFruitToday = this.ShouldSpawnFruit();

        if (!this.SpawnedFruitToday || !Context.IsMainPlayer)
        {
            return;
        }

        int count = 0;

        StardewSeasons currentSeason = StardewSeasonsExtensions.TryParse(Game1.currentSeason, value: out StardewSeasons val, ignoreCase: true) ? val : StardewSeasons.All;
        if (ShouldResetFruitList || this.season != currentSeason)
        {
            this.TreeFruit = this.GetTreeFruits();
        }
        this.season = currentSeason;
        ShouldResetFruitList = false;

        if (Game1.getLocationFromName("FarmCave") is FarmCave farmcave)
        {
            this.Monitor.DebugOnlyLog($"Spawning in the farm cave");

            (Vector2[] tiles, int num) = farmcave.GetTiles();

            if (num > 0)
            {
                foreach (Vector2 tile in new Span<Vector2>(tiles).Shuffled(num, this.Random))
                {
                    if (this.CanSpawnFruitHere(farmcave, tile))
                    {
                        this.PlaceFruit(farmcave, tile);
                        if (++count >= this.config.MaxDailySpawns)
                        {
                            break;
                        }
                    }
                }

                ArrayPool<Vector2>.Shared.Return(tiles);
                farmcave.UpdateReadyFlag();
            }

            if (count >= this.config.MaxDailySpawns)
            {
                goto END;
            }
        }

        if (this.config.UseModCaves)
        {
            foreach (string location in this.GetData(AssetManager.ADDITIONAL_LOCATIONS_LOCATION))
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
                    MatchCollection matches = Regex.Matches(location);
                    if (matches.Count == 1)
                    {
                        Match match = matches[0];
                        parseloc = location[..^match.Value.Length];
                        locLimits.Update(match, namedOnly: true);
                        this.Monitor.DebugOnlyLog($"Found and parsed sublocation: {parseloc} + ({locLimits["x1"]};{locLimits["y1"]});({locLimits["x2"]};{locLimits["y2"]})");
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
                    this.Monitor.DebugOnlyLog($"Found {gameLocation.NameOrUniqueName}");

                    (Vector2[] tiles, int num) = gameLocation.GetTiles(xstart: locLimits["x1"], xend: locLimits["x2"], ystart: locLimits["y1"], yend: locLimits["y2"]);
                    if (num == 0)
                    {
                        continue;
                    }

                    foreach (Vector2 tile in new Span<Vector2>(tiles).Shuffled(num, this.Random))
                    {
                        if (this.CanSpawnFruitHere(gameLocation, tile))
                        {
                            this.PlaceFruit(gameLocation, tile);
                            if (++count >= this.config.MaxDailySpawns)
                            {
                                ArrayPool<Vector2>.Shared.Return(tiles);
                                goto END;
                            }
                        }
                    }

                    ArrayPool<Vector2>.Shared.Return(tiles);
                }
                else
                {
                    this.Monitor.Log(I18n.LocationMissing(loc: location), LogLevel.Trace);
                }
            }
        }

        if (this.config.UseMineCave && Game1.getLocationFromName("Mine") is Mine mine)
        {
            (Vector2[] tiles, int num) = mine.GetTiles(xstart: 11);
            if (num > 0)
            {
                foreach (Vector2 tile in new Span<Vector2>(tiles).Shuffled(num, this.Random))
                {
                    if (this.CanSpawnFruitHere(mine, tile))
                    {
                        this.PlaceFruit(mine, tile);
                        if (++count >= this.config.MaxDailySpawns)
                        {
                            ArrayPool<Vector2>.Shared.Return(tiles);
                            goto END;
                        }
                    }
                }
                ArrayPool<Vector2>.Shared.Return(tiles);
            }
        }

END:
        this.Cleanup();
        return;
    }

    /// <summary>
    /// Place a fruit on a specific tile.
    /// </summary>
    /// <param name="location">Map to place fruit on.</param>
    /// <param name="tile">Tile to place fruit on.</param>
    private void PlaceFruit(GameLocation location, Vector2 tile)
    {
        int fruitToPlace = Utility.GetRandom(
            this.TreeFruit.Count > 0 && this.Random.NextDouble() < (this.config.TreeFruitChance / 100f) ? this.TreeFruit : this.BASE_FRUIT,
            this.Random);

        if (!DataToItemMap.IsActuallyRing(fruitToPlace))
        {
            location.Objects[tile] = new SObject(fruitToPlace, 1) { IsSpawnedObject = true };
            this.Monitor.DebugOnlyLog($"Spawning item {fruitToPlace} at {location.Name}:{tile.X},{tile.Y}", LogLevel.Debug);
        }
    }

    [MethodImpl(TKConstants.Hot)]
    private bool CanSpawnFruitHere(GameLocation location, Vector2 tile)
        => this.Random.NextDouble() < this.config.SpawnChance / 100f
            && location.IsTileViewable(new XLocation((int)tile.X, (int)tile.Y), Game1.viewport)
            && location.isTileLocationTotallyClearAndPlaceableIgnoreFloors(tile);

    /// <summary>
    /// Console command to list valid fruits for spawning.
    /// </summary>
    /// <param name="command">Name of command.</param>
    /// <param name="args">Arguments for command.</param>
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
            if (Game1Wrappers.ObjectInfo.TryGetValue(objectID, out string? val))
            {
                ReadOnlySpan<char> name = val.GetNthChunk('/', SObject.objectInfoDisplayNameIndex);
                if (name.Length > 0)
                {
                    fruitNames.Add(name.ToString());
                }
            }
        }
        StringBuilder sb = StringBuilderCache.Acquire(fruitNames.Count * 6);
        sb.Append("Possible fruits: ");
        sb.AppendJoin(", ", AtraUtils.ContextSort(fruitNames));
        this.Monitor.Log(StringBuilderCache.GetStringAndRelease(sb), LogLevel.Info);
    }

    /// <summary>
    /// Get data from assets, based on which mods are installed.
    /// </summary>
    /// <param name="datalocation">asset name.</param>
    /// <returns>List of data, split by commas.</returns>
    private List<string> GetData(IAssetName datalocation)
    {
        IDictionary<string, string> rawlist = this.Helper.GameContent.Load<Dictionary<string, string>>(datalocation);
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
    /// Generate list of tree fruits valid for spawning, based on user config/deny list/data in Data/fruitTrees.
    /// </summary>
    /// <returns>A list of tree fruit.</returns>
    private List<int> GetTreeFruits()
    {
        this.Monitor.DebugOnlyLog("Generating tree fruit list");

        if (this.config.UseVanillaFruitOnly)
        {
            return this.VANILLA_FRUIT;
        }

        List<string> denylist = this.GetData(AssetManager.DENYLIST_LOCATION);
        List<int> treeFruits = new();

        Dictionary<int, string> fruittrees = this.Helper.GameContent.Load<Dictionary<int, string>>("Data/fruitTrees");
        ReadOnlySpan<char> currentseason = Game1.currentSeason.AsSpan().Trim();
        foreach ((int saplingIndex, string tree) in fruittrees)
        {
            if (this.config.ProgressionMode && !InventoryWatcher.HaveSeen(saplingIndex))
            {
                continue;
            }

            SpanSplit treedata = tree.SpanSplit('/', StringSplitOptions.TrimEntries, expectedCount: 3);

            if ((this.config.SeasonalOnly == SeasonalBehavior.SeasonalOnly || (this.config.SeasonalOnly == SeasonalBehavior.SeasonalExceptWinter && !Game1.IsWinter))
                && !treedata[1].Contains(currentseason, StringComparison.OrdinalIgnoreCase)
                && (!Game1.IsSummer || !treedata[1].Contains("island")))
            {
                continue;
            }

            // 73 is the golden walnut. Let's not let players have that, or 858's Qi gems.
            if (treedata.TryGetAtIndex(2, out SpanSplitEntry val) && int.TryParse(val, out int objectIndex) && objectIndex != 73 && objectIndex != 858)
            {
                try
                {
                    SpanSplit fruit = Game1Wrappers.ObjectInfo[objectIndex].SpanSplit('/', expectedCount: 5);
                    string fruitname = fruit[SObject.objectInfoNameIndex].ToString();
                    if ((this.config.AllowAnyTreeProduct || (fruit[SObject.objectInfoTypeIndex].SpanSplit().TryGetAtIndex(1, out SpanSplitEntry cat) && int.TryParse(cat, out int category) && category == SObject.FruitsCategory))
                        && (!this.config.EdiblesOnly || int.Parse(fruit[SObject.objectInfoEdibilityIndex]) >= 0)
                        && int.Parse(fruit[SObject.objectInfoPriceIndex]) <= this.config.PriceCap
                        && !denylist.Contains(fruitname)
                        && (!this.config.NoBananasBeforeShrine || !fruitname.Equals("Banana", StringComparison.OrdinalIgnoreCase)
                            || (Context.IsWorldReady && Game1.getLocationFromName("IslandEast") is IslandEast islandeast && islandeast.bananaShrineComplete.Value)))
                    {
                        treeFruits.Add(objectIndex);
                    }
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

    /// <summary>
    /// Raised when save is loaded.
    /// </summary>
    /// <param name="sender">Unknown, used by SMAPI.</param>
    /// <param name="e">Parameters.</param>
    /// <remarks>Used to load in this mod's data models.</remarks>
    private void SaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        if (Context.IsSplitScreen && Context.ScreenId != 0)
        {
            return;
        }
        this.migrator = new(this.ModManifest, this.Helper, this.Monitor);
        if (!this.migrator.CheckVersionInfo())
        {
            this.Helper.Events.GameLoop.Saved += this.WriteMigrationData;
        }
        else
        {
            this.migrator = null;
        }
    }

    /// <summary>
    /// Writes migration data then detaches the migrator.
    /// </summary>
    /// <param name="sender">Smapi thing.</param>
    /// <param name="e">Arguments for just-before-saving.</param>
    private void WriteMigrationData(object? sender, SavedEventArgs e)
    {
        if (this.migrator is not null)
        {
            this.migrator.SaveVersionInfo();
            this.migrator = null;
        }
        this.Helper.Events.GameLoop.Saved -= this.WriteMigrationData;
    }
}
