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
using AtraShared.Utils.Extensions;
using HarmonyLib;
using StardewModdingAPI.Events;
using AtraUtils = AtraShared.Utils.Utils;

namespace EasierDartPuzzle;

/// <inheritdoc/>
internal sealed class ModEntry : Mod
{
    /// <summary>
    /// Gets the logger for this file.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; } = null!;

    /// <summary>
    /// Gets the config instance for this mod.
    /// </summary>
    internal static ModConfig Config { get; private set; } = null!;

    /// <inheritdoc/>
    public override void Entry(IModHelper helper)
    {
        ModMonitor = this.Monitor;
        I18n.Init(helper.Translation);
        AssetManager.Initialize(helper.GameContent);
        Config = AtraUtils.GetConfigOrDefault<ModConfig>(helper, this.Monitor);
        if (Config.MinDartCount > Config.MaxDartCount)
        {
            (Config.MinDartCount, Config.MaxDartCount) = (Config.MaxDartCount, Config.MinDartCount);
            helper.AsyncWriteConfig(this.Monitor, Config);
        }

        this.Monitor.Log($"Starting up: {this.ModManifest.UniqueID} - {typeof(ModEntry).Assembly.FullName}");

        this.ApplyPatches(new Harmony(this.ModManifest.UniqueID));

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunch;

        helper.Events.Content.AssetRequested += static (_, e) => AssetManager.Apply(e);
    }

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
        harmony.Snitch(this.Monitor, harmony.Id, transpilersOnly: true);
    }

    /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
    private void OnGameLaunch(object? sender, GameLaunchedEventArgs e)
    {
        GMCMHelper helper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, this.ModManifest);
        if (helper.TryGetAPI())
        {
            helper.Register(
                reset: static () => Config = new(),
                save: () =>
                {
                    if (Config.MinDartCount > Config.MaxDartCount)
                    {
                        (Config.MinDartCount, Config.MaxDartCount) = (Config.MaxDartCount, Config.MinDartCount);
                    }
                    this.Helper.AsyncWriteConfig(this.Monitor, Config);
                })
            .AddParagraph(I18n.ModDescription)
            .GenerateDefaultGMCM(static () => Config);
        }
    }
}