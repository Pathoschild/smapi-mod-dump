/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/GiantCropFertilizer
**
*************************************************/

using AtraShared.ConstantsAndEnums;
using AtraShared.Integrations;
using AtraShared.Integrations.Interfaces;
using AtraShared.MigrationManager;
using AtraShared.Utils;
using AtraShared.Utils.Extensions;
using GiantCropFertilizer.DataModels;
using GiantCropFertilizer.HarmonyPatches;
using HarmonyLib;
using StardewModdingAPI.Events;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using AtraUtils = AtraShared.Utils.Utils;

namespace GiantCropFertilizer;

/// <inheritdoc />
internal class ModEntry : Mod
{
    private const string SAVESTRING = "SavedObjectID";

    private static IJsonAssetsAPI? jsonAssets;

    private MigrationManager? migrator;

    private GiantCropFertilizerIDStorage? storedID;

    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "Field kept near property.")]
    private static int giantCropFertilizerID = -1;

    /// <summary>
    /// Gets the integer ID of the giant crop fertilizer. -1 if not found/not loaded yet.
    /// </summary>
    internal static int GiantCropFertilizerID
    {
        get
        {
            if (giantCropFertilizerID == -1)
            {
                giantCropFertilizerID = jsonAssets?.GetObjectId("Giant Crop Fertilizer") ?? -1;
            }
            return giantCropFertilizerID;
        }
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <summary>
    /// Gets the logger for this mod.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; }

    /// <summary>
    /// Gets the config instance for this mod.
    /// </summary>
    internal static ModConfig Config { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        ModMonitor = this.Monitor;

        helper.Events.Content.AssetRequested += this.OnAssetRequested;
        Config = AtraUtils.GetConfigOrDefault<ModConfig>(helper, this.Monitor);

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        helper.Events.GameLoop.Saved += this.OnSaved;

        helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;
    }


    private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        => AssetEditor.HandleAssetRequested(e);

    private void OnSaved(object? sender, SavedEventArgs e)
    {
        if (Context.IsMainPlayer)
        {
            this.Helper.Data.WriteGlobalData(
                  SAVESTRING,
                  this.storedID ?? new GiantCropFertilizerIDStorage(GiantCropFertilizerID));
        }
    }

    /// <summary>
    /// Applies the patches for this mod.
    /// </summary>
    /// <param name="harmony">This mod's harmony instance.</param>
    /// <remarks>Delay until GameLaunched in order to patch other mods....</remarks>
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

            if (!this.Helper.ModRegistry.IsLoaded("spacechase0.MoreGiantCrops"))
            {
                RemoveFarmCheck.ApplyPatches(harmony);
            }

            if (this.Helper.ModRegistry.Get("spacechase0.DynamicGameAssets") is IModInfo dga
                && dga.Manifest.Version.IsNewerThan("1.4.1"))
            {
                this.Monitor.Log("Found Dynamic Game Assets, applying compat patches", LogLevel.Info);
                CropTranspiler.ApplyDGAPatches(harmony);
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
        // JSON ASSETS integration
        {
            IntegrationHelper helper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, LogLevel.Warn);
            if (helper.TryGetAPI("spacechase0.JsonAssets", "1.10.3", out jsonAssets))
            {
                jsonAssets.LoadAssets(Path.Combine(this.Helper.DirectoryPath, "assets", "json-assets"), this.Helper.Translation);
                jsonAssets.IdsFixed += this.JAIdsFixed;
                this.Monitor.Log("Loaded packs!");
            }
            else
            {
                this.Monitor.Log("Packs could not be loaded! This mod will probably not function.", LogLevel.Error);
                return;
            }
        }

        Task gmcm = Task.Run(() =>
        { // GMCM integration
            GMCMHelper gmcmHelper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, this.ModManifest);
            if (gmcmHelper.TryGetAPI())
            {
                gmcmHelper.Register(
                    reset: () => Config = new(),
                    save: () => this.Helper.WriteConfig(Config))
                .AddParagraph(I18n.ModDescription)
                .AddNumberOption(
                    name: I18n.GiantCropChance_Title,
                    getValue: () => (float)Config.GiantCropChance,
                    setValue: (float val) => Config.GiantCropChance = val,
                    tooltip: I18n.GiantCropChance_Description,
                    min: 0f,
                    max: 1.1f,
                    interval: 0.01f);

                if (this.Helper.ModRegistry.IsLoaded("spacechase0.MoreGiantCrops"))
                {
                    gmcmHelper.AddParagraph(I18n.AllowGiantCropsParagraph);
                }
                else
                {
                    gmcmHelper.AddBoolOption(
                        name: I18n.AllowGiantCropsOffFarm_Title,
                        getValue: () => Config.AllowGiantCropsOffFarm,
                        setValue: (val) => Config.AllowGiantCropsOffFarm = val,
                        tooltip: I18n.AllowGiantCropsOffFarm_Description);
                }
            }
        });

        this.ApplyPatches(new Harmony(this.ModManifest.UniqueID));
        gmcm.Wait();
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        this.FixIds();
        MultiplayerHelpers.AssertMultiplayerVersions(this.Helper.Multiplayer, this.ModManifest, this.Monitor, this.Helper.Translation);

        this.migrator = new(this.ModManifest, this.Helper, this.Monitor);
        this.migrator.ReadVersionInfo();
        this.Helper.Events.GameLoop.Saved += this.WriteMigrationData;
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

    /*********
     * REGION JSON ASSETS
     * *******/

    // Not quite sure why, but JA drops all IDs when returning to title. We're doing that too.
    [EventPriority(EventPriority.High)]
    private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
        => giantCropFertilizerID = -1;

    private void JAIdsFixed(object? sender, EventArgs e)
        => this.FixIds();

    private void FixIds()
    {
        int newID = GiantCropFertilizerID;
        if (newID == -1 || !Context.IsMainPlayer)
        {
            return;
        }

        if (this.Helper.Data.ReadGlobalData<GiantCropFertilizerIDStorage>(SAVESTRING) is not GiantCropFertilizerIDStorage storedIDCls)
        {
            ModMonitor.Log("No need to fix IDs, not installed before.");
            return;
        }

        this.storedID ??= storedIDCls;
        int storedID = this.storedID.ID;

        if (storedID == newID)
        {
            ModMonitor.Log("No need to fix IDs, nothing has changed.");
            return;
        }

        Utility.ForAllLocations((GameLocation loc) =>
        {
            foreach (TerrainFeature terrainfeature in loc.terrainFeatures.Values)
            {
                if (terrainfeature is HoeDirt dirt && dirt.fertilizer.Value == storedID)
                {
                    dirt.fertilizer.Value = newID;
                }
            }
            foreach (SObject obj in loc.Objects.Values)
            {
                if (obj is IndoorPot pot && pot.hoeDirt?.Value?.fertilizer?.Value == storedID)
                {
                    pot.hoeDirt.Value.fertilizer.Value = newID;
                }
            }
        });

        this.storedID.ID = newID;
        ModMonitor.Log($"Fixed IDs! {storedID} => {newID}");
    }
}
