/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/TrashDoesNotConsumeBait
**
*************************************************/

using System.Reflection;
using AtraShared.Integrations;
using AtraShared.MigrationManager;
using AtraShared.Utils.Extensions;
using HarmonyLib;
using StardewModdingAPI.Events;

namespace TrashDoesNotConsumeBait;

/// <inheritdoc/>
internal class ModEntry : Mod
{
    private MigrationManager? migrator;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    /// <summary>
    /// Gets the logger for this mod.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; }

    /// <summary>
    /// Gets or sets the config instance for this mod.
    /// </summary>
    internal static ModConfig Config { get; set; }

    /// <summary>
    /// Gets the content helper for this mod.
    /// </summary>
    internal static IContentHelper ContentHelper { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <inheritdoc/>
    public override void Entry(IModHelper helper)
    {
        ModMonitor = this.Monitor;
        ContentHelper = helper.Content;
        I18n.Init(helper.Translation);

        try
        {
            Config = this.Helper.ReadConfig<ModConfig>();
        }
        catch
        {
            this.Monitor.Log(I18n.IllFormatedConfig(), LogLevel.Warn);
            Config = new();
        }

        helper.Events.GameLoop.GameLaunched += this.SetUpConfig;
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        this.ApplyPatches(new Harmony(this.ModManifest.UniqueID));

        helper.Content.AssetEditors.Add(AssetEditor.Instance);
    }

    /// <summary>
    /// Sets up the GMCM for this mod.
    /// </summary>
    /// <param name="sender">SMAPI.</param>
    /// <param name="e">event args.</param>
    private void SetUpConfig(object? sender, GameLaunchedEventArgs e)
    {
        GMCMHelper helper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, this.ModManifest);
        if (!helper.TryGetAPI())
        {
            return;
        }
        helper.Register(
            reset: () =>
            {
                Config = new();
                AssetEditor.Invalidate();
            },
            save: () => {
                this.Helper.WriteConfig(Config);
                AssetEditor.Invalidate();
            });
        foreach (PropertyInfo property in typeof(ModConfig).GetProperties())
        {

            if (property.PropertyType == typeof(bool))
            {
                helper.AddBoolOption(
                    property: property,
                    getConfig: () => Config);
            }
        }
        helper.AddPageHere("CheatyStuff", I18n.CheatyStuffTitle);
        foreach (PropertyInfo property in typeof(ModConfig).GetProperties())
        {
            if (property.PropertyType == typeof(float))
            {
                helper.AddFloatOption(
                    property: property,
                    getConfig: () => Config,
                    min: 0f,
                    max: 1f,
                    interval: 0.01f);
            }
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
            harmony.PatchAll();
        }
        catch (Exception ex)
        {
            ModMonitor.Log($"Mod crashed while applying harmony patches:\n\n{ex}", LogLevel.Error);
        }
        harmony.Snitch(this.Monitor, this.ModManifest.UniqueID);
    }

    /// <summary>
    /// Sets up the migrator on save loaded.
    /// </summary>
    /// <param name="sender">SMAPI.</param>
    /// <param name="e">Save loaded event arguments.</param>
    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
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

}