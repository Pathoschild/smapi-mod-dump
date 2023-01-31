/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.Integrations;
using AtraShared.Utils.Extensions;

using FarmBuildingHelper.Framework;

using StardewModdingAPI.Events;

using StardewValley.Menus;

using AtraUtils = AtraShared.Utils.Utils;

namespace FarmBuildingHelper;

/// <inheritdoc />
internal sealed class ModEntry : Mod
{
    /// <summary>
    /// Gets the logger for this mod.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; } = null!;

    /// <summary>
    /// Gets the config instance for this mod.
    /// </summary>
    internal static ModConfig Config { get; private set; } = null!;

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        ModMonitor = this.Monitor;
        Config = AtraUtils.GetConfigOrDefault<ModConfig>(helper, this.Monitor);
        this.Monitor.Log($"Starting up: {this.ModManifest.UniqueID} - {typeof(ModEntry).Assembly.FullName}");

        helper.Events.Display.MenuChanged += this.OnMenuChanged;
        helper.Events.GameLoop.GameLaunched += this.SetUpConfig;
    }

    private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
    {
        if (e.NewMenu is CarpenterMenu or PurchaseAnimalsMenu)
        {
            ModMonitor.Log("Menu added", LogLevel.Alert);
        }
        else if (e.OldMenu is CarpenterMenu or PurchaseAnimalsMenu)
        {
            ModMonitor.Log("Menu left", LogLevel.Alert);
        }
    }

    /// <summary>
    /// Generates the GMCM for this mod by looking at the structure of the config class.
    /// </summary>
    /// <param name="sender">Unknown, expected by SMAPI.</param>
    /// <param name="e">Arguments for event.</param>
    /// <remarks>To add a new setting, add the details to the i18n file. Currently handles: bool.</remarks>
    private void SetUpConfig(object? sender, GameLaunchedEventArgs e)
    {
        GMCMHelper helper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, this.ModManifest);
        if (helper.TryGetAPI())
        {
            helper.Register(
                reset: static () => Config = new ModConfig(),
                save: () => this.Helper.AsyncWriteConfig(this.Monitor, Config))
            .AddParagraph(I18n.Mod_Description)
            .GenerateDefaultGMCM(static () => Config);
        }
    }
}
