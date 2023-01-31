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
using HarmonyLib;
using StardewModdingAPI.Events;
using AtraUtils = AtraShared.Utils.Utils;

namespace LessMiniShippingBin;

/// <inheritdoc />
internal sealed class ModEntry : Mod
{
    // ModMonitor is set in Entry, which is as close I can reasonaby get to the constructor.

    /// <summary>
    /// Gets logger for this mod.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; } = null!;

    /// <summary>
    /// Gets or sets an instance of the configuration class for this mod.
    /// </summary>
    internal static ModConfig Config { get; set; } = null!;

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        ModMonitor = this.Monitor;
        I18n.Init(helper.Translation);
        Config = AtraUtils.GetConfigOrDefault<ModConfig>(helper, this.Monitor);
        this.ApplyPatches(new Harmony(this.ModManifest.UniqueID));
        helper.Events.GameLoop.GameLaunched += this.SetUpConfig;

        this.Monitor.Log($"Starting up: {this.ModManifest.UniqueID} - {typeof(ModEntry).Assembly.FullName}");
    }

    /// <summary>
    /// Applies and logs this mod's harmony patches.
    /// </summary>
    /// <param name="harmony">My harmony instance.</param>
    private void ApplyPatches(Harmony harmony)
    {
        // handle patches from annotations.
        harmony.PatchAll(typeof(ModEntry).Assembly);
    }

    /// <summary>
    /// Generates the GMCM for this mod.
    /// </summary>
    /// <param name="sender">SMAPI.</param>
    /// <param name="e">Arguments for event.</param>
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
