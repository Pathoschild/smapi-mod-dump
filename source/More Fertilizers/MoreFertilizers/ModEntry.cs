/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/MoreFertilizers
**
*************************************************/

using AtraShared.ConstantsAndEnums;
using AtraShared.Integrations;
using AtraShared.Integrations.Interfaces;
using AtraShared.MigrationManager;
using AtraShared.Utils;
using AtraShared.Utils.Extensions;
using HarmonyLib;
using MoreFertilizers.DataModels;
using MoreFertilizers.Framework;
using MoreFertilizers.HarmonyPatches;
using MoreFertilizers.HarmonyPatches.Acquisition;
using MoreFertilizers.HarmonyPatches.Compat;
using StardewModdingAPI.Events;
using StardewValley.TerrainFeatures;

namespace MoreFertilizers;

/// <inheritdoc />
internal class ModEntry : Mod
{
    private const string SAVESUFFIX = "_SavedObjectID";

    private static IJsonAssetsAPI? jsonAssets;

    private int countdown = 5;

    private MigrationManager? migrator;

    /// <summary>
    /// Gets the integer ID of the fruit tree fertilizer. -1 if not found/not loaded yet.
    /// </summary>
    internal static int FruitTreeFertilizerID => jsonAssets?.GetObjectId("Fruit Tree Fertilizer") ?? -1;

    /// <summary>
    /// Gets the integer ID of the deluxe fruit tree fertilizer. -1 if not found/not loaded yet.
    /// </summary>
    internal static int DeluxeFruitTreeFertilizerID => jsonAssets?.GetObjectId("Deluxe Fruit Tree Fertilizer") ?? -1;

    /// <summary>
    /// Gets the integer ID of the fish food. -1 if not found/not loaded yet.
    /// </summary>
    internal static int FishFoodID => jsonAssets?.GetObjectId("Fish Food Fertilizer") ?? -1;

    /// <summary>
    /// Gets the integer ID of the deluxe fish food. -1 if not found/not loaded yet.
    /// </summary>
    internal static int DeluxeFishFoodID => jsonAssets?.GetObjectId("Deluxe Fish Food Fertilizer") ?? -1;

    /// <summary>
    /// Gets the integer ID of the domesticated fish food. -1 if not found/not loaded yet.
    /// </summary>
    internal static int DomesticatedFishFoodID => jsonAssets?.GetObjectId("Domesticated Fish Food Fertilizer") ?? -1;

    /// <summary>
    /// Gets the integer ID of the paddy crop fertilizer. -1 if not found/not loaded yet.
    /// </summary>
    internal static int PaddyCropFertilizerID => jsonAssets?.GetObjectId("Waterlogged Fertilizer") ?? -1;

    /// <summary>
    /// Gets the interger ID of the lucky fertiizer. -1 if not found/not loaded yet.
    /// </summary>
    internal static int LuckyFertilizerID => jsonAssets?.GetObjectId("Maebys Good-Luck Fertilizer") ?? -1;

    /// <summary>
    /// Gets the integer ID of the bountiful fertilizer. -1 if not found/not loaded yet.
    /// </summary>
    internal static int BountifulFertilizerID => jsonAssets?.GetObjectId("Bountiful Fertilizer") ?? -1;

    /// <summary>
    /// Gets the integer ID of Joja's fertilizer. -1 if not found/not loaded yet.
    /// </summary>
    internal static int JojaFertilizerID => jsonAssets?.GetObjectId("Joja Fertilizer - More Fertilizers") ?? -1;

    /// <summary>
    /// Gets the integer ID of the Deluxe Joja's fertilizer. -1 if not found/not loaded yet.
    /// </summary>
    internal static int DeluxeJojaFertilizerID => jsonAssets?.GetObjectId("Deluxe Joja Fertilizer - More Fertilizers") ?? -1;

    /// <summary>
    /// Gets the integer ID of the organic fertilzer. -1 if not found/not loaded yet.
    /// </summary>
    internal static int OrganicFertilizerID => jsonAssets?.GetObjectId("Organic Fertilizer - More Fertilizers") ?? -1;

    /// <summary>
    /// Gets a list of fertilizer IDs for fertilizers that are meant to be planted into HoeDirt.
    /// </summary>
    /// <remarks>Will be stored in the <see cref="HoeDirt.fertilizer.Value"/> field.</remarks>
    internal static List<int> PlantableFertilizerIDs { get; } = new List<int>();

    /// <summary>
    /// Gets a list of fertilizer IDs for fertilizers that are placed in other means (not into HoeDirt).
    /// </summary>
    /// <remarks>Handled by <see cref="SpecialFertilizerApplication" /> and typically stored in <see cref="ModDataDictionary"/>.</remarks>
    internal static List<int> SpecialFertilizerIDs { get; } = new List<int>();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <summary>
    /// Gets the logger for this mod.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; }

    /// <summary>
    /// Gets the multiplayer helper for this mod.
    /// </summary>
    internal static IMultiplayerHelper MultiplayerHelper { get; private set; }

    /// <summary>
    /// Gets this mod's uniqueID.
    /// </summary>
    internal static string UNIQUEID { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        I18n.Init(this.Helper.Translation);
        MultiplayerHelper = this.Helper.Multiplayer;
        ModMonitor = this.Monitor;
        UNIQUEID = this.ModManifest.UniqueID;

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        helper.Events.GameLoop.Saved += this.OnSaved;
        helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;

        helper.Events.Multiplayer.ModMessageReceived += this.Multiplayer_ModMessageReceived;
        helper.Events.Multiplayer.PeerConnected += this.Multiplayer_PeerConnected;

        helper.Events.Player.Warped += this.OnPlayerWarp;

        helper.Events.GameLoop.DayEnding += this.OnDayEnd;
        helper.Events.Input.ButtonPressed += this.OnButtonPressed;

#if DEBUG
        helper.Events.Input.ButtonPressed += this.DebugOutput;
#endif
    }

    /// <inheritdoc />
    [UsedImplicitly]
    public override object GetApi() => new CanPlaceHandler();

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        => SpecialFertilizerApplication.ApplyFertilizer(e, this.Helper.Input);

#if DEBUG
    private void DebugOutput(object? sender, ButtonPressedEventArgs e)
    {
        if (Context.IsWorldReady && e.Button.IsUseToolButton())
        {
            if (Game1.currentLocation.terrainFeatures.TryGetValue(e.Cursor.Tile, out TerrainFeature? terrainFeature))
            {
                if (terrainFeature is HoeDirt dirt)
                {
                    this.Monitor.Log($"{e.Cursor.Tile} has fertilizer {dirt.fertilizer.Value}, state {dirt.state.Value} and {dirt.nearWaterForPaddy.Value}", LogLevel.Info);
                }
                else if (terrainFeature is FruitTree tree)
                {
                    this.Monitor.Log($"{e.Cursor.Tile} is on {tree.treeType.Value} with {tree.daysUntilMature.Value}.", LogLevel.Info);
                }
            }
            if (Game1.currentLocation?.modData?.GetInt(CanPlaceHandler.FishFood) is > 0)
            {
                this.Monitor.Log($"FishFood: Remaining time: {Game1.currentLocation?.modData?.GetInt(CanPlaceHandler.FishFood)}", LogLevel.Info);
            }
        }
    }
#endif

    [EventPriority(EventPriority.High)]
    private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
    {
        PlantableFertilizerIDs.Clear();
        SpecialFertilizerIDs.Clear();
    }

    /// <summary>
    /// When safely saved, also save the ids for each fertilizer.
    /// </summary>
    /// <param name="sender">SMAPI.</param>
    /// <param name="e">Event arguments.</param>
    private void OnSaved(object? sender, SavedEventArgs e)
    {
        // TODO: This should be doable with expression trees in a less dumb way.
        MoreFertilizerIDs fertilizerIDs = new();
        fertilizerIDs.FruitTreeFertilizerID = FruitTreeFertilizerID;
        fertilizerIDs.DeluxeFruitTreeFertilizerID = DeluxeFruitTreeFertilizerID;
        fertilizerIDs.FishFoodID = FishFoodID;
        fertilizerIDs.DeluxeFishFoodID = DeluxeFishFoodID;
        fertilizerIDs.DomesticatedFishFoodID = DomesticatedFishFoodID;
        fertilizerIDs.PaddyFertilizerID = PaddyCropFertilizerID;
        fertilizerIDs.LuckyFertilizerID = LuckyFertilizerID;
        fertilizerIDs.BountifulFertilizerID = BountifulFertilizerID;
        fertilizerIDs.JojaFertilizerID = JojaFertilizerID;
        fertilizerIDs.DeluxeJojaFertilizerID = DeluxeJojaFertilizerID;
        fertilizerIDs.OrganicFertilizerID = OrganicFertilizerID;
        this.Helper.Data.WriteGlobalData(Constants.SaveFolderName + SAVESUFFIX, fertilizerIDs);
    }

    /// <summary>
    /// Applies the patches for this mod.
    /// </summary>
    /// <param name="harmony">This mod's harmony instance.</param>
    private void ApplyPatches(Harmony harmony)
    {
        try
        {
            harmony.PatchAll();
            if (this.Helper.ModRegistry.IsLoaded("spacechase0.MultiFertilizer"))
            {
                this.Monitor.Log("Found MultiFertilizer, applying compat patches", LogLevel.Info);

                HoeDirtPatcher.ApplyPatches(harmony);
                MultiFertilizerDrawTranspiler.ApplyPatches(harmony);
            }
            else
            {
                HoeDirtDrawTranspiler.ApplyPatches(harmony);
            }
            if (!this.Helper.ModRegistry.IsLoaded("Digus.ProducerFrameworkMod"))
            {
                PerformObjectDropInTranspiler.ApplyPatches(harmony);
            }
        }
        catch (Exception ex)
        {
            ModMonitor.Log(string.Format(ErrorMessageConsts.HARMONYCRASH, ex), LogLevel.Error);
        }
        harmony.Snitch(this.Monitor, harmony.Id, transpilersOnly: true);
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.ApplyPatches(new Harmony(this.ModManifest.UniqueID));

        {
            IntegrationHelper helper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, LogLevel.Warn);
            if (helper.TryGetAPI("spacechase0.JsonAssets", "1.10.3", out jsonAssets))
            {
                jsonAssets.LoadAssets(Path.Combine(this.Helper.DirectoryPath, "assets", "json-assets"), this.Helper.Translation);
                jsonAssets.IdsAssigned += this.JsonAssets_IdsAssigned;
                jsonAssets.IdsFixed += this.JsonAssets_IdsFixed;
                this.Monitor.Log("Loaded packs!");
            }
            else
            {
                this.Monitor.Log("Packs could not be loaded! This mod will probably not function.", LogLevel.Error);
            }
        }

        {
            IntegrationHelper helper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry);
            if (helper.TryGetAPI("TehPers.FishingOverhaul", "3.2.7", out ISimplifiedFishingApi? fishingAPI))
            {
                fishingAPI.ModifyChanceForFish((Farmer who, double chance) =>
                {
                    if (who.currentLocation?.modData?.GetBool(CanPlaceHandler.FishFood) == true)
                    {
                        return Math.Sqrt(Math.Clamp(chance, 0, 1));
                    }
                    return chance;
                });
            }
        }

        this.Helper.Events.GameLoop.UpdateTicked += this.FiveTicksPostGameLaunched;
    }

    private void FiveTicksPostGameLaunched(object? sender, UpdateTickedEventArgs e)
    {
        if (--this.countdown < 0)
        {
            this.Helper.Content.AssetEditors.Add(AssetEditor.Instance);
            this.Helper.Events.GameLoop.UpdateTicked -= this.FiveTicksPostGameLaunched;
        }
    }

    private void OnDayEnd(object? sender, DayEndingEventArgs e)
    {
        JojaSample.Reset();
        FishFoodHandler.DecrementAndSave(this.Helper.Data, this.Helper.Multiplayer);
    }

    /************
     * REGION JA
     * *********/

    private void JsonAssets_IdsAssigned(object? sender, EventArgs e) => this.FixIDs();

    private void JsonAssets_IdsFixed(object? sender, EventArgs e) => this.FixIDs();

    private void FixIDs()
    {
        PlantableFertilizerIDs.Clear();
        SpecialFertilizerIDs.Clear();

        if (FruitTreeFertilizerID != -1)
        {
            SpecialFertilizerIDs.Add(FruitTreeFertilizerID);
        }

        if (DeluxeFruitTreeFertilizerID != -1)
        {
            SpecialFertilizerIDs.Add(DeluxeFruitTreeFertilizerID);
        }

        if (FishFoodID != -1)
        {
            SpecialFertilizerIDs.Add(FishFoodID);
        }

        if (DeluxeFishFoodID != -1)
        {
            SpecialFertilizerIDs.Add(DeluxeFishFoodID);
        }

        if (DomesticatedFishFoodID != -1)
        {
            SpecialFertilizerIDs.Add(DomesticatedFishFoodID);
        }

        // Plantable ones begin here.
        if (PaddyCropFertilizerID != -1)
        {
            PlantableFertilizerIDs.Add(PaddyCropFertilizerID);
        }

        if (LuckyFertilizerID != -1)
        {
            PlantableFertilizerIDs.Add(LuckyFertilizerID);
        }

        if (BountifulFertilizerID != -1)
        {
            PlantableFertilizerIDs.Add(BountifulFertilizerID);
        }

        if (JojaFertilizerID != -1)
        {
            PlantableFertilizerIDs.Add(JojaFertilizerID);
        }

        if (DeluxeJojaFertilizerID != -1)
        {
            PlantableFertilizerIDs.Add(DeluxeJojaFertilizerID);
        }

        if (OrganicFertilizerID != -1)
        {
            PlantableFertilizerIDs.Add(OrganicFertilizerID);
        }

        if (SpecialFertilizerIDs.Count <= 0 && PlantableFertilizerIDs.Count <= 0)
        { // I have found no valid fertilizers. Just return.
            return;
        }

        if (this.Helper.Data.ReadGlobalData<MoreFertilizerIDs>(Constants.SaveFolderName + SAVESUFFIX) is not MoreFertilizerIDs storedIDCls)
        {
            ModMonitor.Log("No need to fix IDs, not installed before.");
            return;
        }

        Dictionary<int, int> idMapping = new();

        // Have to update the planted ones.
        if (LuckyFertilizerID != -1
            && storedIDCls.LuckyFertilizerID != -1
            && storedIDCls.LuckyFertilizerID != LuckyFertilizerID)
        {
            idMapping.Add(storedIDCls.LuckyFertilizerID, LuckyFertilizerID);
        }

        if (PaddyCropFertilizerID != -1
            && storedIDCls.PaddyFertilizerID != -1
            && storedIDCls.PaddyFertilizerID != PaddyCropFertilizerID)
        {
            idMapping.Add(storedIDCls.PaddyFertilizerID, PaddyCropFertilizerID);
        }

        if (BountifulFertilizerID != -1
            && storedIDCls.BountifulFertilizerID != -1
            && storedIDCls.BountifulFertilizerID != BountifulFertilizerID)
        {
            idMapping.Add(storedIDCls.BountifulFertilizerID, BountifulFertilizerID);
        }

        if (JojaFertilizerID != -1
            && storedIDCls.JojaFertilizerID != -1
            && storedIDCls.JojaFertilizerID != JojaFertilizerID)
        {
            idMapping.Add(storedIDCls.JojaFertilizerID, JojaFertilizerID);
        }

        if (DeluxeJojaFertilizerID != -1
            && storedIDCls.DeluxeJojaFertilizerID != -1
            && storedIDCls.DeluxeJojaFertilizerID != DeluxeJojaFertilizerID)
        {
            idMapping.Add(storedIDCls.DeluxeJojaFertilizerID, DeluxeJojaFertilizerID);
        }

        if (OrganicFertilizerID != -1
            && storedIDCls.OrganicFertilizerID != -1
            && storedIDCls.OrganicFertilizerID != OrganicFertilizerID)
        {
            idMapping.Add(storedIDCls.OrganicFertilizerID, OrganicFertilizerID);
        }

        if (idMapping.Count <= 0 )
        {
            ModMonitor.Log("No need to fix IDs, nothing has changed.");
            return;
        }

        Utility.ForAllLocations((GameLocation loc) =>
        {
            foreach (TerrainFeature terrain in loc.terrainFeatures.Values)
            {
                if (terrain is HoeDirt dirt && dirt.fertilizer.Value != 0)
                {
                    if (idMapping.TryGetValue(dirt.fertilizer.Value, out int newval))
                    {
                        dirt.fertilizer.Value = newval;
                    }
                }
            }
        });

        ModMonitor.Log($"Fixed IDs! {string.Join(", ", idMapping.Select((kvp) => $"{kvp.Key}=>{kvp.Value}"))}");
    }

    /***********
     * REGION MIGRATION
     * **********/
    [EventPriority(EventPriority.Low)]
    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        JojaSample.Reset();

        IntegrationHelper pfmHelper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, LogLevel.Trace);
        if (pfmHelper.TryGetAPI("Digus.ProducerFrameworkMod", "1.7.4", out IProducerFrameworkModAPI? pfmAPI))
        {
            pfmAPI.AddContentPack(Path.Combine(this.Helper.DirectoryPath, "assets", "pfm-assets"));
        }

        MultiplayerHelpers.AssertMultiplayerVersions(this.Helper.Multiplayer, this.ModManifest, this.Monitor, this.Helper.Translation);

        this.migrator = new(this.ModManifest, this.Helper, this.Monitor);
        this.migrator.ReadVersionInfo();
        this.Helper.Events.GameLoop.Saved += this.WriteMigrationData;

        this.FixIDs();

        if (Context.IsMainPlayer)
        {
            FishFoodHandler.LoadHandler(this.Helper.Data, this.Helper.Multiplayer);
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

    /*******************
     * REGION MINESHAFT AND MULTIPLAYER
     ********************/

    private void OnPlayerWarp(object? sender, WarpedEventArgs e)
    {
        JojaSample.JojaSampleEvent(e);
        FishFoodHandler.HandleWarp(e);
    }

    private void Multiplayer_PeerConnected(object? sender, PeerConnectedEventArgs e)
    {
        if (e.Peer.ScreenID == 0 && Context.IsWorldReady && Context.IsMainPlayer)
        {
            this.Helper.Multiplayer.SendMessage(FishFoodHandler.UnsavedLocHandler, "DATAPACKAGE", new[] { UNIQUEID }, new[] { e.Peer.PlayerID });
        }
    }

    private void Multiplayer_ModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != this.ModManifest.UniqueID)
        {
            return;
        }

        if (e.Type is "DATAPACKAGE")
        {
            FishFoodHandler.RecieveHandler(e);
        }
    }
}