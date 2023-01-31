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
using AtraShared.Integrations;
using AtraShared.MigrationManager;
using AtraShared.Utils.Extensions;
using HarmonyLib;
using NerfCavePuzzle.HarmonyPatches;
using StardewModdingAPI.Events;

using AtraUtils = AtraShared.Utils.Utils;

namespace NerfCavePuzzle;

/// <inheritdoc />
internal sealed class ModEntry : Mod
{
    private MigrationManager? migrator = null;

    /// <summary>
    /// Gets the logger for this file.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; } = null!;

    /// <summary>
    /// Gets the data helper for this mod.
    /// </summary>
    internal static IDataHelper DataHelper { get; private set; } = null!;

    /// <summary>
    /// Gets the multiplayer helper for this mod.
    /// </summary>
    internal static IMultiplayerHelper MultiplayerHelper { get; private set; } = null!;

    /// <summary>
    /// Gets the uniqueID of this mod.
    /// </summary>
    internal static string UniqueID { get; private set; } = null!;

    /// <summary>
    /// Gets the configuration class for this mod.
    /// </summary>
    internal static ModConfig Config { get; private set; } = null!;

    /// <summary>
    /// Gets the input helper for this mod.
    /// </summary>
    internal static IInputHelper InputHelper { get; private set; } = null!;

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        ModMonitor = this.Monitor;
        DataHelper = helper.Data;
        MultiplayerHelper = helper.Multiplayer;
        InputHelper = helper.Input;
        UniqueID = this.ModManifest.UniqueID;

        this.Monitor.Log($"Starting up: {this.ModManifest.UniqueID} - {typeof(ModEntry).Assembly.FullName}");

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;

        helper.Events.Multiplayer.ModMessageReceived += this.MultiMessageRecieved;

        Config = AtraUtils.GetConfigOrDefault<ModConfig>(helper, this.Monitor);
        this.ApplyPatches(new Harmony(UniqueID));
    }

    private void MultiMessageRecieved(object? sender, ModMessageReceivedEventArgs e)
        => IslandCaveWestDifficultyTranspiler.HandleSaveFromMultiplayer(e);

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        GMCMHelper helper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, this.ModManifest);
        if (helper.TryGetAPI())
        {
            helper.Register(
                static () => Config = new(),
                () => this.Helper.AsyncWriteConfig(this.Monitor, Config))
            .AddParagraph(I18n.ModDescription)
            .GenerateDefaultGMCM(static () => Config);
        }
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

    private void ApplyPatches(Harmony harmony)
    {
        try
        {
            harmony.PatchAll(typeof(ModEntry).Assembly);
            CaveCrystalTranspiler.ApplyPatch(harmony);
        }
        catch (Exception ex)
        {
            ModMonitor.Log($"Mod failed while applying patches:\n{ex}", LogLevel.Error);
        }
        harmony.Snitch(this.Monitor, UniqueID, transpilersOnly: true);
    }
}