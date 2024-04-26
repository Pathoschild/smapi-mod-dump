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
using StardewMods.BetterChests.Framework.Models.Events;
using StardewMods.BetterChests.Framework.UI;
using StardewMods.Common.Interfaces;
using StardewMods.Common.Services.Integrations.BetterChests.Enums;
using StardewMods.Common.Services.Integrations.FauxCore;
using StardewValley.Menus;

/// <summary>Adds a search bar to the top of the <see cref="ItemGrabMenu" />.</summary>
internal sealed class SearchItems : BaseFeature<SearchItems>
{
    private readonly PerScreen<ClickableTextureComponent> existingStacksButton;
    private readonly IInputHelper inputHelper;
    private readonly PerScreen<bool> isActive = new(() => true);
    private readonly MenuManager menuManager;
    private readonly PerScreen<ClickableTextureComponent> rejectButton;
    private readonly PerScreen<ClickableTextureComponent> saveButton;
    private readonly PerScreen<SearchBar?> searchBar = new();
    private readonly PerScreen<ISearchExpression?> searchExpression;
    private readonly SearchHandler searchHandler;
    private readonly PerScreen<string> searchText;

    /// <summary>Initializes a new instance of the <see cref="SearchItems" /> class.</summary>
    /// <param name="assetHandler">Dependency used for handling assets.</param>
    /// <param name="eventManager">Dependency used for managing events.</param>
    /// <param name="inputHelper">Dependency used for checking and changing input state.</param>
    /// <param name="menuManager">Dependency used for managing the current menu.</param>
    /// <param name="log">Dependency used for logging debug information to the console.</param>
    /// <param name="manifest">Dependency for accessing mod manifest.</param>
    /// <param name="modConfig">Dependency used for accessing config data.</param>
    /// <param name="searchExpression">Dependency for retrieving a parsed search expression.</param>
    /// <param name="searchHandler">Dependency used for handling search.</param>
    /// <param name="searchText">Dependency for retrieving the unified search text.</param>
    public SearchItems(
        AssetHandler assetHandler,
        IEventManager eventManager,
        IInputHelper inputHelper,
        MenuManager menuManager,
        ILog log,
        IManifest manifest,
        IModConfig modConfig,
        PerScreen<ISearchExpression?> searchExpression,
        SearchHandler searchHandler,
        PerScreen<string> searchText)
        : base(eventManager, log, manifest, modConfig)
    {
        this.inputHelper = inputHelper;
        this.menuManager = menuManager;
        this.searchExpression = searchExpression;
        this.searchHandler = searchHandler;
        this.searchText = searchText;

        this.rejectButton = new PerScreen<ClickableTextureComponent>(
            () => new ClickableTextureComponent(
                new Rectangle(0, 0, 24, 24),
                Game1.mouseCursors,
                new Rectangle(322, 498, 12, 12),
                2f)
            {
                name = "Reject",
                hoverText = I18n.Button_RejectUncategorized_Name(),
                myID = 8_675_308,
            });

        this.saveButton = new PerScreen<ClickableTextureComponent>(
            () => new ClickableTextureComponent(
                new Rectangle(0, 0, Game1.tileSize / 2, Game1.tileSize / 2),
                assetHandler.Icons.Value,
                new Rectangle(142, 0, 16, 16),
                2f)
            {
                name = "Save",
                hoverText = I18n.Button_SaveAsCategorization_Name(),
                myID = 8_675_309,
            });

        this.existingStacksButton = new PerScreen<ClickableTextureComponent>(
            () => new ClickableTextureComponent(
                new Rectangle(0, 0, 27, 27),
                Game1.mouseCursors,
                new Rectangle(227, 425, 9, 9),
                3f)
            {
                name = "ExistingStacks",
                hoverText = I18n.Button_IncludeExistingStacks_Name(),
                myID = 8_675_310,
            });
    }

    /// <inheritdoc />
    public override bool ShouldBeActive => this.Config.DefaultOptions.SearchItems != FeatureOption.Disabled;

    /// <inheritdoc />
    protected override void Activate()
    {
        // Events
        this.Events.Subscribe<RenderedActiveMenuEventArgs>(this.OnRenderedActiveMenu);
        this.Events.Subscribe<RenderingActiveMenuEventArgs>(this.OnRenderingActiveMenu);
        this.Events.Subscribe<ButtonPressedEventArgs>(this.OnButtonPressed);
        this.Events.Subscribe<ButtonsChangedEventArgs>(this.OnButtonsChanged);
        this.Events.Subscribe<InventoryMenuChangedEventArgs>(this.OnInventoryMenuChanged);
        this.Events.Subscribe<ItemHighlightingEventArgs>(this.OnItemHighlighting);
        this.Events.Subscribe<ItemsDisplayingEventArgs>(this.OnItemsDisplaying);
    }

    /// <inheritdoc />
    protected override void Deactivate()
    {
        // Events
        this.Events.Unsubscribe<RenderedActiveMenuEventArgs>(this.OnRenderedActiveMenu);
        this.Events.Unsubscribe<RenderingActiveMenuEventArgs>(this.OnRenderingActiveMenu);
        this.Events.Unsubscribe<ButtonPressedEventArgs>(this.OnButtonPressed);
        this.Events.Unsubscribe<ButtonsChangedEventArgs>(this.OnButtonsChanged);
        this.Events.Unsubscribe<InventoryMenuChangedEventArgs>(this.OnInventoryMenuChanged);
        this.Events.Unsubscribe<ItemHighlightingEventArgs>(this.OnItemHighlighting);
        this.Events.Unsubscribe<ItemsDisplayingEventArgs>(this.OnItemsDisplaying);
    }

    private void OnButtonPressed(ButtonPressedEventArgs e)
    {
        var container = this.menuManager.Top.Container;
        if (container is null
            || !this.isActive.Value
            || this.searchBar.Value is null
            || this.menuManager.CurrentMenu is not ItemGrabMenu
            || !this.menuManager.CanFocus(this))
        {
            return;
        }

        var (mouseX, mouseY) = Game1.getMousePosition(true);
        switch (e.Button)
        {
            case SButton.MouseLeft or SButton.ControllerA:
                if (this.searchBar.Value.LeftClick(mouseX, mouseY))
                {
                    this.inputHelper.Suppress(e.Button);
                    return;
                }

                if (container.Options.CategorizeChest != FeatureOption.Enabled)
                {
                    return;
                }

                if (this.rejectButton.Value.containsPoint(mouseX, mouseY))
                {
                    this.inputHelper.Suppress(e.Button);
                    container.Options.CategorizeChestBlockItems =
                        container.Options.CategorizeChestBlockItems == FeatureOption.Enabled
                            ? FeatureOption.Disabled
                            : FeatureOption.Enabled;

                    return;
                }

                if (this.saveButton.Value.containsPoint(mouseX, mouseY))
                {
                    this.inputHelper.Suppress(e.Button);
                    container.Options.CategorizeChestSearchTerm = this.searchText.Value;
                    return;
                }

                if (this.existingStacksButton.Value.containsPoint(mouseX, mouseY))
                {
                    this.inputHelper.Suppress(e.Button);
                    container.Options.CategorizeChestIncludeStacks =
                        container.Options.CategorizeChestIncludeStacks == FeatureOption.Enabled
                            ? FeatureOption.Disabled
                            : FeatureOption.Enabled;

                    this.existingStacksButton.Value.sourceRect =
                        container.Options.CategorizeChestIncludeStacks == FeatureOption.Enabled
                            ? new Rectangle(236, 425, 9, 9)
                            : new Rectangle(227, 425, 9, 9);
                }

                break;

            case SButton.MouseRight or SButton.ControllerB:
                if (this.searchBar.Value.RightClick(mouseX, mouseY))
                {
                    this.inputHelper.Suppress(e.Button);
                }

                break;

            case SButton.Escape when this.menuManager.CurrentMenu.readyToClose():
                this.inputHelper.Suppress(e.Button);
                Game1.playSound("bigDeSelect");
                this.menuManager.CurrentMenu.exitThisMenu();
                break;

            case SButton.Escape: return;

            default:
                if (this.searchBar.Value.Selected
                    && e.Button is not (SButton.LeftShift
                        or SButton.RightShift
                        or SButton.LeftAlt
                        or SButton.RightAlt
                        or SButton.LeftControl
                        or SButton.RightControl))
                {
                    this.inputHelper.Suppress(e.Button);
                }

                return;
        }
    }

    private void OnButtonsChanged(ButtonsChangedEventArgs e)
    {
        if (this.menuManager.Top.Container?.Options.SearchItems != FeatureOption.Enabled
            || !this.Config.Controls.ToggleSearch.JustPressed())
        {
            return;
        }

        this.inputHelper.SuppressActiveKeybinds(this.Config.Controls.ToggleSearch);
        this.isActive.Value = !this.isActive.Value;
    }

    private void OnInventoryMenuChanged(InventoryMenuChangedEventArgs e)
    {
        var container = this.menuManager.Top.Container;
        var top = this.menuManager.Top;
        if (top.Menu is null || container is null)
        {
            this.searchBar.Value = null;
            return;
        }

        if (container.Options.SearchItems != FeatureOption.Enabled)
        {
            this.searchBar.Value = null;
        }

        var width = Math.Min(12 * Game1.tileSize, Game1.uiViewport.Width);

        var x = top.Columns switch
        {
            3 => top.Menu.inventory[1].bounds.Center.X - (width / 2),
            12 => top.Menu.inventory[5].bounds.Right - (width / 2),
            14 => top.Menu.inventory[6].bounds.Right - (width / 2),
            _ => (Game1.uiViewport.Width - width) / 2,
        };

        var y = top.Menu.yPositionOnScreen
            - (IClickableMenu.borderWidth / 2)
            - Game1.tileSize
            - (top.Rows == 3 ? 25 : 4);

        this.searchBar.Value = new SearchBar(
            x,
            y,
            width,
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

        if (container.Options.CategorizeChest != FeatureOption.Enabled)
        {
            return;
        }

        x += width;
        y += 8;

        this.saveButton.Value.bounds.X = x;
        this.saveButton.Value.bounds.Y = y;

        this.existingStacksButton.Value.bounds.X = x + 32;
        this.existingStacksButton.Value.bounds.Y = y + 2;
        this.existingStacksButton.Value.sourceRect =
            container.Options.CategorizeChestIncludeStacks == FeatureOption.Enabled
                ? new Rectangle(236, 425, 9, 9)
                : new Rectangle(227, 425, 9, 9);

        this.rejectButton.Value.bounds.X = x + 64;
        this.rejectButton.Value.bounds.Y = y + 2;
    }

    private void OnItemHighlighting(ItemHighlightingEventArgs e)
    {
        if (this.searchExpression.Value?.PartialMatch(e.Item) == false)
        {
            e.UnHighlight();
        }
    }

    private void OnItemsDisplaying(ItemsDisplayingEventArgs e)
    {
        if (this.searchExpression.Value is null
            || this.Config.SearchItemsMethod is not (FilterMethod.Sorted
                or FilterMethod.GrayedOut
                or FilterMethod.Hidden))
        {
            return;
        }

        e.Edit(
            items => this.Config.SearchItemsMethod switch
            {
                FilterMethod.Sorted or FilterMethod.GrayedOut => items.OrderByDescending(
                    this.searchExpression.Value.PartialMatch),
                FilterMethod.Hidden => items.Where(this.searchExpression.Value.PartialMatch),
                _ => items,
            });
    }

    private void OnRenderedActiveMenu(RenderedActiveMenuEventArgs e)
    {
        var container = this.menuManager.Top.Container;
        if (this.searchBar.Value is null
            || !this.isActive.Value
            || this.menuManager.CurrentMenu is not ItemGrabMenu itemGrabMenu
            || container is null)
        {
            return;
        }

        this.searchBar.Value.Draw(e.SpriteBatch);

        if (container.Options.CategorizeChest != FeatureOption.Enabled)
        {
            return;
        }

        var (mouseX, mouseY) = Game1.getMousePosition(true);
        this.saveButton.Value.tryHover(mouseX, mouseY);
        this.existingStacksButton.Value.tryHover(mouseX, mouseY);
        this.rejectButton.Value.tryHover(mouseX, mouseY);

        this.saveButton.Value.draw(e.SpriteBatch);
        this.existingStacksButton.Value.draw(e.SpriteBatch);
        this.rejectButton.Value.draw(
            e.SpriteBatch,
            container.Options.CategorizeChestBlockItems == FeatureOption.Enabled ? Color.White : Color.Gray,
            1f);

        if (this.saveButton.Value.containsPoint(mouseX, mouseY))
        {
            itemGrabMenu.hoverText = this.saveButton.Value.hoverText;
            return;
        }

        if (this.existingStacksButton.Value.containsPoint(mouseX, mouseY))
        {
            itemGrabMenu.hoverText = this.existingStacksButton.Value.hoverText;
            return;
        }

        if (this.rejectButton.Value.containsPoint(mouseX, mouseY))
        {
            itemGrabMenu.hoverText = this.rejectButton.Value.hoverText;
        }
    }

    private void OnRenderingActiveMenu(RenderingActiveMenuEventArgs e)
    {
        if (this.searchBar.Value is null || !this.isActive.Value || this.menuManager.CurrentMenu is not ItemGrabMenu)
        {
            return;
        }

        var (mouseX, mouseY) = Game1.getMousePosition(true);
        this.searchBar.Value.Update(mouseX, mouseY);
    }
}