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
using AtraShared.Menuing;
using AtraShared.Utils.Extensions;

using HarmonyLib;

using Microsoft.Xna.Framework;

using StardewModdingAPI.Events;

using StardewValley.TerrainFeatures;

using TapGiantCrops.Framework;
using TapGiantCrops.HarmonyPatches;

namespace TapGiantCrops;

/// <inheritdoc />
[HarmonyPatch(typeof(Utility))]
internal sealed class ModEntry : Mod
{
    private static readonly TapGiantCrop Api = new();

    /// <summary>
    /// Gets the logger for this mod.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; } = null!;

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        ModMonitor = this.Monitor;
        AssetManager.Initialize(helper.GameContent);

        helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        helper.Events.GameLoop.DayEnding += this.OnDayEnding;

        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;

        helper.Events.Content.AssetRequested += static (_, e) => AssetManager.Load(e);
        helper.Events.Content.AssetsInvalidated += static (_, e) => AssetManager.Reset(e.NamesWithoutLocale);

        this.Monitor.Log($"Starting up: {this.ModManifest.UniqueID} - {typeof(ModEntry).Assembly.FullName}");

        this.ApplyPatches(new Harmony(this.ModManifest.UniqueID));
    }

    /// <inheritdoc />
    public override object? GetApi() => Api;

    /// <inheritdoc cref="IGameLoopEvents.DayEnding"/>
    private void OnDayEnding(object? sender, DayEndingEventArgs e)
    {
        Utility.ForAllLocations((location) =>
        {
            if (location?.resourceClumps is null)
            {
                return;
            }

            foreach (ResourceClump? feature in location.resourceClumps)
            {
                if (feature is GiantCrop crop)
                {
                    Vector2 offset = crop.tile.Value;
                    offset.X += crop.width.Value / 2;
                    offset.Y += crop.height.Value - 1;
                    if (location.objects.TryGetValue(offset, out SObject? tapper) && tapper.Name.Contains("Tapper", StringComparison.Ordinal)
                        && tapper.heldObject is not null && tapper.heldObject.Value is null)
                    {
                        (SObject obj, int days)? output = Api.GetTapperProduct(crop, tapper);
                        if (output is not null)
                        {
                            tapper.heldObject.Value = output.Value.obj;
                            int days = output.Value.days;
                            tapper.MinutesUntilReady = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, Math.Max(1, days));
                            this.Monitor.DebugOnlyLog($"Assigning product to tapper at {location.NameOrUniqueName} {offset}", LogLevel.Info);
                        }
                    }
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

            if (new Version(1, 6) > new Version(Game1.version) &&
                !this.Helper.ModRegistry.IsLoaded("atravita.GrowableGiantCrops") &&
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
        harmony.Snitch(this.Monitor, this.ModManifest.UniqueID, transpilersOnly: true);
    }

    /// <inheritdoc cref="IInputEvents.ButtonPressed"/>
    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (!(e.Button.IsUseToolButton() || e.Button.IsActionButton())
            || !MenuingExtensions.IsNormalGameplay())
        {
            return;
        }
        if (Game1.player.ActiveObject is SObject obj && Api.TryPlaceTapper(Game1.currentLocation, e.Cursor.GrabTile, obj))
        {
            Game1.player.reduceActiveItemByOne();
        }
    }

    /// <inheritdoc cref="IGameLoopEvents.SaveLoaded"/>
    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e) => Api.Init();

    [HarmonyPriority(Priority.High)]
    [HarmonyPatch(nameof(Utility.playerCanPlaceItemHere))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony Convention")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "Reviewed.")]
    private static bool Prefix(GameLocation location, Item item, int x, int y, Farmer f, ref bool __result)
    {
        try
        {
            if (Utility.withinRadiusOfPlayer(x, y, 2, f) && item is SObject obj)
            {
                Vector2 tile = new(MathF.Floor(x / 64f), MathF.Floor(y / 64f));
                if (Api.CanPlaceTapper(location, tile, obj))
                {
                    __result = true;
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            ModMonitor.Log($"Attempt to prefix Utility.playerCanPlaceItemHere has failed:\n\n{ex}", LogLevel.Error);
        }
        return true;
    }
}
