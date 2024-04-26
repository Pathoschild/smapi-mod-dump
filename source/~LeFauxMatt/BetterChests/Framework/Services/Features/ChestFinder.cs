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
using StardewMods.BetterChests.Framework.Models.Events;
using StardewMods.BetterChests.Framework.Services.Factory;
using StardewMods.BetterChests.Framework.UI;
using StardewMods.Common.Interfaces;
using StardewMods.Common.Services.Integrations.BetterChests.Enums;
using StardewMods.Common.Services.Integrations.FauxCore;
using StardewMods.Common.Services.Integrations.ToolbarIcons;

/// <summary>Search for which chests have the item you're looking for.</summary>
internal sealed class ChestFinder : BaseFeature<ChestFinder>
{
    private readonly AssetHandler assetHandler;
    private readonly ContainerFactory containerFactory;
    private readonly PerScreen<int> currentIndex = new();
    private readonly IInputHelper inputHelper;
    private readonly MenuManager menuManager;
    private readonly PerScreen<List<Pointer>> pointers = new(() => []);
    private readonly PerScreen<ISearchExpression?> searchExpression;
    private readonly SearchHandler searchHandler;
    private readonly PerScreen<string> searchText;
    private readonly ToolbarIconsIntegration toolbarIconsIntegration;

    /// <summary>Initializes a new instance of the <see cref="ChestFinder" /> class.</summary>
    /// <param name="assetHandler">Dependency used for handling assets.</param>
    /// <param name="containerFactory">Dependency used for accessing containers.</param>
    /// <param name="eventManager">Dependency used for managing events.</param>
    /// <param name="inputHelper">Dependency used for checking and changing input state.</param>
    /// <param name="log">Dependency used for logging debug information to the console.</param>
    /// <param name="manifest">Dependency for accessing mod manifest.</param>
    /// <param name="menuManager">Dependency used for managing the current menu.</param>
    /// <param name="modConfig">Dependency used for accessing config data.</param>
    /// <param name="searchExpression">Dependency for retrieving a parsed search expression.</param>
    /// <param name="searchHandler">Dependency used for handling search.</param>
    /// <param name="searchText">Dependency for retrieving the unified search text.</param>
    /// <param name="toolbarIconsIntegration">Dependency for Toolbar Icons integration.</param>
    public ChestFinder(
        AssetHandler assetHandler,
        ContainerFactory containerFactory,
        IEventManager eventManager,
        IInputHelper inputHelper,
        ILog log,
        IManifest manifest,
        MenuManager menuManager,
        IModConfig modConfig,
        PerScreen<ISearchExpression?> searchExpression,
        SearchHandler searchHandler,
        PerScreen<string> searchText,
        ToolbarIconsIntegration toolbarIconsIntegration)
        : base(eventManager, log, manifest, modConfig)
    {
        this.assetHandler = assetHandler;
        this.containerFactory = containerFactory;
        this.inputHelper = inputHelper;
        this.menuManager = menuManager;
        this.searchExpression = searchExpression;
        this.searchHandler = searchHandler;
        this.searchText = searchText;
        this.toolbarIconsIntegration = toolbarIconsIntegration;
    }

    /// <inheritdoc />
    public override bool ShouldBeActive => this.Config.DefaultOptions.ChestFinder != FeatureOption.Disabled;

    /// <inheritdoc />
    protected override void Activate()
    {
        // Events
        this.Events.Subscribe<RenderedHudEventArgs>(this.OnRenderedHud);
        this.Events.Subscribe<ButtonsChangedEventArgs>(this.OnButtonsChanged);
        this.Events.Subscribe<MenuChangedEventArgs>(this.OnMenuChanged);
        this.Events.Subscribe<SearchChangedEventArgs>(this.OnSearchChanged);
        this.Events.Subscribe<WarpedEventArgs>(this.OnWarped);

        // Integrations
        if (!this.toolbarIconsIntegration.IsLoaded)
        {
            return;
        }

        this.toolbarIconsIntegration.Api.AddToolbarIcon(
            this.Id,
            this.assetHandler.Icons.Name.BaseName,
            new Rectangle(48, 0, 16, 16),
            I18n.Button_FindChest_Name());

        this.toolbarIconsIntegration.Api.Subscribe(this.OnIconPressed);
    }

    /// <inheritdoc />
    protected override void Deactivate()
    {
        // Events
        this.Events.Unsubscribe<RenderedHudEventArgs>(this.OnRenderedHud);
        this.Events.Unsubscribe<ButtonsChangedEventArgs>(this.OnButtonsChanged);
        this.Events.Subscribe<MenuChangedEventArgs>(this.OnMenuChanged);
        this.Events.Subscribe<SearchChangedEventArgs>(this.OnSearchChanged);
        this.Events.Unsubscribe<WarpedEventArgs>(this.OnWarped);

        // Integrations
        if (!this.toolbarIconsIntegration.IsLoaded)
        {
            return;
        }

        this.toolbarIconsIntegration.Api.RemoveToolbarIcon(this.Id);
        this.toolbarIconsIntegration.Api.Unsubscribe(this.OnIconPressed);
    }

    private void OnButtonsChanged(ButtonsChangedEventArgs e)
    {
        // Activate Search Bar
        if (Context.IsPlayerFree
            && Game1.displayHUD
            && this.menuManager.CurrentMenu is null
            && this.Config.Controls.ToggleSearch.JustPressed())
        {
            this.inputHelper.SuppressActiveKeybinds(this.Config.Controls.ToggleSearch);
            this.OpenSearchBar();
            return;
        }

        if (this.menuManager.CurrentMenu is not SearchOverlay
            || !this.pointers.Value.Any()
            || !this.Config.Controls.OpenFoundChest.JustPressed())
        {
            return;
        }

        // Open Found Chest
        this.inputHelper.SuppressActiveKeybinds(this.Config.Controls.OpenFoundChest);
        var container = this.pointers.Value.First().Container;
        container.Mutex?.RequestLock(
            () =>
            {
                container.ShowMenu();
            });
    }

    private void OnRenderedHud(RenderedHudEventArgs e)
    {
        if (this.menuManager.CurrentMenu is not SearchOverlay && (!Game1.displayHUD || !Context.IsPlayerFree))
        {
            return;
        }

        foreach (var pointer in this.pointers.Value)
        {
            pointer.Draw(e.SpriteBatch);
        }
    }

    private void OnMenuChanged(MenuChangedEventArgs e) => this.ReinitializePointers();

    private void OnSearchChanged(SearchChangedEventArgs e) => this.ReinitializePointers();

    private void OnWarped(WarpedEventArgs e) => this.ReinitializePointers();

    private void OpenSearchBar() =>
        Game1.activeClickableMenu = new SearchOverlay(
            () => this.searchText.Value,
            value =>
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    this.Log.Trace("{0}: Searching for {1}", this.Id, value);
                }

                if (this.searchText.Value == value)
                {
                    return;
                }

                this.searchText.Value = value;
                this.searchExpression.Value = this.searchHandler.TryParseExpression(value, out var expression)
                    ? expression
                    : null;

                this.Events.Publish(new SearchChangedEventArgs(this.searchExpression.Value));
            });

    private void OnIconPressed(IIconPressedEventArgs e)
    {
        if (e.Id == this.Id)
        {
            this.OpenSearchBar();
        }
    }

    private void ReinitializePointers()
    {
        this.pointers.Value.Clear();
        if (this.searchExpression.Value is null)
        {
            return;
        }

        if (this.menuManager.CurrentMenu is not SearchOverlay && (!Game1.displayHUD || !Context.IsPlayerFree))
        {
            return;
        }

        foreach (var container in this
            .containerFactory.GetAll(
                Game1.player.currentLocation,
                container => container.Options.ChestFinder == FeatureOption.Enabled)
            .Where(container => container is not FarmerContainer))
        {
            if (container.Items.Any(this.searchExpression.Value.PartialMatch))
            {
                this.pointers.Value.Add(new Pointer(container));
            }
        }

        this.Log.Info("{0}: Found {1} chests", this.Id, this.pointers.Value.Count);
        this.currentIndex.Value = 0;
    }
}