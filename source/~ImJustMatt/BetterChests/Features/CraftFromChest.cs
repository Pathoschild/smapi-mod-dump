/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Features;

using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewMods.BetterChests.Helpers;
using StardewMods.BetterChests.Models;
using StardewMods.BetterChests.StorageHandlers;
using StardewMods.Common.Enums;
using StardewMods.Common.Integrations.BetterChests;
using StardewValley.Locations;
using StardewValley.Menus;

/// <summary>
///     Craft using items from placed chests and chests in the farmer's inventory.
/// </summary>
internal class CraftFromChest : IFeature
{
    private const int MaxTimeOut = 60;

    private static CraftFromChest? Instance;

    private readonly ModConfig _config;
    private readonly PerScreen<int> _currentTab = new();
    private readonly PerScreen<List<IStorageObject>> _eligible = new(() => new());
    private readonly IModHelper _helper;
    private readonly PerScreen<List<IStorageObject>> _locked = new(() => new());
    private readonly PerScreen<int> _timeOut = new();

    private bool _isActivated;

    private CraftFromChest(IModHelper helper, ModConfig config)
    {
        this._helper = helper;
        this._config = config;
    }

    private static IEnumerable<IStorageObject> Eligible =>
        from storage in Storages.All
        where storage.CraftFromChest is not (FeatureOptionRange.Disabled or FeatureOptionRange.Default)
           && storage.CraftFromChestDisableLocations?.Contains(Game1.player.currentLocation.Name) != true
           && !(storage.CraftFromChestDisableLocations?.Contains("UndergroundMine") == true
             && Game1.player.currentLocation is MineShaft mineShaft
             && mineShaft.Name.StartsWith("UndergroundMine"))
           && storage.Source is not null
           && storage.CraftFromChest.WithinRangeOfPlayer(
                  storage.CraftFromChestDistance,
                  storage.Source,
                  storage.Position)
        select storage;

    private List<IStorageObject> CurrentEligible => this._eligible.Value;

    private int CurrentTab
    {
        get => this._currentTab.Value;
        set => this._currentTab.Value = value;
    }

    private List<IStorageObject> LockedEligible => this._locked.Value;

    private int TimeOut
    {
        get => this._timeOut.Value;
        set => this._timeOut.Value = value;
    }

    /// <summary>
    ///     Initializes <see cref="CraftFromChest" />.
    /// </summary>
    /// <param name="helper">SMAPI helper for events, input, and content.</param>
    /// <param name="config">Mod config data.</param>
    /// <returns>Returns an instance of the <see cref="CraftFromChest" /> class.</returns>
    public static CraftFromChest Init(IModHelper helper, ModConfig config)
    {
        return CraftFromChest.Instance ??= new(helper, config);
    }

    /// <inheritdoc />
    public void Activate()
    {
        if (this._isActivated)
        {
            return;
        }

        this._isActivated = true;
        this._helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        this._helper.Events.GameLoop.UpdateTicking += this.OnUpdateTicking;
        this._helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;

        if (Integrations.ToolbarIcons.IsLoaded)
        {
            Integrations.ToolbarIcons.API.AddToolbarIcon(
                "BetterChests.CraftFromChest",
                "furyx639.BetterChests/Icons",
                new(32, 0, 16, 16),
                I18n.Button_CraftFromChest_Name());
            Integrations.ToolbarIcons.API.ToolbarIconPressed += this.OnToolbarIconPressed;
        }

        if (Integrations.BetterCrafting.IsLoaded)
        {
            Integrations.BetterCrafting.API.RegisterInventoryProvider(typeof(StorageWrapper), new StorageProvider());
        }
    }

    /// <inheritdoc />
    public void Deactivate()
    {
        if (!this._isActivated)
        {
            return;
        }

        this._isActivated = false;
        this._helper.Events.GameLoop.UpdateTicked -= this.OnUpdateTicked;
        this._helper.Events.GameLoop.UpdateTicking -= this.OnUpdateTicking;
        this._helper.Events.Input.ButtonsChanged -= this.OnButtonsChanged;

        if (Integrations.ToolbarIcons.IsLoaded)
        {
            Integrations.ToolbarIcons.API.RemoveToolbarIcon("BetterChests.CraftFromChest");
            Integrations.ToolbarIcons.API.ToolbarIconPressed -= this.OnToolbarIconPressed;
        }

        if (Integrations.BetterCrafting.IsLoaded)
        {
            Integrations.BetterCrafting.API.UnregisterInventoryProvider(typeof(StorageWrapper));
        }
    }

    private void ExitFunction()
    {
        foreach (var storage in this.LockedEligible)
        {
            storage.Mutex?.ReleaseLock();
        }

        this.LockedEligible.Clear();
    }

    private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Context.IsPlayerFree || !this._config.ControlScheme.OpenCrafting.JustPressed())
        {
            return;
        }

        this._helper.Input.SuppressActiveKeybinds(this._config.ControlScheme.OpenCrafting);
        this.OpenCrafting();
    }

    private void OnToolbarIconPressed(object? sender, string id)
    {
        if (id == "BetterChests.CraftFromChest")
        {
            this.OpenCrafting();
        }
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (this.TimeOut == 0 || --this.TimeOut != 0)
        {
            return;
        }

        foreach (var storage in this.CurrentEligible)
        {
            storage.Mutex?.ReleaseLock();
        }

        this.CurrentEligible.Clear();
        this.TimeOut = 0;
        var width = 800 + IClickableMenu.borderWidth * 2;
        var height = 600 + IClickableMenu.borderWidth * 2;
        var (x, y) = Utility.getTopLeftPositionForCenteringOnScreen(width, height).ToPoint();
        Game1.activeClickableMenu = new CraftingPage(
            x,
            y,
            width,
            height,
            false,
            true,
            this.LockedEligible.OfType<ChestStorage>().Select(storage => storage.Chest).ToList())
        {
            exitFunction = this.ExitFunction,
        };
    }

    private void OnUpdateTicking(object? sender, UpdateTickingEventArgs e)
    {
        foreach (var storage in this.CurrentEligible)
        {
            storage.Mutex?.Update(storage.Source as GameLocation ?? Game1.currentLocation);
        }

        if (Game1.activeClickableMenu is not GameMenu { currentTab: var currentTab } gameMenu
         || currentTab == this.CurrentTab)
        {
            return;
        }

        this.CurrentTab = currentTab;
        if (gameMenu.pages[currentTab] is not CraftingPage craftingPage)
        {
            return;
        }

        craftingPage._materialContainers ??= new();
        craftingPage._materialContainers.AddRange(
            CraftFromChest.Eligible.OfType<ChestStorage>().Select(storage => storage.Chest));
        craftingPage._materialContainers = craftingPage._materialContainers.Distinct().ToList();
    }

    private void OpenCrafting()
    {
        this.CurrentEligible.Clear();
        this.CurrentEligible.AddRange(CraftFromChest.Eligible);
        if (!this.CurrentEligible.Any())
        {
            Game1.showRedMessage(I18n.Alert_CraftFromChest_NoEligible());
            return;
        }

        if (Integrations.BetterCrafting.IsLoaded)
        {
            Integrations.BetterCrafting.API.OpenCraftingMenu(
                false,
                false,
                null,
                null,
                null,
                false,
                this.CurrentEligible.Select(
                        storage => new Tuple<object, GameLocation>(
                            new StorageWrapper(storage),
                            storage.Source as GameLocation ?? Game1.currentLocation))
                    .ToList());
            return;
        }

        this.TimeOut = CraftFromChest.MaxTimeOut;
        for (var index = this.CurrentEligible.Count - 1; index >= 0; index--)
        {
            var storage = this.CurrentEligible[index];
            storage.Mutex?.RequestLock(
                () => this.LockedEligible.Add(storage),
                () => this.CurrentEligible.Remove(storage));
        }
    }
}