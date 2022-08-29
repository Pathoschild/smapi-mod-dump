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

using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI.Events;
using StardewMods.BetterChests.Helpers;
using StardewMods.Common.Enums;
using StardewMods.Common.Integrations.BetterChests;
using StardewValley.Locations;
using StardewValley.Menus;

/// <summary>
///     Stash items into placed chests and chests in the farmer's inventory.
/// </summary>
internal class StashToChest : IFeature
{
    private static StashToChest? Instance;

    private readonly ModConfig _config;
    private readonly IModHelper _helper;

    private bool _isActivated;

    private StashToChest(IModHelper helper, ModConfig config)
    {
        this._helper = helper;
        this._config = config;
    }

    private static IEnumerable<IStorageObject> Eligible =>
        from storage in Storages.All
        where storage.StashToChest is not (FeatureOptionRange.Disabled or FeatureOptionRange.Default)
           && storage.StashToChestDisableLocations?.Contains(Game1.player.currentLocation.Name) != true
           && !(storage.StashToChestDisableLocations?.Contains("UndergroundMine") == true
             && Game1.player.currentLocation is MineShaft mineShaft
             && mineShaft.Name.StartsWith("UndergroundMine"))
           && storage.Source is not null
           && storage.StashToChest.WithinRangeOfPlayer(storage.StashToChestDistance, storage.Source, storage.Position)
        select storage;

    /// <summary>
    ///     Initializes <see cref="StashToChest" />.
    /// </summary>
    /// <param name="helper">SMAPI helper for events, input, and content.</param>
    /// <param name="config">Mod config data.</param>
    /// <returns>Returns an instance of the <see cref="StashToChest" /> class.</returns>
    public static StashToChest Init(IModHelper helper, ModConfig config)
    {
        return StashToChest.Instance ??= new(helper, config);
    }

    /// <inheritdoc />
    public void Activate()
    {
        if (this._isActivated)
        {
            return;
        }

        this._isActivated = true;
        this._helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
        this._helper.Events.Input.ButtonPressed += StashToChest.OnButtonPressed;

        if (!Integrations.ToolbarIcons.IsLoaded)
        {
            return;
        }

        Integrations.ToolbarIcons.API.AddToolbarIcon(
            "BetterChests.StashToChest",
            "furyx639.BetterChests/Icons",
            new(16, 0, 16, 16),
            I18n.Button_StashToChest_Name());
        Integrations.ToolbarIcons.API.ToolbarIconPressed += StashToChest.OnToolbarIconPressed;
    }

    /// <inheritdoc />
    public void Deactivate()
    {
        if (!this._isActivated)
        {
            return;
        }

        this._isActivated = false;
        this._helper.Events.Input.ButtonsChanged -= this.OnButtonsChanged;
        this._helper.Events.Input.ButtonPressed -= StashToChest.OnButtonPressed;

        if (!Integrations.ToolbarIcons.IsLoaded)
        {
            return;
        }

        Integrations.ToolbarIcons.API.RemoveToolbarIcon("BetterChests.StashToChest");
        Integrations.ToolbarIcons.API.ToolbarIconPressed -= StashToChest.OnToolbarIconPressed;
    }

    private static void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (e.Button is not SButton.MouseLeft
         || Game1.activeClickableMenu is not ItemGrabMenu { context: { } context } itemGrabMenu)
        {
            return;
        }

        var (x, y) = Game1.getMousePosition(true);
        if (itemGrabMenu.fillStacksButton?.containsPoint(x, y) == true
         && Storages.TryGetOne(context, out var storage)
         && storage.StashToChest is not (FeatureOptionRange.Disabled or FeatureOptionRange.Default))
        {
            StashToChest.StashIntoStorage(storage);
        }
    }

    private static void OnToolbarIconPressed(object? sender, string id)
    {
        if (id == "BetterChests.StashToChest")
        {
            StashToChest.StashIntoAll();
        }
    }

    private static void StashIntoAll()
    {
        var storages = StashToChest.Eligible.OrderByDescending(storage => storage.StashToChestPriority).ToList();
        var stashedAny = false;
        foreach (var unused in storages.Where(StashToChest.StashIntoStorage))
        {
            stashedAny = true;
        }

        if (stashedAny)
        {
            Game1.playSound("Ship");
            return;
        }

        Game1.showRedMessage(I18n.Alert_StashToChest_NoEligible());
    }

    private static bool StashIntoStorage(IStorageObject storage)
    {
        var stashedAny = false;

        for (var index = 0; index < Game1.player.MaxItems; index++)
        {
            if (Game1.player.Items[index] is null
             || Game1.player.Items[index].modData.ContainsKey("furyx639.BetterChests/LockedSlot"))
            {
                continue;
            }

            var stack = Game1.player.Items[index].Stack;
            var tmp = storage.StashItem(Game1.player.Items[index], storage.StashToChestStacks is FeatureOption.Enabled);
            if (tmp is null)
            {
                Game1.player.Items[index] = null;
            }

            stashedAny = stashedAny || Game1.player.Items[index] is null || stack != Game1.player.Items[index].Stack;
        }

        return stashedAny;
    }

    private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!this._config.ControlScheme.StashItems.JustPressed())
        {
            return;
        }

        // Stash to All
        if (Context.IsPlayerFree)
        {
            StashToChest.StashIntoAll();
            this._helper.Input.SuppressActiveKeybinds(this._config.ControlScheme.StashItems);
            return;
        }

        if (Game1.activeClickableMenu is not ItemGrabMenu
         || BetterItemGrabMenu.Context?.StashToChest is null
                                                        or FeatureOptionRange.Disabled
                                                        or FeatureOptionRange.Default)
        {
            return;
        }

        // Stash to Current
        StashToChest.StashIntoStorage(BetterItemGrabMenu.Context);
        this._helper.Input.SuppressActiveKeybinds(this._config.ControlScheme.StashItems);
    }
}