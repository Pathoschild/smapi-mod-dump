/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.MigrationManager;
using PelicanTownFoodBank.Models;
using StardewModdingAPI.Events;

namespace PelicanTownFoodBank;

/// <inheritdoc />
internal class ModEntry : Mod
{
    private MigrationManager? migrator;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    /// <summary>
    /// Gets the logger for this mod.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; }

    /// <summary>
    /// Gets the game content helper for this mod.
    /// </summary>
    internal static IGameContentHelper GameContentHelper { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        ModMonitor = this.Monitor;
        GameContentHelper = helper.GameContent;
#if DEBUG
        helper.Events.Input.ButtonPressed += this.OnButtonPressed;
#endif

        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        helper.Events.Content.LocaleChanged += this.OnLocaleChanged;
        helper.Events.Content.AssetRequested += this.OnAssetRequested;
    }

#if DEBUG
    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (e.Button is not SButton.K || !Context.IsWorldReady || PantryStockManager.Sellables is null)
        {
            return;
        }
        Game1.activeClickableMenu = PantryStockManager.GetFoodBankMenu();
    }
#endif

    private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        => AssetManager.Load(e);

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
        => PantryStockManager.Reset();

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

    private void OnLocaleChanged(object? sender, LocaleChangedEventArgs e)
    => FoodBankMenu.OnLocaleChange();
}