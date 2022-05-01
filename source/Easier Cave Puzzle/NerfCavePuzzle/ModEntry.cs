/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/NerfCavePuzzle
**
*************************************************/

using System.Reflection;
using AtraShared.Integrations;
using AtraShared.MigrationManager;
using AtraShared.Utils;
using AtraShared.Utils.Extensions;
using HarmonyLib;
using NerfCavePuzzle.HarmonyPatches;
using StardewModdingAPI.Events;

namespace NerfCavePuzzle;

/// <inheritdoc />
internal class ModEntry : Mod
{
    private MigrationManager? migrator = null;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    /// <summary>
    /// Gets the logger for this file.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; }

    /// <summary>
    /// Gets the data helper for this mod.
    /// </summary>
    internal static IDataHelper DataHelper { get; private set; }

    /// <summary>
    /// Gets the multiplayer helper for this mod.
    /// </summary>
    internal static IMultiplayerHelper MultiplayerHelper { get; private set; }

    /// <summary>
    /// Gets the uniqueID of this mod.
    /// </summary>
    internal static string UniqueID { get; private set; }

    /// <summary>
    /// Gets the configuration class for this mod.
    /// </summary>
    internal static ModConfig Config { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        ModMonitor = this.Monitor;
        DataHelper = helper.Data;
        MultiplayerHelper = helper.Multiplayer;
        UniqueID = this.ModManifest.UniqueID;

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;

        helper.Events.Multiplayer.ModMessageReceived += this.MultiMessageRecieved;

        try
        {
            Config = this.Helper.ReadConfig<ModConfig>();
        }
        catch
        {
            this.Monitor.Log(I18n.IllFormatedConfig(), LogLevel.Warn);
            Config = new();
        }
        this.ApplyPatches(new Harmony(UniqueID));
    }

    private void MultiMessageRecieved(object? sender, ModMessageReceivedEventArgs e)
        => IslandCaveWestDifficultyTranspiler.HandleSaveFromMultiplayer(e);

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        GMCMHelper helper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, this.ModManifest);
        if (!helper.TryGetAPI())
        {
            return;
        }
        helper.Register(() => Config = new(), () => this.Helper.WriteConfig(Config))
            .AddParagraph(I18n.ModDescription);
        foreach (PropertyInfo property in typeof(ModConfig).GetProperties())
        {
            if (property.PropertyType == typeof(bool))
            {
                helper.AddBoolOption(property, () => Config);
            }
            else if (property.PropertyType == typeof(float))
            {
                helper.AddFloatOption(property, () => Config, min: 0.1f, max: 10f, interval: 0.1f);
            }
            else if (property.PropertyType == typeof(int))
            {
                helper.AddIntOption(property, () => Config, min: 5, max: 7);
            }
        }
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        MultiplayerHelpers.AssertMultiplayerVersions(this.Helper.Multiplayer, this.ModManifest, this.Monitor, this.Helper.Translation);
        this.migrator = new(this.ModManifest, this.Helper, this.Monitor);
        this.migrator.ReadVersionInfo();

        this.Helper.Events.GameLoop.Saved += this.WriteMigrationData;
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
            harmony.PatchAll();
            CaveCrystalTranspiler.ApplyPatch(harmony);
        }
        catch (Exception ex)
        {
            ModMonitor.Log($"Mod failed while applying patches:\n{ex}", LogLevel.Error);
        }
        harmony.Snitch(this.Monitor, UniqueID, transpilersOnly: true);
    }
}