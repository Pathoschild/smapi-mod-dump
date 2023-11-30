/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/thakyZ/StardewValleyMods
**
*************************************************/

using System.Diagnostics.CodeAnalysis;

using GenericModConfigMenu;

using HarmonyLib;

using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace NoPauseWhenInactiveGlobal;

/// <inheritdoc />
public class ModEntry : Mod
{
    /// <summary>
    /// This mod's config instance.
    /// </summary>
    [NotNull, AllowNull]
    internal static ModConfig Config { get; private set; }

    /// <summary>
    /// The Logging function for the mod.
    /// </summary>
    [NotNull, AllowNull]
    internal static IMonitor Logger { get; private set; }

    /// <summary>
    /// The SMAPI mod helper instance.
    /// </summary>
    [NotNull, AllowNull]
    internal static IModHelper ModHelper { get; private set; }

    /// <summary>
    /// A bool to check if a save game has been loaded or not.
    /// </summary>
    internal static bool IsSaveLoaded { get; private set; } = false;

    /// <inheritdoc />
    public override void Entry(IModHelper modHelper)
    {
        ModHelper = modHelper;
        Logger = this.Monitor;
        I18n.Init(modHelper.Translation);
        Config = ModHelper.ReadConfig<ModConfig>();
        modHelper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        modHelper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        modHelper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;
        // Patch all harmony methods via attributes found in the same namespace or descendants.
        new Harmony("NekoBoiNick.NoPauseWhenInactiveGlobal").PatchAll();
    }

    /// <summary>
    /// Sets the property <see cref="IsSaveLoaded"/> to <see cref="false"/>, when returning back to the main menu.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
    {
        IsSaveLoaded = false;
    }

    /// <summary>
    /// Sets the property <see cref="IsSaveLoaded"/> to <see cref="true"/>, when loading a save game.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        IsSaveLoaded = true;
    }

    /// <summary>
    /// Implements the GenericModConfig options menu if it is loaded into the game.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        // get Generic Mod Config Menu's API (if it's installed)
        var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is not null)
        {
            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(Config),
                titleScreenOnly: true
            );

            // Add the config option to disable pausing of the game outside of a save.
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => ModHelper.Translation.Get("nopausewheninative.global.option.name"),
                tooltip: () => ModHelper.Translation.Get("nopausewheninative.global.option.tooltip"),
                getValue: () => Config.DisableGamePause,
                setValue: value => Config.DisableGamePause = value
            );
        }
    }
}
