/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/MoreFertilizers
**
*************************************************/

using System.Reflection;
using AtraShared.ConstantsAndEnums;
using AtraShared.Integrations;
using AtraShared.Integrations.Interfaces;
using AtraShared.MigrationManager;
using AtraShared.Utils;
using AtraShared.Utils.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using MoreFertilizers.DataModels;
using MoreFertilizers.Framework;
using MoreFertilizers.HarmonyPatches;
using MoreFertilizers.HarmonyPatches.Acquisition;
using MoreFertilizers.HarmonyPatches.Compat;
using MoreFertilizers.HarmonyPatches.FishFood;
using MoreFertilizers.HarmonyPatches.FruitTreePatches;
using StardewModdingAPI.Events;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

using AtraUtils = AtraShared.Utils.Utils;

namespace MoreFertilizers;

/// <inheritdoc />
internal class ModEntry : Mod
{
    private const string SavedIDKey = "MFSavedObjectID";

    private static IJsonAssetsAPI? jsonAssets;

    private static MoreFertilizerIDs? storedIDs;

    private MigrationManager? migrator;

#pragma warning disable SA1204 // Static elements should appear before instance elements. Keep backing fields near properties.
#pragma warning disable SA1201 // Elements should appear in the correct order
    private static int fruitTreeFertilizerID = -1;

    /// <summary>
    /// Gets the integer ID of the fruit tree fertilizer. -1 if not found/not loaded yet.
    /// </summary>
    internal static int FruitTreeFertilizerID
    {
        get
        {
            if (fruitTreeFertilizerID != -1)
            {
                return fruitTreeFertilizerID;
            }
            else
            {
                fruitTreeFertilizerID = jsonAssets?.GetObjectId("Fruit Tree Fertilizer") ?? -1;
                return fruitTreeFertilizerID;
            }
        }
    }

    private static int deluxeFruitTreeFertilizerID = -1;

    /// <summary>
    /// Gets the integer ID of the deluxe fruit tree fertilizer. -1 if not found/not loaded yet.
    /// </summary>
    internal static int DeluxeFruitTreeFertilizerID
    {
        get
        {
            if (deluxeFruitTreeFertilizerID != -1)
            {
                return deluxeFruitTreeFertilizerID;
            }
            else
            {
                deluxeFruitTreeFertilizerID = jsonAssets?.GetObjectId("Deluxe Fruit Tree Fertilizer") ?? -1;
                return deluxeFruitTreeFertilizerID;
            }
        }
    }

    private static int fishfoodID = -1;

    /// <summary>
    /// Gets the integer ID of the fish food. -1 if not found/not loaded yet.
    /// </summary>
    internal static int FishFoodID
    {
        get
        {
            if (fishfoodID != -1)
            {
                return fishfoodID;
            }
            else
            {
                fishfoodID = jsonAssets?.GetObjectId("Fish Food Fertilizer") ?? -1;
                return fishfoodID;
            }
        }
    }

    private static int deluxeFishFoodID = -1;

    /// <summary>
    /// Gets the integer ID of the deluxe fish food. -1 if not found/not loaded yet.
    /// </summary>
    internal static int DeluxeFishFoodID
    {
        get
        {
            if (deluxeFishFoodID != -1)
            {
                return deluxeFishFoodID;
            }
            else
            {
                deluxeFishFoodID = jsonAssets?.GetObjectId("Deluxe Fish Food Fertilizer") ?? -1;
                return deluxeFishFoodID;
            }
        }
    }

    private static int domesticatedFishFoodID = -1;

    /// <summary>
    /// Gets the integer ID of the domesticated fish food. -1 if not found/not loaded yet.
    /// </summary>
    internal static int DomesticatedFishFoodID
    {
        get
        {
            if (domesticatedFishFoodID != -1)
            {
                return domesticatedFishFoodID;
            }
            else
            {
                domesticatedFishFoodID = jsonAssets?.GetObjectId("Domesticated Fish Food Fertilizer") ?? -1;
                return domesticatedFishFoodID;
            }
        }
    }

    private static int paddyCropFertilizerID = -1;

    /// <summary>
    /// Gets the integer ID of the paddy crop fertilizer. -1 if not found/not loaded yet.
    /// </summary>
    internal static int PaddyCropFertilizerID
    {
        get
        {
            if (paddyCropFertilizerID != -1)
            {
                return paddyCropFertilizerID;
            }
            else
            {
                paddyCropFertilizerID = jsonAssets?.GetObjectId("Waterlogged Fertilizer") ?? -1;
                return paddyCropFertilizerID;
            }
        }
    }

    private static int luckyFertilizerID = -1;

    /// <summary>
    /// Gets the interger ID of the lucky fertiizer. -1 if not found/not loaded yet.
    /// </summary>
    internal static int LuckyFertilizerID
    {
        get
        {
            if (luckyFertilizerID != -1)
            {
                return luckyFertilizerID;
            }
            else
            {
                luckyFertilizerID = jsonAssets?.GetObjectId("Maebys Good-Luck Fertilizer") ?? -1;
                return luckyFertilizerID;
            }
        }
    }

    private static int bountifulFertilizerID = -1;

    /// <summary>
    /// Gets the integer ID of the bountiful fertilizer. -1 if not found/not loaded yet.
    /// </summary>
    internal static int BountifulFertilizerID
    {
        get
        {
            if (bountifulFertilizerID != -1)
            {
                return bountifulFertilizerID;
            }
            else
            {
                bountifulFertilizerID = jsonAssets?.GetObjectId("Bountiful Fertilizer") ?? -1;
                return bountifulFertilizerID;
            }
        }
    }

    private static int jojaFertilizerID = -1;

    /// <summary>
    /// Gets the integer ID of Joja's fertilizer. -1 if not found/not loaded yet.
    /// </summary>
    internal static int JojaFertilizerID
    {
        get
        {
            if (jojaFertilizerID != -1)
            {
                return jojaFertilizerID;
            }
            else
            {
                jojaFertilizerID = jsonAssets?.GetObjectId("Joja Fertilizer - More Fertilizers") ?? -1;
                return jojaFertilizerID;
            }
        }
    }

    private static int deluxeJojaFertilizerID = -1;

    /// <summary>
    /// Gets the integer ID of the Deluxe Joja's fertilizer. -1 if not found/not loaded yet.
    /// </summary>
    internal static int DeluxeJojaFertilizerID
    {
        get
        {
            if (deluxeJojaFertilizerID != -1)
            {
                return deluxeJojaFertilizerID;
            }
            else
            {
                deluxeJojaFertilizerID = jsonAssets?.GetObjectId("Deluxe Joja Fertilizer - More Fertilizers") ?? -1;
                return deluxeJojaFertilizerID;
            }
        }
    }

    private static int organicFertilizerID = -1;

    /// <summary>
    /// Gets the integer ID of the organic fertilzer. -1 if not found/not loaded yet.
    /// </summary>
    internal static int OrganicFertilizerID
    {
        get
        {
            if (organicFertilizerID != -1)
            {
                return organicFertilizerID;
            }
            else
            {
                organicFertilizerID = jsonAssets?.GetObjectId("Organic Fertilizer - More Fertilizers") ?? -1;
                return organicFertilizerID;
            }
        }
    }
#pragma warning restore SA1201 // Elements should appear in the correct order
#pragma warning restore SA1204 // Static elements should appear before instance elements

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

    /// <summary>
    /// Gets the config instance for this mod.
    /// </summary>
    internal static ModConfig Config { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        I18n.Init(this.Helper.Translation);
        MultiplayerHelper = this.Helper.Multiplayer;
        ModMonitor = this.Monitor;
        UNIQUEID = this.ModManifest.UniqueID;
        Config = AtraUtils.GetConfigOrDefault<ModConfig>(helper, this.Monitor);

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        helper.Events.GameLoop.Saving += this.OnSaving;
        helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;

        helper.Events.Multiplayer.ModMessageReceived += this.Multiplayer_ModMessageReceived;
        helper.Events.Multiplayer.PeerConnected += this.Multiplayer_PeerConnected;

        helper.Events.Player.Warped += this.OnPlayerWarp;

        helper.Events.GameLoop.DayEnding += this.OnDayEnd;
        helper.Events.Input.ButtonPressed += this.OnButtonPressed;

        helper.Events.Content.AssetRequested += this.OnAssetRequested;

#if DEBUG
        helper.Events.Input.ButtonPressed += this.DebugOutput;
#endif
    }

    /// <inheritdoc />
    [UsedImplicitly]
    public override object GetApi() => new CanPlaceHandler();

    private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        => AssetEditor.Edit(e);

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
                else if (terrainFeature is FruitTree fruitTree)
                {
                    this.Monitor.Log($"{e.Cursor.Tile} is on {fruitTree.treeType.Value} with {fruitTree.daysUntilMature.Value}.", LogLevel.Info);
                }
                else if (terrainFeature is Tree tree)
                {
                    this.Monitor.Log($"{e.Cursor.Tile} {(tree?.modData?.GetBool(CanPlaceHandler.TreeFertilizer) == true ? "had" : "did not have" )} tree fertilizer.", LogLevel.Info);
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
    private void OnSaving(object? sender, SavingEventArgs e)
    {
        if (!Context.IsMainPlayer)
        {
            return;
        }
        // TODO: This should be doable with expression trees in a less dumb way.
        if (storedIDs is null)
        {
            storedIDs = new();
            storedIDs.FruitTreeFertilizerID = FruitTreeFertilizerID;
            storedIDs.DeluxeFruitTreeFertilizerID = DeluxeFruitTreeFertilizerID;
            storedIDs.FishFoodID = FishFoodID;
            storedIDs.DeluxeFishFoodID = DeluxeFishFoodID;
            storedIDs.DomesticatedFishFoodID = DomesticatedFishFoodID;
            storedIDs.PaddyFertilizerID = PaddyCropFertilizerID;
            storedIDs.LuckyFertilizerID = LuckyFertilizerID;
            storedIDs.BountifulFertilizerID = BountifulFertilizerID;
            storedIDs.JojaFertilizerID = JojaFertilizerID;
            storedIDs.DeluxeJojaFertilizerID = DeluxeJojaFertilizerID;
            storedIDs.OrganicFertilizerID = OrganicFertilizerID;
        }
        this.Helper.Data.WriteSaveData(SavedIDKey, storedIDs);
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

            if (this.Helper.ModRegistry.Get("spacechase0.MultiFertilizer") is IModInfo info
                && info.Manifest.Version.IsOlderThan("1.0.6"))
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
            else if (this.Helper.ModRegistry.Get("Digus.PFMAutomate") is IModInfo pfmAutomate
                && pfmAutomate.Manifest.Version.IsNewerThan("1.4.1"))
            {
                this.Monitor.Log("Found PFMAutomate, transpiling PFM to support that.", LogLevel.Info);
                PFMAutomateTranspilers.ApplyPatches(harmony);
            }

            if (this.Helper.ModRegistry.Get("spacechase0.DynamicGameAssets") is IModInfo dga
                && dga.Manifest.Version.IsNewerThan("1.4.1"))
            {
                this.Monitor.Log("Found Dynamic Game Assets, applying compat patches", LogLevel.Info);
                FruitTreeDayUpdateTranspiler.ApplyDGAPatch(harmony);
                FruitTreeDrawTranspiler.ApplyDGAPatch(harmony);
                CropHarvestTranspiler.ApplyDGAPatch(harmony);
                SObjectPatches.ApplyDGAPatch(harmony);
            }

            if (this.Helper.ModRegistry.Get("Cherry.MultiYieldCrops") is IModInfo multiYield
                && multiYield.Manifest.Version.IsNewerThan("1.0.1"))
            {
                this.Monitor.Log("Found MultiYieldCrops, applying compat patches", LogLevel.Info);
                MultiYieldCropsCompat.ApplyPatches(harmony);
            }

            if (this.Helper.ModRegistry.Get("Pathoschild.Automate") is IModInfo automate
                && automate.Manifest.Version.IsNewerThan("1.25.2"))
            {
                this.Monitor.Log("Found Automate, applying compat patches", LogLevel.Info);
                AutomateTranspiler.ApplyPatches(harmony);
            }

            if (this.Helper.ModRegistry.Get("Satozaki.MillerTime") is IModInfo millerTime
                && millerTime.Manifest.Version.IsNewerThan("0.99.0"))
            {
                this.Monitor.Log("Found Miller Time, applying compat patches", LogLevel.Info);
                MillerTimeDayUpdateTranspiler.ApplyPatches(harmony);
            }
        }
        catch (Exception ex)
        {
            ModMonitor.Log(string.Format(ErrorMessageConsts.HARMONYCRASH, ex), LogLevel.Error);
        }
        harmony.Snitch(this.Monitor, harmony.Id, transpilersOnly: true);
    }

    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "Reviewed.")]
    private static ModConfig GetConfig() => Config;

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.ApplyPatches(new Harmony(this.ModManifest.UniqueID));
        {
            GMCMHelper helper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, this.ModManifest);
            if (helper.TryGetAPI())
            {
                helper.TryGetOptionsAPI();

                helper.Register(
                    reset: static () => Config = new(),
                    save: () => this.Helper.WriteConfig(Config))
                    .AddParagraph(I18n.Mod_Description);
                foreach (PropertyInfo property in typeof(ModConfig).GetProperties())
                {
                    if (property.PropertyType == typeof(bool))
                    {
                        helper.AddBoolOption(property, GetConfig);
                    }
                    else if (property.PropertyType == typeof(Color))
                    {
                        helper.AddColorPicker(property, GetConfig, defaultColor: new(147, 112, 219, 155));
                    }
                }
            }
        }
        {
            IntegrationHelper helper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, LogLevel.Warn);
            if (helper.TryGetAPI("spacechase0.JsonAssets", "1.10.3", out jsonAssets))
            {
                jsonAssets.LoadAssets(Path.Combine(this.Helper.DirectoryPath, "assets", "json-assets"), this.Helper.Translation);
                jsonAssets.IdsFixed += this.JsonAssets_IdsFixed;
            }
            else
            {
                this.Monitor.Log("Packs could not be loaded! This mod will probably not function.", LogLevel.Error);
            }
        }
        {
            IntegrationHelper helper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, LogLevel.Trace);
            if (helper.TryGetAPI("TehPers.FishingOverhaul", "3.2.7", out ISimplifiedFishingApi? fishingAPI))
            {
                fishingAPI.ModifyChanceForFish(static (Farmer who, double chance) =>
                {
                    if (who.currentLocation is not null)
                    {
                        return GetFishTranspiler.AlterFishChance(chance, who.currentLocation);
                    }
                    return chance;
                });
            }
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

        if (!Context.IsMainPlayer)
        {
            return;
        }

        if (storedIDs is null)
        {
            if (this.Helper.Data.ReadSaveData<MoreFertilizerIDs>(SavedIDKey) is not MoreFertilizerIDs storedIDCls)
            {
                ModMonitor.Log("No need to fix IDs, not installed before.");
                return;
            }
            storedIDs = storedIDCls;
        }

        Dictionary<int, int> idMapping = new();

        // Have to update the planted ones.
        if (LuckyFertilizerID != -1
            && storedIDs.LuckyFertilizerID != -1
            && storedIDs.LuckyFertilizerID != LuckyFertilizerID)
        {
            idMapping.Add(storedIDs.LuckyFertilizerID, LuckyFertilizerID);
            storedIDs.LuckyFertilizerID = LuckyFertilizerID;
        }

        if (PaddyCropFertilizerID != -1
            && storedIDs.PaddyFertilizerID != -1
            && storedIDs.PaddyFertilizerID != PaddyCropFertilizerID)
        {
            idMapping.Add(storedIDs.PaddyFertilizerID, PaddyCropFertilizerID);
            storedIDs.PaddyFertilizerID = PaddyCropFertilizerID;
        }

        if (BountifulFertilizerID != -1
            && storedIDs.BountifulFertilizerID != -1
            && storedIDs.BountifulFertilizerID != BountifulFertilizerID)
        {
            idMapping.Add(storedIDs.BountifulFertilizerID, BountifulFertilizerID);
            storedIDs.BountifulFertilizerID = BountifulFertilizerID;
        }

        if (JojaFertilizerID != -1
            && storedIDs.JojaFertilizerID != -1
            && storedIDs.JojaFertilizerID != JojaFertilizerID)
        {
            idMapping.Add(storedIDs.JojaFertilizerID, JojaFertilizerID);
            storedIDs.JojaFertilizerID = JojaFertilizerID;
        }

        if (DeluxeJojaFertilizerID != -1
            && storedIDs.DeluxeJojaFertilizerID != -1
            && storedIDs.DeluxeJojaFertilizerID != DeluxeJojaFertilizerID)
        {
            idMapping.Add(storedIDs.DeluxeJojaFertilizerID, DeluxeJojaFertilizerID);
            storedIDs.DeluxeJojaFertilizerID = DeluxeJojaFertilizerID;
        }

        if (OrganicFertilizerID != -1
            && storedIDs.OrganicFertilizerID != -1
            && storedIDs.OrganicFertilizerID != OrganicFertilizerID)
        {
            idMapping.Add(storedIDs.OrganicFertilizerID, OrganicFertilizerID);
            storedIDs.OrganicFertilizerID = OrganicFertilizerID;
        }

        // Update stored IDs for the special ones.
        if (FruitTreeFertilizerID != -1)
        {
            storedIDs.FruitTreeFertilizerID = FruitTreeFertilizerID;
        }

        if (DeluxeFruitTreeFertilizerID != -1)
        {
            storedIDs.DeluxeFruitTreeFertilizerID = DeluxeFruitTreeFertilizerID;
        }

        if (FishFoodID != -1)
        {
            storedIDs.FishFoodID = FishFoodID;
        }

        if (DeluxeFishFoodID != -1)
        {
            storedIDs.DeluxeFishFoodID = DeluxeFishFoodID;
        }

        if (DomesticatedFishFoodID != -1)
        {
            storedIDs.DomesticatedFishFoodID = DomesticatedFishFoodID;
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
            foreach (SObject obj in loc.Objects.Values)
            {
                if (obj is IndoorPot pot && pot.hoeDirt?.Value?.fertilizer?.Value is int value && value != 0)
                {
                    if (idMapping.TryGetValue(value, out int newvalue))
                    {
                        pot.hoeDirt.Value.fertilizer.Value = newvalue;
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
        this.Helper.GameContent.InvalidateCache("Data/ObjectInformation");
        this.Helper.GameContent.InvalidateCache($"Data/ObjectInformation.{this.Helper.Translation.Locale}");

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