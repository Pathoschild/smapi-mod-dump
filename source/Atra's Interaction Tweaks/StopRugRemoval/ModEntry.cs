/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StopRugRemoval
**
*************************************************/

using System.Reflection;
using AtraShared.Integrations;
using AtraShared.MigrationManager;
using AtraShared.Utils.Extensions;
using HarmonyLib;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StopRugRemoval.Configuration;
using StopRugRemoval.HarmonyPatches;
using StopRugRemoval.HarmonyPatches.BombHandling;

namespace StopRugRemoval;

/// <summary>
/// Entry class to the mod.
/// </summary>
public class ModEntry : Mod
{
    private static readonly Lazy<IReflectedField<Multiplayer>> MultiplayerLazy = new(() => ReflectionHelper!.GetField<Multiplayer>(typeof(Game1), "multiplayer"));

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:Field names should begin with lower-case letter", Justification = "Reviewed.")]
    private static GMCMHelper? GMCM = null;

    private MigrationManager? migrator;

    

    /// <summary>
    /// Gets Game1.multiplayer.
    /// </summary>
    /// <remarks>This still requires reflection and is likely slow.</remarks>
    internal static Multiplayer Multiplayer => MultiplayerLazy.Value.GetValue();

    // the following three properties are set in the entry method, which is approximately as close as I can get to the constructor anyways.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    /// <summary>
    /// Gets the logger for this file.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; }

    /// <summary>
    /// Gets instance that holds the configuration for this mod.
    /// </summary>
    internal static ModConfig Config { get; private set; }

    /// <summary>
    /// Gets the reflection helper for this mod.
    /// </summary>
    internal static IReflectionHelper ReflectionHelper { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <inheritdoc/>
    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        ModMonitor = this.Monitor;
        ReflectionHelper = this.Helper.Reflection;
        try
        {
            Config = this.Helper.ReadConfig<ModConfig>();
        }
        catch
        {
            this.Monitor.Log(I18n.IllFormatedConfig(), LogLevel.Warn);
            Config = new();
        }

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunch;
        helper.Events.GameLoop.SaveLoaded += this.SaveLoaded;
        helper.Events.Player.Warped += this.Player_Warped;

        this.ApplyPatches(new Harmony(this.ModManifest.UniqueID));
    }

    private void Player_Warped(object? sender, WarpedEventArgs e)
        => ConfirmBomb.HaveConfirmed.Value = false;

    /// <summary>
    /// Applies and logs this mod's harmony patches.
    /// </summary>
    /// <param name="harmony">My harmony instance.</param>
    private void ApplyPatches(Harmony harmony)
    {
        try
        {
            // handle patches from annotations.
            harmony.PatchAll();
        }
        catch (Exception ex)
        {
            ModMonitor.Log($"Mod crashed while applying harmony patches\n\n{ex}", LogLevel.Error);
        }
        harmony.Snitch(this.Monitor, this.ModManifest.UniqueID);
    }

    private void OnGameLaunch(object? sender, GameLaunchedEventArgs e)
    {
        PlantGrassUnder.GetSmartBuildingBuildMode(this.Helper.ModRegistry);
        GMCM = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, this.ModManifest);
        if (!GMCM.TryGetAPI())
        {
            return;
        }
        this.SetUpBasicConfig();
    }

    /// <summary>
    /// Raised when save is loaded.
    /// </summary>
    /// <param name="sender">Unknown, used by SMAPI.</param>
    /// <param name="e">Parameters.</param>
    private void SaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        if (Context.IsSplitScreen && Context.ScreenId != 0)
        {
            return;
        }

        // Have to wait until here to populate locations
        Config.PrePopulateLocations();
        this.Helper.WriteConfig(Config);

        this.migrator = new(this.ModManifest, this.Helper, this.Monitor);
        this.migrator.ReadVersionInfo();

        this.Helper.Events.GameLoop.Saved += this.WriteMigrationData;

        if (GMCM?.HasGottenAPI == true)
        {
            GMCM.Unregister();
            this.SetUpBasicConfig();
            GMCM.AddPageHere("Bombs", I18n.BombLocationDetailed)
                .AddParagraph(I18n.BombLocationDetailed_Description);

            foreach (GameLocation loc in Game1.locations)
            {
                GMCM.AddEnumOption(
                    name: () => loc.NameOrUniqueName,
                    getValue: () => Config.SafeLocationMap.TryGetValue(loc.NameOrUniqueName, out IsSafeLocationEnum val) ? val : IsSafeLocationEnum.Dynamic,
                    setValue: (value) => Config.SafeLocationMap[loc.NameOrUniqueName] = value);
            }
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

    private void SetUpBasicConfig()
    {
        GMCM!.Register(
                reset: () =>
                {
                    Config = new ModConfig();
                    Config.PrePopulateLocations();
                },
                save: () => this.Helper.WriteConfig(Config))
            .AddParagraph(I18n.Mod_Description);

        foreach (PropertyInfo property in typeof(ModConfig).GetProperties())
        {
            if (property.PropertyType == typeof(bool))
            {
                GMCM.AddBoolOption(property, () => Config);
            }
            else if (property.PropertyType == typeof(KeybindList))
            {
                GMCM.AddKeybindList(property, () => Config);
            }
        }
        GMCM!.AddSectionTitle(I18n.ConfirmBomb_Title)
            .AddParagraph(I18n.ConfirmBomb_Description)
            .AddEnumOption(
                name: I18n.InSafeAreas_Title,
                getValue: () => Config.InSafeAreas,
                setValue: (value) => Config.InSafeAreas = value,
                tooltip: I18n.InSafeAreas_Description)
            .AddEnumOption(
                name: I18n.InDangerousAreas_Title,
                getValue: () => Config.InDangerousAreas,
                setValue: (value) => Config.InDangerousAreas = value,
                tooltip: I18n.InDangerousAreas_Description);
    }
}
