/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraCore.Utilities;

using AtraShared.ConstantsAndEnums;
using AtraShared.Integrations;
using AtraShared.Integrations.Interfaces;
using AtraShared.MigrationManager;
using AtraShared.Utils;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.Shims;

using GiantCropFertilizer.DataModels;
using GiantCropFertilizer.HarmonyPatches;

using HarmonyLib;

using StardewModdingAPI.Events;

using StardewValley.Buildings;

using AtraUtils = AtraShared.Utils.Utils;

namespace GiantCropFertilizer;

/// <inheritdoc />
internal sealed class ModEntry : Mod
{
    private const string SAVESTRING = "SavedObjectID";

    private static IJsonAssetsAPI? jsonAssets;

    private int oldID = -1;
    private int newID = -1;
    private ISolidFoundationsAPI? solidFoundationsAPI;

    private MigrationManager? migrator;

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

        Config = AtraUtils.GetConfigOrDefault<ModConfig>(helper, this.Monitor);
        this.Monitor.Log($"Starting up: {this.ModManifest.UniqueID} - {typeof(ModEntry).Assembly.FullName}");

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
    }

    private void OnSaving(object? sender, SavingEventArgs e)
    {
        if (Context.IsMainPlayer)
        {
            this.Helper.Data.WriteSaveData(SAVESTRING, GiantCropFertilizerID.ToString());
            this.Monitor.Log($"Saved IDs!", LogLevel.Info);
        }
        this.Helper.Events.GameLoop.Saving -= this.OnSaving;
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

            if (new Version(1, 6) > new Version(Game1.version) &&
                (this.Helper.ModRegistry.Get("spacechase0.MoreGiantCrops") is not IModInfo giant || giant.Manifest.Version.IsOlderThan("1.2.0")))
            {
                this.Monitor.Log("Applying patch to restore giant crops to save locations", LogLevel.Debug);
                FixSaveThing.ApplyPatches(harmony);
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
        { // JSON ASSETS integration
            IntegrationHelper helper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, LogLevel.Warn);
            if (helper.TryGetAPI("spacechase0.JsonAssets", "1.10.3", out jsonAssets))
            {
                jsonAssets.LoadAssets(Path.Combine(this.Helper.DirectoryPath, "assets", "json-assets"), this.Helper.Translation);
            }
            else
            {
                this.Monitor.Log("Packs could not be loaded! This mod will probably not function.", LogLevel.Error);
                return;
            }
        }

        // Wait to hook events until after we know JA can handle our items.
        this.Helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        this.Helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;

        // GMCM integration
        {
            GMCMHelper gmcmHelper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, this.ModManifest);
            if (gmcmHelper.TryGetAPI())
            {
                gmcmHelper.Register(
                    reset: static () => Config = new(),
                    save: () => this.Helper.AsyncWriteConfig(this.Monitor, Config))
                .AddParagraph(I18n.ModDescription)
                .AddNumberOption(
                    name: I18n.GiantCropChance_Title,
                    getValue: static () => (float)Config.GiantCropChance,
                    setValue: static (float val) => Config.GiantCropChance = val,
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
                        getValue: static () => Config.AllowGiantCropsOffFarm,
                        setValue: static (val) => Config.AllowGiantCropsOffFarm = val,
                        tooltip: I18n.AllowGiantCropsOffFarm_Description);
                }
            }
        }

        this.ApplyPatches(new Harmony(this.ModManifest.UniqueID));
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        if (Context.IsSplitScreen && Context.ScreenId != 0)
        {
            return;
        }
        MultiplayerHelpers.AssertMultiplayerVersions(this.Helper.Multiplayer, this.ModManifest, this.Monitor, this.Helper.Translation);
        this.migrator = new(this.ModManifest, this.Helper, this.Monitor);
        if (!this.migrator.CheckVersionInfo())
        {
            this.Helper.Events.GameLoop.Saved += this.WriteMigrationData;
        }
        else
        {
            this.migrator = null;
        }

        this.GrabIds();
        if (Context.IsMainPlayer)
        {
            this.FixIds();
        }
    }

    /// <inheritdoc cref="IGameLoopEvents.Save"/>
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

    #region jsonAssets

    // Not quite sure why, but JA drops all IDs when returning to title. We're doing that too.
    [EventPriority(EventPriority.High + 100)]
    private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
        => giantCropFertilizerID = -1;

    private void GrabIds()
    {
        // reset the ID, ask for it again from JA?
        giantCropFertilizerID = -1;

        if (GiantCropFertilizerID == -1)
        {
            this.Monitor.Log($"Could not get ID from JA.");
        }
    }

    private void FixIds()
    {
        int newID = GiantCropFertilizerID;
        if (newID == -1)
        {
            this.Monitor.Log($"Could not get ID from JA.");
        }

        int storedID;
        if (this.Helper.Data.ReadSaveData<string>(SAVESTRING) is not string savedIdstring || !int.TryParse(savedIdstring, out storedID))
        {
            if (this.Helper.Data.ReadGlobalData<GiantCropFertilizerIDStorage>(SAVESTRING) is not GiantCropFertilizerIDStorage storedIDCls
                || storedIDCls.ID == -1)
            {
                this.Helper.Events.GameLoop.Saving -= this.OnSaving;
                this.Helper.Events.GameLoop.Saving += this.OnSaving;

                ModMonitor.Log("No need to fix IDs, not installed before.");
                return;
            }
            storedID = storedIDCls.ID;
        }

        if (storedID == newID)
        {
            ModMonitor.Log("No need to fix IDs, nothing has changed.");
            return;
        }

        this.Helper.Events.GameLoop.Saving -= this.OnSaving;
        this.Helper.Events.GameLoop.Saving += this.OnSaving;

        IntegrationHelper helper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, LogLevel.Trace);
        if (this.solidFoundationsAPI is not null || helper.TryGetAPI("PeacefulEnd.SolidFoundations", "1.12.1", out this.solidFoundationsAPI))
        {
            this.oldID = storedID;
            this.newID = newID;
            this.solidFoundationsAPI.AfterBuildingRestoration -= this.AfterSFBuildingRestore;
            this.solidFoundationsAPI.AfterBuildingRestoration += this.AfterSFBuildingRestore;
        }

        Utility.ForAllLocations((GameLocation loc) => loc.FixIDsInLocation(storedID, newID));

        ModMonitor.Log($"Fixed IDs! {storedID} => {newID}");
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
            else if (this.oldID == -1 || this.newID == -1)
            {
                this.Monitor.Log("IdMap was not set correctly, deshuffling code will fail.", LogLevel.Error);
            }
            else
            {
                foreach (Building? building in GameLocationUtils.GetBuildings())
                {
                    if (SolidFoundationShims.IsSFBuilding?.Invoke(building) == true)
                    {
                        building.indoors.Value?.FixIDsInLocation(this.oldID, this.newID);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            this.Monitor.Log($"Failed in deshuffling IDs in SF buildings:\n\n{ex}", LogLevel.Error);
        }
        this.oldID = -1;
        this.newID = -1;
        this.solidFoundationsAPI = null;
    }

    #endregion
}
