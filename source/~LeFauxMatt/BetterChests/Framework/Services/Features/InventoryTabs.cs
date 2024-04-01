/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework.Services.Features;

using System.Collections.Immutable;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewMods.BetterChests.Framework.Interfaces;
using StardewMods.BetterChests.Framework.Models;
using StardewMods.BetterChests.Framework.Models.Events;
using StardewMods.BetterChests.Framework.Services.Factory;
using StardewMods.Common.Interfaces;
using StardewMods.Common.Services.Integrations.BetterChests.Enums;
using StardewMods.Common.Services.Integrations.FauxCore;
using StardewValley.Menus;

/// <summary>Adds tabs to the <see cref="ItemGrabMenu" /> to filter the displayed items.</summary>
internal sealed class InventoryTabs : BaseFeature<InventoryTabs>, IItemFilter
{
    private readonly PerScreen<List<InventoryTab>> cachedTabs = new(() => []);
    private readonly PerScreen<int> currentIndex = new(() => -1);
    private readonly IInputHelper inputHelper;
    private readonly InventoryTabFactory inventoryTabFactory;
    private readonly PerScreen<bool> isActive = new();
    private readonly ItemGrabMenuManager itemGrabMenuManager;
    private readonly PerScreen<int> newIndex = new(() => -1);
    private readonly PerScreen<bool> resetCache = new(() => true);

    /// <summary>Initializes a new instance of the <see cref="InventoryTabs" /> class.</summary>
    /// <param name="eventManager">Dependency used for managing events.</param>
    /// <param name="inputHelper">Dependency used for checking and changing input state.</param>
    /// <param name="inventoryTabFactory">Dependency used for managing inventory tabs.</param>
    /// <param name="itemGrabMenuManager">Dependency used for managing the item grab menu.</param>
    /// <param name="log">Dependency used for logging debug information to the console.</param>
    /// <param name="manifest">Dependency for accessing mod manifest.</param>
    /// <param name="modConfig">Dependency used for accessing config data.</param>
    public InventoryTabs(
        IEventManager eventManager,
        IInputHelper inputHelper,
        InventoryTabFactory inventoryTabFactory,
        ItemGrabMenuManager itemGrabMenuManager,
        ILog log,
        IManifest manifest,
        IModConfig modConfig)
        : base(eventManager, log, manifest, modConfig)
    {
        this.inputHelper = inputHelper;
        this.inventoryTabFactory = inventoryTabFactory;
        this.itemGrabMenuManager = itemGrabMenuManager;
    }

    /// <inheritdoc />
    public override bool ShouldBeActive => this.Config.DefaultOptions.InventoryTabs != FeatureOption.Disabled;

    /// <inheritdoc />
    public bool MatchesFilter(Item item) =>
        this.Config.InventoryTabMethod == FilterMethod.Default
        || this.resetCache.Value
        || !this.cachedTabs.Value.Any()
        || this.currentIndex.Value < 0
        || this.currentIndex.Value >= this.cachedTabs.Value.Count
        || this.cachedTabs.Value[this.newIndex.Value].MatchesFilter(item);

    /// <inheritdoc />
    protected override void Activate()
    {
        // Events
        this.Events.Subscribe<RenderingActiveMenuEventArgs>(this.OnRenderingActiveMenu);
        this.Events.Subscribe<ButtonPressedEventArgs>(this.OnButtonPressed);
        this.Events.Subscribe<ButtonsChangedEventArgs>(this.OnButtonsChanged);
        this.Events.Subscribe<MouseWheelScrolledEventArgs>(this.OnMouseWheelScrolled);
        this.Events.Subscribe<ItemGrabMenuChangedEventArgs>(this.OnItemGrabMenuChanged);
    }

    /// <inheritdoc />
    protected override void Deactivate()
    {
        // Events
        this.Events.Unsubscribe<RenderingActiveMenuEventArgs>(this.OnRenderingActiveMenu);
        this.Events.Unsubscribe<ButtonPressedEventArgs>(this.OnButtonPressed);
        this.Events.Unsubscribe<ButtonsChangedEventArgs>(this.OnButtonsChanged);
        this.Events.Unsubscribe<MouseWheelScrolledEventArgs>(this.OnMouseWheelScrolled);
        this.Events.Unsubscribe<ItemGrabMenuChangedEventArgs>(this.OnItemGrabMenuChanged);
    }

    private IEnumerable<Item> FilterByTab(IEnumerable<Item> items) =>
        this.currentIndex.Value == -1
            ? items
            : this.Config.InventoryTabMethod switch
            {
                FilterMethod.Sorted or FilterMethod.GrayedOut => items.OrderByDescending(this.MatchesFilter),
                FilterMethod.Hidden => items.Where(this.MatchesFilter),
                _ => items,
            };

    private void OnButtonPressed(ButtonPressedEventArgs e)
    {
        if (!this.isActive.Value
            || this.resetCache.Value
            || !this.cachedTabs.Value.Any()
            || e.Button is not (SButton.MouseLeft or SButton.MouseRight or SButton.ControllerA))
        {
            return;
        }

        var (x, y) = Game1.getMousePosition(true);
        var tab = this.cachedTabs.Value.FirstOrDefault(tab => tab.Component.containsPoint(x, y));
        if (tab is null)
        {
            return;
        }

        this.newIndex.Value = this.cachedTabs.Value.IndexOf(tab);
        if (this.newIndex.Value == this.currentIndex.Value)
        {
            this.newIndex.Value = -1;
        }

        this.inputHelper.Suppress(e.Button);
    }

    private void OnButtonsChanged(ButtonsChangedEventArgs e)
    {
        if (!this.isActive.Value || this.resetCache.Value || !this.cachedTabs.Value.Any())
        {
            return;
        }

        if (this.Config.Controls.PreviousTab.JustPressed())
        {
            this.newIndex.Value--;
            this.inputHelper.SuppressActiveKeybinds(this.Config.Controls.PreviousTab);
        }

        if (this.Config.Controls.NextTab.JustPressed())
        {
            this.newIndex.Value++;
            this.inputHelper.SuppressActiveKeybinds(this.Config.Controls.NextTab);
        }
    }

    private void OnMouseWheelScrolled(MouseWheelScrolledEventArgs e)
    {
        if (!this.isActive.Value || this.resetCache.Value || !this.cachedTabs.Value.Any())
        {
            return;
        }

        var (x, y) = Game1.getMousePosition(true);
        if (!this.cachedTabs.Value.Any(tab => tab.Component.containsPoint(x, y)))
        {
            return;
        }

        switch (e.Delta)
        {
            case > 0:
                this.newIndex.Value--;
                return;
            case < 0:
                this.newIndex.Value++;
                return;
            default: return;
        }
    }

    private void OnRenderingActiveMenu(RenderingActiveMenuEventArgs e)
    {
        if (!this.isActive.Value || this.itemGrabMenuManager.CurrentMenu is null)
        {
            return;
        }

        // Check if tabs needs to be refreshed
        if (this.resetCache.Value)
        {
            this.RefreshTabs();
            this.resetCache.Value = false;
        }

        // Check if there are any tabs
        if (!this.cachedTabs.Value.Any())
        {
            return;
        }

        // Wrap index
        if (this.newIndex.Value < -1)
        {
            this.newIndex.Value = this.cachedTabs.Value.Count - 1;
        }
        else if (this.newIndex.Value >= this.cachedTabs.Value.Count)
        {
            this.newIndex.Value = -1;
        }

        // Check if index changed
        if (this.newIndex.Value != this.currentIndex.Value)
        {
            // Deselect previous tab
            if (this.currentIndex.Value != -1)
            {
                this.cachedTabs.Value[this.currentIndex.Value].Deselect();
            }

            // Select current tab
            if (this.newIndex.Value != -1)
            {
                this.cachedTabs.Value[this.newIndex.Value].Select();
                this.Log.Trace(
                    "{0}: Set tab to {1}",
                    this.Id,
                    this.cachedTabs.Value[this.newIndex.Value].Component.hoverText);
            }
            else
            {
                this.Log.Trace("{0}: Set tab to All", this.Id);
            }

            this.currentIndex.Value = this.newIndex.Value;
        }

        var (x, y) = Game1.getMousePosition(true);
        foreach (var tab in this.cachedTabs.Value)
        {
            // Tab background
            tab.Draw(e.SpriteBatch);

            // Hover text
            if (tab.Component.containsPoint(x, y))
            {
                (Game1.activeClickableMenu as ItemGrabMenu)!.hoverText = tab.Component.hoverText;
            }
        }
    }

    private void OnItemGrabMenuChanged(ItemGrabMenuChangedEventArgs e)
    {
        if (this.itemGrabMenuManager.Top.Container?.Options.InventoryTabs != FeatureOption.Enabled)
        {
            this.isActive.Value = false;
            this.newIndex.Value = -1;
            return;
        }

        this.isActive.Value = true;
        this.itemGrabMenuManager.Top.AddHighlightMethod(this.MatchesFilter);
        this.itemGrabMenuManager.Top.AddOperation(this.FilterByTab);
        this.currentIndex.Value = -1;
        this.resetCache.Value = true;
    }

    private void RefreshTabs()
    {
        this.cachedTabs.Value.Clear();
        if (this.itemGrabMenuManager.CurrentMenu is null
            || this.itemGrabMenuManager.Top.Menu is null
            || this.itemGrabMenuManager.Top.Container is null
            || this.itemGrabMenuManager.Top.Container.Options.InventoryTabs != FeatureOption.Enabled
            || !this.itemGrabMenuManager.Top.Container.Options.InventoryTabList.Any())
        {
            return;
        }

        // Load tabs
        foreach (var name in this.itemGrabMenuManager.Top.Container.Options.InventoryTabList)
        {
            if (this.inventoryTabFactory.TryGetOne(name, out var tab))
            {
                this.cachedTabs.Value.Add(tab);
            }
        }

        // Assign positions and ids
        var top = this.itemGrabMenuManager.Top;
        var yPosition = top.Menu.yPositionOnScreen
            - (IClickableMenu.borderWidth / 2)
            - Game1.tileSize
            - (top.Rows == 3 ? 20 : 4);

        var below = top.Menu.inventory.Take(this.itemGrabMenuManager.Top.Columns).ToArray();
        var components = this.cachedTabs.Value.Select(tab => tab.Component).ToImmutableArray();
        for (var i = 0; i < components.Length; ++i)
        {
            components[i].myID = 69_420 + i;
            this.itemGrabMenuManager.CurrentMenu.allClickableComponents.Add(components[i]);
            if (i > 0)
            {
                components[i - 1].rightNeighborID = 69_420 + i;
                components[i].leftNeighborID = 69_419 + i;
            }

            if (i < below.Length)
            {
                below[i].upNeighborID = 69_420 + i;
                components[i].downNeighborID = below[i].myID;
            }

            this.cachedTabs.Value[i].Deselect();
            components[i].bounds.X = i > 0 ? components[i - 1].bounds.Right : top.Menu.inventory[0].bounds.Left;
            components[i].bounds.Y = yPosition;
        }
    }
}