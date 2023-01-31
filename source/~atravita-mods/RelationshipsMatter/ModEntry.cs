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
using AtraShared.Schedules;
using AtraShared.Utils.Extensions;

using HarmonyLib;

using StardewModdingAPI.Events;

using AtraUtils = AtraShared.Utils.Utils;

namespace RelationshipsMatter;

/// <inheritdoc/>
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

    /// <summary>
    /// Gets the scheduler instance for this mod.
    /// </summary>
    internal static ScheduleUtilityFunctions Scheduler { get; private set; } = null!;

    /// <inheritdoc/>
    public override void Entry(IModHelper helper)
    {
        ModMonitor = this.Monitor;
        I18n.Init(helper.Translation);
        RMUtils.Init(helper.GameContent);
        Config = AtraUtils.GetConfigOrDefault<ModConfig>(helper, this.Monitor);
        Scheduler = new(this.Monitor, this.Helper.Translation);

        this.Monitor.Log($"Starting up: {this.ModManifest.UniqueID} - {typeof(ModEntry).Assembly.FullName}");

        helper.Events.GameLoop.GameLaunched += this.SetUpConfig;
        helper.Events.Content.AssetsInvalidated += (_, e) => RMUtils.Reset(e.NamesWithoutLocale);
        this.ApplyPatches(new(this.ModManifest.UniqueID));
    }

    /// <summary>
    /// Sets up the GMCM for this mod.
    /// </summary>
    /// <param name="sender">SMAPI.</param>
    /// <param name="e">event args.</param>
    private void SetUpConfig(object? sender, GameLaunchedEventArgs e)
    {
        GMCMHelper helper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, this.ModManifest);
        if (helper.TryGetAPI())
        {
            helper.Register(
                reset: static () => Config = new(),
                save: () => this.Helper.AsyncWriteConfig(this.Monitor, Config))
            .AddParagraph(I18n.ModDescription)
            .GenerateDefaultGMCM(static () => Config);
        }
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
}
