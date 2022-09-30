/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework.Features;

using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI.Events;
using StardewMods.BetterChests.Framework.Handlers;
using StardewMods.BetterChests.Framework.Models;
using StardewMods.Common.Enums;
using StardewValley.Locations;

/// <summary>
///     Craft using items from placed chests and chests in the farmer's inventory.
/// </summary>
internal sealed class CraftFromChest : IFeature
{
#nullable disable
    private static IFeature Instance;
#nullable enable

    private readonly ModConfig _config;
    private readonly IModHelper _helper;

    private bool _isActivated;

    private CraftFromChest(IModHelper helper, ModConfig config)
    {
        this._helper = helper;
        this._config = config;
    }

    private static IEnumerable<BaseStorage> Eligible
    {
        get
        {
            foreach (var storage in Storages.All)
            {
                if (storage.CraftFromChest is not (FeatureOptionRange.Disabled or FeatureOptionRange.Default)
                 && !storage.CraftFromChestDisableLocations.Contains(Game1.player.currentLocation.Name)
                 && !(storage.CraftFromChestDisableLocations.Contains("UndergroundMine")
                   && Game1.player.currentLocation is MineShaft mineShaft
                   && mineShaft.Name.StartsWith("UndergroundMine"))
                 && storage.CraftFromChest.WithinRangeOfPlayer(
                        storage.CraftFromChestDistance,
                        storage.Location,
                        storage.Position))
                {
                    yield return storage;
                }
            }
        }
    }

    /// <summary>
    ///     Initializes <see cref="CraftFromChest" />.
    /// </summary>
    /// <param name="helper">SMAPI helper for events, input, and content.</param>
    /// <param name="config">Mod config data.</param>
    /// <returns>Returns an instance of the <see cref="CraftFromChest" /> class.</returns>
    public static IFeature Init(IModHelper helper, ModConfig config)
    {
        return CraftFromChest.Instance ??= new CraftFromChest(helper, config);
    }

    /// <inheritdoc />
    public void Activate()
    {
        if (this._isActivated)
        {
            return;
        }

        this._isActivated = true;
        BetterCrafting.CraftingStoragesLoading += CraftFromChest.OnCraftingStoragesLoading;
        this._helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;

        if (Integrations.ToolbarIcons.IsLoaded)
        {
            Integrations.ToolbarIcons.API.AddToolbarIcon(
                "BetterChests.CraftFromChest",
                "furyx639.BetterChests/Icons",
                new(32, 0, 16, 16),
                I18n.Button_CraftFromChest_Name());
            Integrations.ToolbarIcons.API.ToolbarIconPressed += CraftFromChest.OnToolbarIconPressed;
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
        BetterCrafting.CraftingStoragesLoading -= CraftFromChest.OnCraftingStoragesLoading;
        this._helper.Events.Input.ButtonsChanged -= this.OnButtonsChanged;

        if (Integrations.ToolbarIcons.IsLoaded)
        {
            Integrations.ToolbarIcons.API.RemoveToolbarIcon("BetterChests.CraftFromChest");
            Integrations.ToolbarIcons.API.ToolbarIconPressed -= CraftFromChest.OnToolbarIconPressed;
        }

        if (Integrations.BetterCrafting.IsLoaded)
        {
            Integrations.BetterCrafting.API.UnregisterInventoryProvider(typeof(StorageWrapper));
        }
    }

    private static void OnCraftingStoragesLoading(object? sender, CraftingStoragesLoadingEventArgs e)
    {
        e.AddStorages(CraftFromChest.Eligible);
    }

    private static void OnToolbarIconPressed(object? sender, string id)
    {
        if (id != "BetterChests.CraftFromChest")
        {
            return;
        }

        if (!CraftFromChest.Eligible.Any())
        {
            Game1.showRedMessage(I18n.Alert_CraftFromChest_NoEligible());
            return;
        }

        BetterCrafting.ShowCraftingPage();
    }

    private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Context.IsPlayerFree || !this._config.ControlScheme.OpenCrafting.JustPressed())
        {
            return;
        }

        this._helper.Input.SuppressActiveKeybinds(this._config.ControlScheme.OpenCrafting);
        if (!CraftFromChest.Eligible.Any())
        {
            Game1.showRedMessage(I18n.Alert_CraftFromChest_NoEligible());
            return;
        }

        BetterCrafting.ShowCraftingPage();
    }
}