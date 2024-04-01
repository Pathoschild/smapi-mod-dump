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

using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewMods.BetterChests.Framework.Interfaces;
using StardewMods.BetterChests.Framework.Models.Containers;
using StardewMods.BetterChests.Framework.Services.Factory;
using StardewMods.BetterChests.Framework.Services.Transient;
using StardewMods.BetterChests.Framework.UI;
using StardewMods.Common.Interfaces;
using StardewMods.Common.Services.Integrations.BetterChests.Enums;
using StardewMods.Common.Services.Integrations.FauxCore;
using StardewMods.Common.Services.Integrations.ToolbarIcons;
using StardewValley.Menus;

/// <summary>Search for which chests have the item you're looking for.</summary>
internal sealed class ChestFinder : BaseFeature<ChestFinder>
{
    private readonly AssetHandler assetHandler;
    private readonly ContainerFactory containerFactory;
    private readonly PerScreen<int> currentIndex = new();
    private readonly IInputHelper inputHelper;
    private readonly PerScreen<bool> isActive = new();
    private readonly PerScreen<ItemMatcher> itemMatcher;
    private readonly PerScreen<List<Pointer>> pointers = new(() => []);
    private readonly PerScreen<bool> resetCache = new(() => true);
    private readonly PerScreen<SearchBar> searchBar;
    private readonly PerScreen<SearchOverlay> searchOverlay;
    private readonly ToolbarIconsIntegration toolbarIconsIntegration;

    /// <summary>Initializes a new instance of the <see cref="ChestFinder" /> class.</summary>
    /// <param name="assetHandler">Dependency used for handling assets.</param>
    /// <param name="containerFactory">Dependency used for accessing containers.</param>
    /// <param name="eventManager">Dependency used for managing events.</param>
    /// <param name="inputHelper">Dependency used for checking and changing input state.</param>
    /// <param name="itemMatcherFactory">Dependency used for getting an ItemMatcher.</param>
    /// <param name="log">Dependency used for logging debug information to the console.</param>
    /// <param name="manifest">Dependency for accessing mod manifest.</param>
    /// <param name="modConfig">Dependency used for accessing config data.</param>
    /// <param name="toolbarIconsIntegration">Dependency for Toolbar Icons integration.</param>
    public ChestFinder(
        AssetHandler assetHandler,
        ContainerFactory containerFactory,
        IEventManager eventManager,
        IInputHelper inputHelper,
        ItemMatcherFactory itemMatcherFactory,
        ILog log,
        IManifest manifest,
        IModConfig modConfig,
        ToolbarIconsIntegration toolbarIconsIntegration)
        : base(eventManager, log, manifest, modConfig)
    {
        this.assetHandler = assetHandler;
        this.containerFactory = containerFactory;
        this.inputHelper = inputHelper;
        this.toolbarIconsIntegration = toolbarIconsIntegration;
        this.itemMatcher = new PerScreen<ItemMatcher>(itemMatcherFactory.GetOneForSearch);
        this.searchBar = new PerScreen<SearchBar>(
            () => new SearchBar(
                () => this.itemMatcher.Value.SearchText,
                value =>
                {
                    this.Log.Trace("{0}: Searching for {1}", this.Id, value);
                    this.itemMatcher.Value.SearchText = value;
                    this.resetCache.Value = true;
                }));

        this.searchOverlay = new PerScreen<SearchOverlay>(() => new SearchOverlay(this.searchBar.Value));
    }

    /// <inheritdoc />
    public override bool ShouldBeActive => this.Config.DefaultOptions.ChestFinder != FeatureOption.Disabled;

    /// <inheritdoc />
    protected override void Activate()
    {
        // Events
        this.Events.Subscribe<RenderedHudEventArgs>(this.OnRenderedHud);
        this.Events.Subscribe<RenderingHudEventArgs>(this.OnRenderingHud);
        this.Events.Subscribe<ButtonPressedEventArgs>(this.OnButtonPressed);
        this.Events.Subscribe<ButtonsChangedEventArgs>(this.OnButtonsChanged);
        this.Events.Subscribe<ChestInventoryChangedEventArgs>(this.OnChestInventoryChanged);
        this.Events.Subscribe<WarpedEventArgs>(this.OnWarped);

        // Integrations
        if (!this.toolbarIconsIntegration.IsLoaded)
        {
            return;
        }

        this.toolbarIconsIntegration.Api.AddToolbarIcon(
            this.Id,
            this.assetHandler.IconTexturePath,
            new Rectangle(48, 0, 16, 16),
            I18n.Button_FindChest_Name());

        this.toolbarIconsIntegration.Api.Subscribe(this.OnIconPressed);
    }

    /// <inheritdoc />
    protected override void Deactivate()
    {
        // Events
        this.Events.Unsubscribe<RenderedHudEventArgs>(this.OnRenderedHud);
        this.Events.Unsubscribe<RenderingHudEventArgs>(this.OnRenderingHud);
        this.Events.Unsubscribe<ButtonPressedEventArgs>(this.OnButtonPressed);
        this.Events.Unsubscribe<ButtonsChangedEventArgs>(this.OnButtonsChanged);
        this.Events.Unsubscribe<ChestInventoryChangedEventArgs>(this.OnChestInventoryChanged);
        this.Events.Unsubscribe<WarpedEventArgs>(this.OnWarped);

        // Integrations
        if (!this.toolbarIconsIntegration.IsLoaded)
        {
            return;
        }

        this.toolbarIconsIntegration.Api.RemoveToolbarIcon(this.Id);
        this.toolbarIconsIntegration.Api.Unsubscribe(this.OnIconPressed);
    }

    private void OnButtonPressed(ButtonPressedEventArgs e)
    {
        if (!this.isActive.Value
            || e.Button is not (SButton.MouseLeft or SButton.MouseRight)
            || !Context.IsPlayerFree
            || !Game1.displayHUD)
        {
            return;
        }

        var (mouseX, mouseY) = Game1.getMousePosition(true);
        switch (e.Button)
        {
            case SButton.MouseLeft:
                this.searchOverlay.Value.receiveLeftClick(mouseX, mouseY);
                break;
            case SButton.MouseRight:
                this.searchOverlay.Value.receiveRightClick(mouseX, mouseY);
                break;
            default: return;
        }

        if (Game1.activeClickableMenu is SearchOverlay)
        {
            this.inputHelper.Suppress(e.Button);
        }
    }

    private void OnButtonsChanged(ButtonsChangedEventArgs e)
    {
        // Activate Search Bar
        if (Context.IsPlayerFree
            && Game1.displayHUD
            && Game1.activeClickableMenu is null
            && this.Config.Controls.FindChest.JustPressed())
        {
            this.OpenSearchBar();
            this.inputHelper.SuppressActiveKeybinds(this.Config.Controls.FindChest);
            return;
        }

        // Close Search Bar
        if (this.isActive.Value && this.Config.Controls.CloseChestFinder.JustPressed())
        {
            this.CloseSearchBar();
            this.inputHelper.SuppressActiveKeybinds(this.Config.Controls.CloseChestFinder);
            return;
        }

        if (!this.isActive.Value || !this.pointers.Value.Any() || !this.Config.Controls.OpenFoundChest.JustPressed())
        {
            return;
        }

        // Open Found Chest
        if (Game1.activeClickableMenu is ItemGrabMenu)
        {
            this.currentIndex.Value++;
        }
        else
        {
            this.currentIndex.Value = 0;
        }

        this.OpenFoundChest();
        this.inputHelper.SuppressActiveKeybinds(this.Config.Controls.OpenFoundChest);
    }

    private void OnChestInventoryChanged(ChestInventoryChangedEventArgs e)
    {
        if (e.Location.Equals(Game1.currentLocation))
        {
            this.resetCache.Value = true;
        }
    }

    private void OnRenderedHud(RenderedHudEventArgs e)
    {
        if (!this.isActive.Value || !Game1.displayHUD || Game1.activeClickableMenu is SearchOverlay)
        {
            return;
        }

        // Check if storages needs to be reset
        if (this.resetCache.Value)
        {
            this.SearchForStorages();
            this.resetCache.Value = false;
        }

        // Check if there are any storages found
        foreach (var pointer in this.pointers.Value)
        {
            pointer.Draw(e.SpriteBatch);
        }

        this.searchOverlay.Value.draw(e.SpriteBatch);
    }

    private void OnRenderingHud(RenderingHudEventArgs e)
    {
        if (!this.isActive.Value || !Game1.displayHUD)
        {
            return;
        }

        var (mouseX, mouseY) = Game1.getMousePosition(true);
        this.searchBar.Value.Update(mouseX, mouseY);
    }

    private void OnWarped(WarpedEventArgs e) => this.resetCache.Value = true;

    private void OpenSearchBar()
    {
        this.isActive.Value = true;
        this.searchOverlay.Value.Show();
    }

    private void OnIconPressed(IIconPressedEventArgs e)
    {
        if (e.Id == this.Id)
        {
            this.OpenSearchBar();
        }
    }

    private void CloseSearchBar()
    {
        this.isActive.Value = false;
        this.pointers.Value.Clear();
        this.resetCache.Value = true;
    }

    private void OpenFoundChest()
    {
        if (this.currentIndex.Value < 0)
        {
            this.currentIndex.Value = this.pointers.Value.Count - 1;
        }
        else if (this.currentIndex.Value >= this.pointers.Value.Count)
        {
            this.currentIndex.Value = 0;
        }

        var container = this.pointers.Value[this.currentIndex.Value].Container;
        container.Mutex?.RequestLock(
            () =>
            {
                container.ShowMenu();
            });
    }

    private void SearchForStorages()
    {
        this.pointers.Value.Clear();
        if (this.itemMatcher.Value.IsEmpty)
        {
            return;
        }

        foreach (var container in this
            .containerFactory.GetAllFromLocation(
                Game1.player.currentLocation,
                container => container.Options.ChestFinder == FeatureOption.Enabled)
            .Where(container => container is ChestContainer or ObjectContainer))
        {
            if (container.Items.Any(this.itemMatcher.Value.MatchesFilter))
            {
                this.pointers.Value.Add(new Pointer(container));
            }
        }

        this.Log.Trace("{0}: Found {1} chests", this.Id, this.pointers.Value.Count);
        this.currentIndex.Value = 0;
    }
}