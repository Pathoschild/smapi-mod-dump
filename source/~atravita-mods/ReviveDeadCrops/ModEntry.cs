/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.ConstantsAndEnums;
using AtraShared.Integrations;
using AtraShared.Integrations.Interfaces;
using AtraShared.Menuing;
using AtraShared.Utils.Extensions;

using HarmonyLib;

using ReviveDeadCrops.Framework;

using StardewModdingAPI.Events;

using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace ReviveDeadCrops;

/// <inheritdoc />
internal sealed class ModEntry : Mod
{
    private static IJsonAssetsAPI? jaAPI;

    private static int everlastingID = -1;

    /// <summary>
    /// Gets the id of the everlasting fertilizer.
    /// </summary>
    internal static int EverlastingID
    {
        get
        {
            if (everlastingID == -1)
            {
                everlastingID = jaAPI?.GetObjectId("Everlasting Fertilizer - More Fertilizers") ?? -1;
            }

            return everlastingID;
        }
    }

    /// <summary>
    /// Gets the logging instance for this mod.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; } = null!;

    /// <summary>
    /// Gets the API for this mod.
    /// </summary>
    internal static ReviveDeadCropsApi Api { get; } = ReviveDeadCropsApi.Instance;

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        ModMonitor = this.Monitor;

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        helper.Events.GameLoop.DayEnding += this.OnDayEnd;

        helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;

        this.Monitor.Log($"Starting up: {this.ModManifest.UniqueID} - {typeof(ModEntry).Assembly.FullName}");

        this.ApplyPatches(new Harmony(this.ModManifest.UniqueID));
    }

    /// <inheritdoc />
    [UsedImplicitly]
    public override object? GetApi() => Api;

    /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        IntegrationHelper helper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, LogLevel.Trace);
        _ = helper.TryGetAPI("spacechase0.JsonAssets", "1.10.3", out jaAPI);
    }

    /// <inheritdoc cref="IGameLoopEvents.ReturnedToTitle"/>
    private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
        => everlastingID = -1;

    // we need to make sure to slot in before Solid Foundations removes its buildings.
    [EventPriority(EventPriority.High + 20)]
    private void OnDayEnd(object? sender, DayEndingEventArgs e)
    {
        if (!Api.Changed)
        {
            return;
        }
        Api.Changed = false;

        Utility.ForAllLocations(
            (location) =>
            {
                foreach (TerrainFeature terrain in location.terrainFeatures.Values)
                {
                    if (terrain is HoeDirt dirt && dirt.modData?.GetBool(ReviveDeadCropsApi.REVIVED_PLANT_MARKER) == true
                        && dirt.crop is not null && dirt.fertilizer.Value != EverlastingID)
                    {
                        this.Monitor.DebugOnlyLog($"Found dirt with marker at {dirt.currentTileLocation} with crop {dirt.crop?.indexOfHarvest ?? -1}");
                        dirt.modData.SetBool(ReviveDeadCropsApi.REVIVED_PLANT_MARKER, false, false);
                        dirt.crop?.Kill();
                    }
                }

                foreach (SObject obj in location.Objects.Values)
                {
                    if (obj is IndoorPot pot && pot.hoeDirt.Value is HoeDirt dirt
                        && dirt.modData?.GetBool(ReviveDeadCropsApi.REVIVED_PLANT_MARKER) == true
                        && dirt.crop is not null && dirt.fertilizer.Value != EverlastingID)
                    {
                        this.Monitor.DebugOnlyLog($"Found dirt with marker at {dirt.currentTileLocation} with crop {dirt.crop?.indexOfHarvest ?? -1}");
                        dirt.modData.SetBool(ReviveDeadCropsApi.REVIVED_PLANT_MARKER, false, false);
                        dirt.crop?.Kill();
                    }
                }
            });
    }

    /// <summary>
    /// Applies the patches for this mod.
    /// </summary>
    /// <param name="harmony">This mod's harmony instance.</param>
    private void ApplyPatches(Harmony harmony)
    {
        try
        {
            harmony.PatchAll(typeof(ModEntry).Assembly);
        }
        catch (Exception ex)
        {
            ModMonitor.Log(string.Format(ErrorMessageConsts.HARMONYCRASH, ex), LogLevel.Error);
        }
        harmony.Snitch(this.Monitor, this.ModManifest.UniqueID, transpilersOnly: true);
    }

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (!(e.Button.IsUseToolButton() || e.Button.IsActionButton())
            || !MenuingExtensions.IsNormalGameplay())
        {
            return;
        }

        if (Game1.player.ActiveObject is SObject obj && Api.TryApplyDust(Game1.currentLocation, e.Cursor.GrabTile, obj))
        {
            this.Helper.Input.Suppress(e.Button);
            Game1.player.reduceActiveItemByOne();
        }
    }
}
