/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

#if DEBUG
using System.Diagnostics;
#endif
using System.Runtime.CompilerServices;

using AtraBase.Toolkit;

using AtraCore.Utilities;

using AtraShared.ConstantsAndEnums;
using AtraShared.Integrations;
using AtraShared.Integrations.Interfaces;
using AtraShared.Menuing;
using AtraShared.MigrationManager;
using AtraShared.Utils;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.Shims;

using CommunityToolkit.Diagnostics;

using HarmonyLib;

using MoreFertilizers.DataModels;
using MoreFertilizers.Framework;
using MoreFertilizers.HarmonyPatches;
using MoreFertilizers.HarmonyPatches.Acquisition;
using MoreFertilizers.HarmonyPatches.Compat;
using MoreFertilizers.HarmonyPatches.EverlastingFertilizer;
using MoreFertilizers.HarmonyPatches.FishFood;
using MoreFertilizers.HarmonyPatches.FruitTreePatches;

using Newtonsoft.Json;

using StardewModdingAPI.Events;

using StardewValley.Buildings;
using StardewValley.TerrainFeatures;

using AtraUtils = AtraShared.Utils.Utils;

namespace MoreFertilizers;

/// <inheritdoc />
[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1124:Do not use regions", Justification = "Reviewed.")]
internal sealed class ModEntry : Mod
{
    private const string SavedIDKey = "MFSavedObjectID";

    private static IJsonAssetsAPI? jsonAssets;
    private static MoreFertilizerIDs? storedIDs;

    private MigrationManager? migrator;

    private Dictionary<int, int>? idmap;
    private ISolidFoundationsAPI? solidFoundationsAPI;

    #region IDs

#pragma warning disable SA1201 // Elements should appear in the correct order
    /// <summary>
    /// Gets a reference to the JA API.
    /// </summary>
    internal static IJsonAssetsAPI? JsonAssetsAPI => jsonAssets;

    private static int prismaticFertilizerID = -1;

    /// <summary>
    /// Gets the integer id of the Prismatic Fertilizer, or -1 if not found/not loaded yet.
    /// </summary>
    internal static int PrismaticFertilizerID
    {
        get
        {
            if (prismaticFertilizerID == -1)
            {
                prismaticFertilizerID = jsonAssets?.GetObjectId("Prismatic Fertilizer - More Fertilizers") ?? -1;
            }
            return prismaticFertilizerID;
        }
    }

    private static int everlastingFertilizerID = -1;

    /// <summary>
    /// Gets the integer ID of the Everlasting Fertilizer. Returns -1 if not found/not loaded yet.
    /// </summary>
    internal static int EverlastingFertilizerID
    {
        get
        {
            if (everlastingFertilizerID == -1)
            {
                everlastingFertilizerID = jsonAssets?.GetObjectId("Everlasting Fertilizer - More Fertilizers") ?? -1;
            }
            return everlastingFertilizerID;
        }
    }

    private static int wisdomFertilizerID = -1;

    /// <summary>
    /// Gets the integer ID of the Wisdom Fertilizer. Returns -1 if not found/not loaded yet.
    /// </summary>
    internal static int WisdomFertilizerID
    {
        get
        {
            if (wisdomFertilizerID == -1)
            {
                wisdomFertilizerID = jsonAssets?.GetObjectId("Wisdom Fertilizer - More Fertilizers") ?? -1;
            }
            return wisdomFertilizerID;
        }
    }

    private static int fruitTreeFertilizerID = -1;

    /// <summary>
    /// Gets the integer ID of the fruit tree fertilizer. -1 if not found/not loaded yet.
    /// </summary>
    internal static int FruitTreeFertilizerID
    {
        get
        {
            if (fruitTreeFertilizerID == -1)
            {
                fruitTreeFertilizerID = jsonAssets?.GetObjectId("Fruit Tree Fertilizer") ?? -1;
            }
            return fruitTreeFertilizerID;
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
            if (deluxeFruitTreeFertilizerID == -1)
            {
                deluxeFruitTreeFertilizerID = jsonAssets?.GetObjectId("Deluxe Fruit Tree Fertilizer") ?? -1;
            }
            return deluxeFruitTreeFertilizerID;
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
            if (fishfoodID == -1)
            {
                fishfoodID = jsonAssets?.GetObjectId("Fish Food Fertilizer") ?? -1;
            }
            return fishfoodID;
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
            if (deluxeFishFoodID == -1)
            {
                deluxeFishFoodID = jsonAssets?.GetObjectId("Deluxe Fish Food Fertilizer") ?? -1;
            }
            return deluxeFishFoodID;
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
            if (domesticatedFishFoodID == -1)
            {
                domesticatedFishFoodID = jsonAssets?.GetObjectId("Domesticated Fish Food Fertilizer") ?? -1;
            }
            return domesticatedFishFoodID;
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
            if (paddyCropFertilizerID == -1)
            {
                paddyCropFertilizerID = jsonAssets?.GetObjectId("Waterlogged Fertilizer") ?? -1;
            }
            return paddyCropFertilizerID;
        }
    }

    private static int luckyFertilizerID = -1;

    /// <summary>
    /// Gets the integer ID of the lucky fertilizer. -1 if not found/not loaded yet.
    /// </summary>
    internal static int LuckyFertilizerID
    {
        get
        {
            if (luckyFertilizerID == -1)
            {
                luckyFertilizerID = jsonAssets?.GetObjectId("Maebys Good-Luck Fertilizer") ?? -1;
            }
            return luckyFertilizerID;
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
            if (bountifulFertilizerID == -1)
            {
                bountifulFertilizerID = jsonAssets?.GetObjectId("Bountiful Fertilizer") ?? -1;
            }
            return bountifulFertilizerID;
        }
    }

    private static int bountifulBushID = -1;

    /// <summary>
    /// Gets the integer ID of the bountiful bush fertilizer. -1 if not found/not loaded yet.
    /// </summary>
    internal static int BountifulBushID
    {
        get
        {
            if (bountifulBushID == -1)
            {
                bountifulBushID = jsonAssets?.GetObjectId("Bountiful Bush Fertilizer") ?? -1;
            }
            return bountifulBushID;
        }
    }

    private static int rapidBushFertilizerID = -1;

    /// <summary>
    /// Gets the integer ID of the rapid bush fertilizer. -1 if not found/not loaded yet.
    /// </summary>
    internal static int RapidBushFertilizerID
    {
        get
        {
            if (rapidBushFertilizerID == -1)
            {
                rapidBushFertilizerID = jsonAssets?.GetObjectId("Rapid Bush Fertilizer") ?? -1;
            }
            return rapidBushFertilizerID;
        }
    }

    private static int treeTapperFertilizerID = -1;

    /// <summary>
    /// Gets the integer ID of the tree tapper fertilizer.
    /// </summary>
    internal static int TreeTapperFertilizerID
    {
        get
        {
            if (treeTapperFertilizerID == -1)
            {
                treeTapperFertilizerID = jsonAssets?.GetObjectId("Tree Tapper's Fertilizer - More Fertilizers") ?? -1;
            }
            return treeTapperFertilizerID;
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
            if (jojaFertilizerID == -1)
            {
                jojaFertilizerID = jsonAssets?.GetObjectId("Joja Fertilizer - More Fertilizers") ?? -1;
            }
            return jojaFertilizerID;
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
            if (deluxeJojaFertilizerID == -1)
            {
                deluxeJojaFertilizerID = jsonAssets?.GetObjectId("Deluxe Joja Fertilizer - More Fertilizers") ?? -1;
            }
            return deluxeJojaFertilizerID;
        }
    }

    private static int secretJojaFertilizerID = -1;

    /// <summary>
    /// Gets the integer ID of the Secret Joja's fertilizer. -1 if not found/not loaded yet.
    /// </summary>
    internal static int SecretJojaFertilizerID
    {
        get
        {
            if (secretJojaFertilizerID == -1)
            {
                secretJojaFertilizerID = jsonAssets?.GetObjectId("Secret Joja Fertilizer - More Fertilizers") ?? -1;
            }
            return secretJojaFertilizerID;
        }
    }

    private static int organicFertilizerID = -1;

    /// <summary>
    /// Gets the integer ID of the organic fertilizer. -1 if not found/not loaded yet.
    /// </summary>
    internal static int OrganicFertilizerID
    {
        get
        {
            if (organicFertilizerID == -1)
            {
                organicFertilizerID = jsonAssets?.GetObjectId("Organic Fertilizer - More Fertilizers") ?? -1;
            }
            return organicFertilizerID;
        }
    }

    private static int miraculousBeverages = -1;

    /// <summary>
    /// Gets the integer ID of the miraculous beverages fertilizer. -1 if not found/not loaded...
    /// </summary>
    internal static int MiraculousBeveragesID
    {
        get
        {
            if (miraculousBeverages == -1)
            {
                miraculousBeverages = jsonAssets?.GetObjectId("Miraculous Beverages - More Fertilizers") ?? -1;
            }
            return miraculousBeverages;
        }
    }

    private static int seedyFertilizerID = -1;

    /// <summary>
    /// Gets the integer ID of the seedy fertilizer. -1 if not found/not loaded.
    /// </summary>
    internal static int SeedyFertilizerID
    {
        get
        {
            if (seedyFertilizerID == -1)
            {
                seedyFertilizerID = jsonAssets?.GetObjectId("Seedy Fertilizer - More Fertilizers") ?? -1;
            }
            return seedyFertilizerID;
        }
    }

    private static int radioactiveFertilizerID = -1;

    /// <summary>
    /// Gets the integer ID of the radioactive fertilizer, or -1 if not found/not loaded.
    /// </summary>
    internal static int RadioactiveFertilizerID
    {
        get
        {
            if (radioactiveFertilizerID == -1)
            {
                radioactiveFertilizerID = jsonAssets?.GetObjectId("Radioactive Fertilizer - More Fertilizers") ?? -1;
            }
            return radioactiveFertilizerID;
        }
    }
#pragma warning restore SA1201 // Elements should appear in the correct order

    #endregion

    /// <summary>
    /// Gets a list of fertilizer IDs for fertilizers that are meant to be planted into HoeDirt.
    /// </summary>
    /// <remarks>Will be stored in the <see cref="HoeDirt.fertilizer.Value"/> field.</remarks>
    internal static HashSet<int> PlantableFertilizerIDs { get; } = new ();

    /// <summary>
    /// Gets a list of fertilizer IDs for fertilizers that are placed in other means (not into HoeDirt).
    /// </summary>
    /// <remarks>Handled by <see cref="SpecialFertilizerApplication" /> and typically stored in <see cref="ModDataDictionary"/>.</remarks>
    internal static HashSet<int> SpecialFertilizerIDs { get; } = new();

    /**************
     * Generally useful things that need to be attached to something static.
     **************/

    /// <summary>
    /// Gets the logger for this mod.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; } = null!;

    /// <summary>
    /// Gets the multi-player gmcmHelper for this mod.
    /// </summary>
    internal static IMultiplayerHelper MultiplayerHelper { get; private set; } = null!;

    /// <summary>
    /// Gets the mod content gmcmHelper for this mod.
    /// </summary>
    internal static IModContentHelper ModContentHelper { get; private set; } = null!;

    /// <summary>
    /// Gets the location of this mod.
    /// </summary>
    internal static string DIRPATH { get; private set; } = null!;

    /// <summary>
    /// Gets this mod's uniqueID.
    /// </summary>
    internal static string UNIQUEID { get; private set; } = null!;

    /// <summary>
    /// Gets the config instance for this mod.
    /// </summary>
    internal static ModConfig Config { get; private set; } = null!;

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        AssetEditor.Initialize(helper.GameContent);

        MultiplayerHelper = helper.Multiplayer;
        ModContentHelper = helper.ModContent;
        ModMonitor = this.Monitor;
        DIRPATH = helper.DirectoryPath;
        UNIQUEID = this.ModManifest.UniqueID;
        Config = AtraUtils.GetConfigOrDefault<ModConfig>(helper, this.Monitor);

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        this.Monitor.Log($"Starting up: {this.ModManifest.UniqueID} - {typeof(ModEntry).Assembly.FullName}");

#if DEBUG
        helper.Events.Input.ButtonPressed += this.DebugOutput;
#endif
    }

    /// <inheritdoc />
    [UsedImplicitly]
    public override object GetApi() => new CanPlaceHandler();

    /// <inheritdoc cref="IInputEvents.ButtonPressed"/>
    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if ((e.Button.IsUseToolButton() || e.Button.IsActionButton())
            && MenuingExtensions.IsNormalGameplay())
        {
            SpecialFertilizerApplication.ApplyFertilizer(e, this.Helper.Input);
        }
    }

#if DEBUG
    /// <inheritdoc cref="IInputEvents.ButtonPressed"/>
    private void DebugOutput(object? sender, ButtonPressedEventArgs e)
    {
        if (MenuingExtensions.IsNormalGameplay() && e.Button.IsUseToolButton())
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
                    this.Monitor.Log($"{e.Cursor.Tile} {(tree?.modData?.GetBool(CanPlaceHandler.TreeFertilizer) == true ? "had" : "did not have" )} tree fertilizer and is growth stage {tree?.growthStage?.Value ?? -1}", LogLevel.Info);
                }
                else if (terrainFeature is Bush bush)
                {
                    string fertilizer = bush?.modData is null ? string.Empty
                        : bush.modData.GetBool(CanPlaceHandler.MiraculousBeverages) == true ? "beverages"
                        : bush.modData.GetBool(CanPlaceHandler.RapidBush) == true ? "rapid"
                        : bush.modData.GetBool(CanPlaceHandler.BountifulBush) == true ? "bountiful"
                        : string.Empty;
                    this.Monitor.Log($"{e.Cursor.Tile} has {fertilizer} fertilizer.", LogLevel.Info);
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
        // JA will reassign us IDs when it returns to title.
        // (I'm not quite sure why?)
        // But we need to drop our IDs too.
        bountifulBushID = -1;
        bountifulFertilizerID = -1;
        deluxeFishFoodID = -1;
        deluxeFruitTreeFertilizerID = -1;
        deluxeJojaFertilizerID = -1;
        domesticatedFishFoodID = -1;
        everlastingFertilizerID = -1;
        fishfoodID = -1;
        fruitTreeFertilizerID = -1;
        jojaFertilizerID = -1;
        luckyFertilizerID = -1;
        miraculousBeverages = -1;
        organicFertilizerID = -1;
        paddyCropFertilizerID = -1;
        prismaticFertilizerID = -1;
        rapidBushFertilizerID = -1;
        secretJojaFertilizerID = -1;
        seedyFertilizerID = -1;
        treeTapperFertilizerID = -1;
        wisdomFertilizerID = -1;
        radioactiveFertilizerID = -1;

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
        this.Helper.Events.GameLoop.Saving -= this.OnSaving;
        if (!Context.IsMainPlayer)
        {
            return;
        }

        // TODO: This should be doable with expression trees in a less dumb way.
        if (storedIDs is null)
        {
            storedIDs = new()
            {
                BountifulBushID = BountifulBushID,
                BountifulFertilizerID = BountifulFertilizerID,
                DeluxeFishFoodID = DeluxeFishFoodID,
                DeluxeFruitTreeFertilizerID = DeluxeFruitTreeFertilizerID,
                DeluxeJojaFertilizerID = DeluxeJojaFertilizerID,
                DomesticatedFishFoodID = DomesticatedFishFoodID,
                EverlastingFertilizerID = EverlastingFertilizerID,
                FishFoodID = FishFoodID,
                FruitTreeFertilizerID = FruitTreeFertilizerID,
                JojaFertilizerID = JojaFertilizerID,
                LuckyFertilizerID = LuckyFertilizerID,
                MiraculousBeveragesID = MiraculousBeveragesID,
                OrganicFertilizerID = OrganicFertilizerID,
                PaddyFertilizerID = PaddyCropFertilizerID,
                PrismaticFertilizerID = PrismaticFertilizerID,
                RapidBushFertilizerID = RapidBushFertilizerID,
                SecretJojaFertilizerID = SecretJojaFertilizerID,
                SeedyFertilizerID = SeedyFertilizerID,
                TreeTapperFertilizerID = TreeTapperFertilizerID,
                WisdomFertilizerID = WisdomFertilizerID,
                RadioactiveFertilizerID = RadioactiveFertilizerID,
            };
        }
        this.Helper.Data.WriteSaveData(SavedIDKey, storedIDs);
        this.Monitor.Log("Writing IDs into save data");
    }

    /// <summary>
    /// Applies the patches for this mod.
    /// </summary>
    /// <param name="harmony">This mod's harmony instance.</param>
    private void ApplyPatches(Harmony harmony)
    {
#if DEBUG
        Stopwatch sw = new();
        sw.Start();
#endif

        try
        {
            harmony.PatchAll(typeof(ModEntry).Assembly);

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
                CropNewDayTranspiler.ApplyDGAPatches(harmony);
            }

            if (this.Helper.ModRegistry.Get("PeacefulEnd.AlternativeTextures") is IModInfo at
                && at.Manifest.Version.IsNewerThan("6.0.0"))
            {
                this.Monitor.Log("Found Alternative Textures, applying compat patches", LogLevel.Info);
                FruitTreeDrawTranspiler.ApplyATPatch(harmony);
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
                PerformObjectDropInTranspiler.ApplyAutomateTranspiler(harmony);
            }

            if (this.Helper.ModRegistry.Get("Satozaki.MillerTime") is IModInfo millerTime
                && millerTime.Manifest.Version.IsNewerThan("0.99.0"))
            {
                this.Monitor.Log("Found Miller Time, applying compat patches", LogLevel.Info);
                MillerTimeDayUpdateTranspiler.ApplyPatches(harmony);
            }

            if (this.Helper.ModRegistry.IsLoaded("stokastic.PrismaticTools") ||
                this.Helper.ModRegistry.IsLoaded("kakashigr.RadioactiveTools"))
            {
                this.Monitor.Log("Found either prismatic tools or radioactive tools. Applying compat patches", LogLevel.Info);
                ExtendedToolsMods.ApplyPatches(harmony);
                AddCrowsForExtendedToolsTranspiler.ApplyPatches(harmony);
            }

            if (this.Helper.ModRegistry.Get("spacechase0.TheftOfTheWinterStar") is IModInfo winterStar
                && winterStar.Manifest.Version.IsNewerThan("1.2.3"))
            {
                this.Monitor.Log("Found Theft of the Winter Star, applying compat patches", LogLevel.Info);
                RemoveSeasonCheck.ApplyPatchesForWinterStar(harmony);
            }
            else
            {
                RemoveSeasonCheck.ApplyPatches(harmony);
            }
        }
        catch (Exception ex)
        {
            this.Monitor.Log(string.Format(ErrorMessageConsts.HARMONYCRASH, ex), LogLevel.Error);
        }
        harmony.Snitch(this.Monitor, harmony.Id, transpilersOnly: true);
#if DEBUG
        sw.Stop();
        this.Monitor.Log($"took {sw.ElapsedMilliseconds} ms to apply harmony patches", LogLevel.Info);
#endif
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        IntegrationHelper jaHelper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, LogLevel.Warn);
        if (!jaHelper.TryGetAPI("spacechase0.JsonAssets", "1.10.3", out jsonAssets))
        {
            this.Monitor.Log("Packs could not be loaded! This mod will probably not function.", LogLevel.Error);
            return;
        }
        jsonAssets.LoadAssets(Path.Combine(this.Helper.DirectoryPath, "assets", "json-assets"), this.Helper.Translation);

        RadioactiveFertilizerHandler.Initialize(this.Helper.GameContent, this.Helper.ModRegistry, this.Helper.Translation);

        // Only register for events if JA pack loading was successful.
        this.Helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        this.Helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;

        this.Helper.Events.Multiplayer.ModMessageReceived += this.Multiplayer_ModMessageReceived;
        this.Helper.Events.Multiplayer.PeerConnected += this.Multiplayer_PeerConnected;

        this.Helper.Events.Player.Warped += this.OnPlayerWarp;

        this.Helper.Events.GameLoop.DayStarted += this.OnDayStart;
        this.Helper.Events.GameLoop.DayEnding += this.OnDayEnd;
        this.Helper.Events.Input.ButtonPressed += this.OnButtonPressed;

        this.Helper.Events.Content.AssetRequested += static (_, e) => AssetEditor.Edit(e);
        this.Helper.Events.Content.AssetsInvalidated += this.OnAssetInvalidated;

        if (this.Helper.ModRegistry.IsLoaded("atravita.SpecialOrdersExtended"))
        {
            this.Helper.Events.Content.AssetRequested += static (_, e) => AssetEditor.EditSpecialOrderDialogue(e);
        }

        // Apply harmony patches.
        this.ApplyPatches(new Harmony(this.ModManifest.UniqueID));

        // Handle optional integrations.
        GMCMHelper gmcmHelper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, this.ModManifest);
        if (gmcmHelper.TryGetAPI())
        {
            gmcmHelper.TryGetOptionsAPI();

            gmcmHelper.Register(
                reset: static () => Config = new(),
                save: () => this.Helper.AsyncWriteConfig(this.Monitor, Config))
                .AddParagraph(I18n.Mod_Description)
                .GenerateDefaultGMCM(static () => Config);
        }
        IntegrationHelper helper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, LogLevel.Trace);
        if (helper.TryGetAPI("TehPers.FishingOverhaul", "3.2.7", out ISimplifiedFishingApi? fishingAPI))
        {
            fishingAPI.ModifyChanceForFish(static (Farmer who, double chance) =>
                who.currentLocation is null ? chance : GetFishTranspiler.AlterFishChance(chance, who.currentLocation));
        }

        CropHarvestTranspiler.Initialize(this.Helper.ModRegistry);
    }

    /// <inheritdoc cref="IContentEvents.AssetsInvalidated"/>
    private void OnAssetInvalidated(object? sender, AssetsInvalidatedEventArgs e)
    {
        RadioactiveFertilizerHandler.Reset(e.NamesWithoutLocale);
        AssetEditor.Reset(e.NamesWithoutLocale);
    }

    /// <inheritdoc cref="IGameLoopEvents.DayStarted"/>
    private void OnDayStart(object? sender, DayStartedEventArgs e)
        => GameLocationPatches.Reinitialize();

    /// <inheritdoc cref="IGameLoopEvents.DayEnding"/>
    private void OnDayEnd(object? sender, DayEndingEventArgs e)
    {
        if (Game1.player.getFriendshipHeartLevelForNPC("George") >= 6 && Game1.player.mailReceived.Contains("georgeGifts"))
        {
            Game1.addMailForTomorrow(AssetEditor.GEORGE_EVENT);
        }

        if (Game1.getAllFarmers().Any(p => p.foragingLevel.Value >= 4))
        {
            Game1.addMailForTomorrow(AssetEditor.BOUNTIFUL_BUSH_UNLOCK);
        }

        JojaSample.Reset();
        FishFoodHandler.DecrementAndSave(this.Helper.Data, this.Helper.Multiplayer);
        RadioactiveFertilizerHandler.OnDayEnd();
    }

    #region JsonAssets
    private void GrabIds()
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

        if (BountifulBushID != -1)
        {
            SpecialFertilizerIDs.Add(BountifulBushID);
        }

        if (RapidBushFertilizerID != -1)
        {
            SpecialFertilizerIDs.Add(RapidBushFertilizerID);
        }

        if (TreeTapperFertilizerID != -1)
        {
            SpecialFertilizerIDs.Add(TreeTapperFertilizerID);
        }

        if (PrismaticFertilizerID != -1)
        {
            SpecialFertilizerIDs.Add(PrismaticFertilizerID);
        }

        // Plant-able ones begin here.
        if (EverlastingFertilizerID != -1)
        {
            PlantableFertilizerIDs.Add(EverlastingFertilizerID);
        }

        if (WisdomFertilizerID != -1)
        {
            PlantableFertilizerIDs.Add(WisdomFertilizerID);
        }

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

        if (SecretJojaFertilizerID != -1)
        {
            PlantableFertilizerIDs.Add(SecretJojaFertilizerID);
        }

        if (OrganicFertilizerID != -1)
        {
            PlantableFertilizerIDs.Add(OrganicFertilizerID);
        }

        if (MiraculousBeveragesID != -1)
        {
            PlantableFertilizerIDs.Add(MiraculousBeveragesID);
        }

        if (SeedyFertilizerID != -1)
        {
            PlantableFertilizerIDs.Add(SeedyFertilizerID);
        }

        if (RadioactiveFertilizerID != -1)
        {
            PlantableFertilizerIDs.Add(RadioactiveFertilizerID);
        }

        if (SpecialFertilizerIDs.Count <= 0 && PlantableFertilizerIDs.Count <= 0)
        {
            this.Monitor.Log("No valid IDs found?");
        }
    }

    private void FixIDs()
    {
        if (SpecialFertilizerIDs.Count <= 0 && PlantableFertilizerIDs.Count <= 0)
        {
            this.Monitor.Log("No valid IDs found while attempting to deshuffle?");
            return;
        }

        if (this.Helper.Data.ReadSaveData<MoreFertilizerIDs>(SavedIDKey) is not MoreFertilizerIDs storedIDCls)
        {
            ModMonitor.Log("No need to fix IDs, not installed before.");

            this.Helper.Events.GameLoop.Saving -= this.OnSaving;
            this.Helper.Events.GameLoop.Saving += this.OnSaving;

            return;
        }
        storedIDs = storedIDCls;

        Dictionary<int, int> idMapping = new();

        // special case! Update the museum reward tracking too...
        if (PrismaticFertilizerID != -1)
        {
            if (storedIDs.PrismaticFertilizerID != -1
            && PrismaticFertilizerID != storedIDs.PrismaticFertilizerID)
            {
                string oldkey = $"museumCollectedRewardO_{storedIDs.PrismaticFertilizerID}_1";
                string newkey = $"museumCollectedRewardO_{PrismaticFertilizerID}_1";

                foreach (Farmer player in Game1.getAllFarmers())
                {
                    if (player.mailReceived.Remove(oldkey))
                    {
                        player.mailReceived.Add(newkey);
                    }
                }
            }
            storedIDs.PrismaticFertilizerID = PrismaticFertilizerID;
        }

        // Have to update the planted ones.
        if (EverlastingFertilizerID != -1)
        {
            if (storedIDs.EverlastingFertilizerID != -1 && EverlastingFertilizerID != storedIDs.EverlastingFertilizerID)
            {
                idMapping.Add(storedIDs.EverlastingFertilizerID, EverlastingFertilizerID);
            }
            storedIDs.EverlastingFertilizerID = EverlastingFertilizerID;
        }

        if (WisdomFertilizerID != -1)
        {
            if (storedIDs.WisdomFertilizerID != -1 && storedIDs.WisdomFertilizerID != WisdomFertilizerID)
            {
                idMapping.Add(storedIDs.WisdomFertilizerID, WisdomFertilizerID);
            }
            storedIDs.WisdomFertilizerID = WisdomFertilizerID;
        }

        if (LuckyFertilizerID != -1)
        {
            if (storedIDs.LuckyFertilizerID != -1 && storedIDs.LuckyFertilizerID != LuckyFertilizerID)
            {
                idMapping.Add(storedIDs.LuckyFertilizerID, LuckyFertilizerID);
            }
            storedIDs.LuckyFertilizerID = LuckyFertilizerID;
        }

        if (PaddyCropFertilizerID != -1)
        {
            if (storedIDs.PaddyFertilizerID != -1 && storedIDs.PaddyFertilizerID != PaddyCropFertilizerID)
            {
                idMapping.Add(storedIDs.PaddyFertilizerID, PaddyCropFertilizerID);
            }
            storedIDs.PaddyFertilizerID = PaddyCropFertilizerID;
        }

        if (BountifulFertilizerID != -1)
        {
            if (storedIDs.BountifulFertilizerID != -1 && storedIDs.BountifulFertilizerID != BountifulFertilizerID)
            {
                idMapping.Add(storedIDs.BountifulFertilizerID, BountifulFertilizerID);
            }
            storedIDs.BountifulFertilizerID = BountifulFertilizerID;
        }

        if (JojaFertilizerID != -1)
        {
            if (storedIDs.JojaFertilizerID != -1 && storedIDs.JojaFertilizerID != JojaFertilizerID)
            {
                idMapping.Add(storedIDs.JojaFertilizerID, JojaFertilizerID);
            }
            storedIDs.JojaFertilizerID = JojaFertilizerID;
        }

        if (DeluxeJojaFertilizerID != -1)
        {
            if (storedIDs.DeluxeJojaFertilizerID != -1 && storedIDs.DeluxeJojaFertilizerID != DeluxeJojaFertilizerID)
            {
                idMapping.Add(storedIDs.DeluxeJojaFertilizerID, DeluxeJojaFertilizerID);
            }
            storedIDs.DeluxeJojaFertilizerID = DeluxeJojaFertilizerID;
        }

        if (SecretJojaFertilizerID != -1)
        {
            if (storedIDs.SecretJojaFertilizerID != -1 && storedIDs.SecretJojaFertilizerID != SecretJojaFertilizerID)
            {
                idMapping.Add(storedIDs.SecretJojaFertilizerID, SecretJojaFertilizerID);
            }
            storedIDs.SecretJojaFertilizerID = SecretJojaFertilizerID;
        }

        if (OrganicFertilizerID != -1)
        {
            if (storedIDs.OrganicFertilizerID != -1 && storedIDs.OrganicFertilizerID != OrganicFertilizerID)
            {
                idMapping.Add(storedIDs.OrganicFertilizerID, OrganicFertilizerID);
            }
            storedIDs.OrganicFertilizerID = OrganicFertilizerID;
        }

        if (MiraculousBeveragesID != -1)
        {
            if (storedIDs.MiraculousBeveragesID != -1 && storedIDs.MiraculousBeveragesID != MiraculousBeveragesID)
            {
                idMapping.Add(storedIDs.MiraculousBeveragesID, MiraculousBeveragesID);
            }
            storedIDs.MiraculousBeveragesID = MiraculousBeveragesID;
        }

        if (SeedyFertilizerID != -1)
        {
            if (storedIDs.SeedyFertilizerID != -1 && storedIDs.SeedyFertilizerID != SeedyFertilizerID)
            {
                idMapping.Add(storedIDs.SeedyFertilizerID, SeedyFertilizerID);
            }
            storedIDs.SeedyFertilizerID = SeedyFertilizerID;
        }

        if (RadioactiveFertilizerID != -1)
        {
            if (storedIDs.RadioactiveFertilizerID != -1 && storedIDs.RadioactiveFertilizerID != RadioactiveFertilizerID)
            {
                idMapping.Add(storedIDs.RadioactiveFertilizerID, RadioactiveFertilizerID);
            }
            storedIDs.RadioactiveFertilizerID = RadioactiveFertilizerID;
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

        if (RapidBushFertilizerID != -1)
        {
            storedIDs.RapidBushFertilizerID = RapidBushFertilizerID;
        }

        if (BountifulBushID != -1)
        {
            storedIDs.BountifulBushID = BountifulBushID;
        }

        if (TreeTapperFertilizerID != -1)
        {
            storedIDs.TreeTapperFertilizerID = TreeTapperFertilizerID;
        }

        if (idMapping.Count <= 0)
        {
            ModMonitor.Log("No need to fix IDs, nothing has changed.");
            return;
        }

        this.Helper.Events.GameLoop.Saving -= this.OnSaving;
        this.Helper.Events.GameLoop.Saving += this.OnSaving;

        // Grab the SF API to deshuffle in there too.
        IntegrationHelper helper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, LogLevel.Trace);
        if (this.solidFoundationsAPI is not null || helper.TryGetAPI("PeacefulEnd.SolidFoundations", "1.12.1", out this.solidFoundationsAPI))
        {
            this.idmap = idMapping;
            this.solidFoundationsAPI.AfterBuildingRestoration -= this.AfterSFBuildingRestore;
            this.solidFoundationsAPI.AfterBuildingRestoration += this.AfterSFBuildingRestore;
        }

        Utility.ForAllLocations((GameLocation loc) => loc.FixHoeDirtInLocation(idMapping));

        ModMonitor.Log($"Fixed IDs! {string.Join(", ", idMapping.Select((kvp) => $"{kvp.Key}=>{kvp.Value}"))}");
    }

    private void AfterSFBuildingRestore(object? sender, EventArgs e)
    {
        // unhook event
        this.solidFoundationsAPI!.AfterBuildingRestoration -= this.AfterSFBuildingRestore;
        try
        {
            if (SolidFoundationShims.IsSFBuilding is null)
            {
                this.Monitor.Log("Could not get a handle on SF's building class, deshuffling code will fail!", LogLevel.Error);
            }
            else if (this.idmap is null)
            {
                this.Monitor.Log("IdMap was not set correctly, deshuffling code will fail.", LogLevel.Error);
            }
            else
            {
                foreach (Building? building in GameLocationUtils.GetBuildings())
                {
                    if (SolidFoundationShims.IsSFBuilding?.Invoke(building) == true)
                    {
                        building.indoors.Value?.FixHoeDirtInLocation(this.idmap);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            this.Monitor.Log($"Failed while attempting to deshuffle in SF buildings:\n\n{ex}", LogLevel.Error);
        }
        this.idmap = null;
        this.solidFoundationsAPI = null;
    }

    #endregion

    #region migration

    /// <inheritdoc cref="IGameLoopEvents.SaveLoaded"/>
    [EventPriority(EventPriority.Low)]
    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        JojaSample.Reset();

        if (Context.IsSplitScreen && Context.ScreenId != 0)
        {
            return;
        }

        MiraculousFertilizerHandler.Initialize();
        MultiplayerHelpers.AssertMultiplayerVersions(this.Helper.Multiplayer, this.ModManifest, this.Monitor, this.Helper.Translation);

        IntegrationHelper pfmHelper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, LogLevel.Trace);
        if (pfmHelper.TryGetAPI("Digus.ProducerFrameworkMod", "1.7.4", out IProducerFrameworkModAPI? pfmAPI))
        {
            pfmAPI.AddContentPack(Path.Combine(this.Helper.DirectoryPath, "assets", "pfm-assets"));
        }

        this.migrator = new(this.ModManifest, this.Helper, this.Monitor);

        if (!this.migrator.CheckVersionInfo())
        {
            this.Helper.Events.GameLoop.Saved += this.WriteMigrationData;
            try
            {
                this.migrator.RunMigration(new SemanticVersion("0.3.0"), this.GetIdsFromJAIfNeeded);
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"Failed while attempting to run migrations.\n\n{ex}");
            }
        }
        else
        {
            this.migrator = null;
        }

        this.GrabIds();
        if (Context.IsMainPlayer)
        {
            this.FixIDs();
        }

        if (Context.IsMainPlayer)
        {
            FishFoodHandler.LoadHandler(this.Helper.Data, this.Helper.Multiplayer);
        }
    }

    [MethodImpl(TKConstants.Cold)]
    private bool GetIdsFromJAIfNeeded(IModHelper helper, IMonitor monitor)
    {
        if (!Context.IsMainPlayer)
        {
            return true;
        }

        this.Monitor.Log($"Running migration for 0.3.0.");
        Guard.IsNotNull(Constants.CurrentSavePath);

        if (this.Helper.Data.ReadSaveData<MoreFertilizerIDs>(SavedIDKey) is not MoreFertilizerIDs storedIDCls)
        {
            monitor.Log("Ids not found.");
            storedIDCls = new();
        }

        string path = Path.Combine(Constants.CurrentSavePath, "JsonAssets", "ids-objects.json");
        if (!File.Exists(path))
        {
            monitor.Log($"Can't find JA id file");
            return true;
        }

        Dictionary<string, int>? idsFromJA;
        try
        {
            idsFromJA = JsonConvert.DeserializeObject<Dictionary<string, int>>(File.ReadAllText(path));
        }
        catch (Exception ex)
        {
            monitor.Log($"Tried to deserialize JA's data, couldn't.\n\n{ex}", LogLevel.Warn);
            return false;
        }

        if (idsFromJA is null)
        {
            monitor.Log($"Tried to deserialize JA's data, couldn't - got null instead.");
            return true;
        }

        if (storedIDCls.PrismaticFertilizerID == -1 && idsFromJA.TryGetValue("Prismatic Fertilizer - More Fertilizers", out int oldprismatic))
        {
            monitor.Log($"Grabbing old prismatic ID from JA");
            storedIDCls.PrismaticFertilizerID = oldprismatic;
        }

        if (storedIDCls.EverlastingFertilizerID == -1 && idsFromJA.TryGetValue("Everlasting Fertilizer - More Fertilizers", out int oldeverlasting))
        {
            monitor.Log($"Grabbing old everlasting from JA");
            storedIDCls.EverlastingFertilizerID = oldeverlasting;
        }

        if (storedIDCls.WisdomFertilizerID == -1 && idsFromJA.TryGetValue("Wisdom Fertilizer - More Fertilizers", out int oldwisdom))
        {
            monitor.Log($"Grabbing old wisdom ID from JA");
            storedIDCls.WisdomFertilizerID = oldwisdom;
        }

        if (storedIDCls.PaddyFertilizerID == -1 && idsFromJA.TryGetValue("Waterlogged Fertilizer", out int oldpaddy))
        {
            monitor.Log($"Grabbing old waterlogged ID from JA");
            storedIDCls.PaddyFertilizerID = oldpaddy;
        }

        if (storedIDCls.LuckyFertilizerID == -1 && idsFromJA.TryGetValue("Maebys Good-Luck Fertilizer", out int oldlucky))
        {
            monitor.Log($"Grabbing old lucky ID from JA");
            storedIDCls.LuckyFertilizerID = oldlucky;
        }

        if (storedIDCls.BountifulFertilizerID == -1 && idsFromJA.TryGetValue("Bountiful Fertilizer", out int oldbountiful))
        {
            monitor.Log($"Grabbing old bountiful ID from JA");
            storedIDCls.BountifulFertilizerID = oldbountiful;
        }

        if (storedIDCls.JojaFertilizerID == -1 && idsFromJA.TryGetValue("Joja Fertilizer - More Fertilizers", out int oldjoja))
        {
            monitor.Log($"Grabbing old joja ID from JA");
            storedIDCls.JojaFertilizerID = oldjoja;
        }

        if (storedIDCls.DeluxeJojaFertilizerID == -1 && idsFromJA.TryGetValue("Deluxe Joja Fertilizer - More Fertilizers", out int olddeluxe))
        {
            monitor.Log($"Grabbing old deluxe joja ID from JA");
            storedIDCls.DeluxeJojaFertilizerID = olddeluxe;
        }

        if (storedIDCls.SecretJojaFertilizerID == -1 && idsFromJA.TryGetValue("Secret Joja Fertilizer - More Fertilizers", out int oldsecret))
        {
            monitor.Log($"Grabbing old secret joja ID from JA");
            storedIDCls.SecretJojaFertilizerID = oldsecret;
        }

        if (storedIDCls.OrganicFertilizerID == -1 && idsFromJA.TryGetValue("Organic Fertilizer - More Fertilizers", out int oldorganic))
        {
            monitor.Log($"Grabbing old organic ID from JA");
            storedIDCls.OrganicFertilizerID = oldorganic;
        }

        if (storedIDCls.MiraculousBeveragesID == -1 && idsFromJA.TryGetValue("Miraculous Beverages - More Fertilizers", out int oldbeverage))
        {
            monitor.Log($"Grabbing old beverage ID from JA");
            storedIDCls.MiraculousBeveragesID = oldbeverage;
        }

        if (storedIDCls.SeedyFertilizerID == -1 && idsFromJA.TryGetValue("Seedy Fertilizer - More Fertilizers", out int oldseedy))
        {
            monitor.Log($"Grabbing old seedy ID from JA");
            storedIDCls.SeedyFertilizerID = oldseedy;
        }

        this.Helper.Data.WriteSaveData(SavedIDKey, storedIDCls);
        return true;
    }

    /// <inheritdoc cref="IGameLoopEvents.Saved"/>
    /// <remarks>
    /// Writes migration data then detaches the migrator.
    /// </remarks>
    private void WriteMigrationData(object? sender, SavedEventArgs e)
    {
        if (this.migrator is not null)
        {
            this.migrator.SaveVersionInfo();
            this.migrator = null;
        }
        this.Helper.Events.GameLoop.Saved -= this.WriteMigrationData;
    }

    #endregion

    /// <inheritdoc cref="IPlayerEvents.Warped"/>
    [EventPriority(EventPriority.Low)]
    private void OnPlayerWarp(object? sender, WarpedEventArgs e)
    {
        if (e.IsLocalPlayer)
        {
            JojaSample.JojaSampleEvent(e);
            FishFoodHandler.HandleWarp(e);
        }
    }

    #region multiplayer

    /// <inheritdoc cref="IMultiplayerEvents.PeerConnected"/>
    private void Multiplayer_PeerConnected(object? sender, PeerConnectedEventArgs e)
    {
        if (e.Peer.ScreenID == 0 && Context.IsWorldReady && Context.IsMainPlayer)
        {
            this.Helper.Multiplayer.SendMessage(FishFoodHandler.UnsavedLocHandler, "DATAPACKAGE", new[] { UNIQUEID }, new[] { e.Peer.PlayerID });
        }
    }

    /// <inheritdoc cref="IMultiplayerEvents.ModMessageReceived"/>
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
    #endregion
}
