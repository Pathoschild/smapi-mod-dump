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

using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewMods.BetterChests.Framework.Interfaces;
using StardewMods.BetterChests.Framework.Models.Containers;
using StardewMods.BetterChests.Framework.Models.Events;
using StardewMods.BetterChests.Framework.Services.Factory;
using StardewMods.Common.Helpers;
using StardewMods.Common.Interfaces;
using StardewMods.Common.Models;
using StardewMods.Common.Services.Integrations.BetterChests;
using StardewMods.Common.Services.Integrations.FauxCore;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

/// <summary>Access chests remotely.</summary>
internal sealed class AccessChest : BaseFeature<AccessChest>
{
    private readonly PerScreen<Rectangle> bounds = new(() => Rectangle.Empty);
    private readonly ContainerFactory containerFactory;
    private readonly PerScreen<ClickableComponent?> currentContainer = new();
    private readonly PerScreen<List<IStorageContainer>> currentContainers = new(() => []);
    private readonly IIconRegistry iconRegistry;
    private readonly IInputHelper inputHelper;
    private readonly PerScreen<bool> isActive = new();
    private readonly PerScreen<List<ClickableComponent>> items = new(() => []);
    private readonly PerScreen<ClickableTextureComponent?> leftArrow = new();
    private readonly MenuHandler menuHandler;
    private readonly PerScreen<int> offset = new();
    private readonly PerScreen<ClickableTextureComponent?> rightArrow = new();
    private readonly PerScreen<IExpression?> searchExpression = new();

    /// <summary>Initializes a new instance of the <see cref="AccessChest" /> class.</summary>
    /// <param name="containerFactory">Dependency used for accessing containers.</param>
    /// <param name="eventManager">Dependency used for managing events.</param>
    /// <param name="iconRegistry">Dependency used for registering and retrieving icons.</param>
    /// <param name="inputHelper">Dependency used for checking and changing input state.</param>
    /// <param name="menuHandler">Dependency used for managing the current menu.</param>
    /// <param name="modConfig">Dependency used for accessing config data.</param>
    public AccessChest(
        ContainerFactory containerFactory,
        IEventManager eventManager,
        IIconRegistry iconRegistry,
        IInputHelper inputHelper,
        MenuHandler menuHandler,
        IModConfig modConfig)
        : base(eventManager, modConfig)
    {
        this.containerFactory = containerFactory;
        this.iconRegistry = iconRegistry;
        this.inputHelper = inputHelper;
        this.menuHandler = menuHandler;
    }

    /// <inheritdoc />
    public override bool ShouldBeActive => this.Config.DefaultOptions.AccessChest != RangeOption.Disabled;

    private ClickableTextureComponent LeftArrow =>
        this.leftArrow.Value ??= this.iconRegistry.Icon(VanillaIcon.ArrowLeft).Component(IconStyle.Transparent);

    private ClickableTextureComponent RightArrow =>
        this.rightArrow.Value ??= this.iconRegistry.Icon(VanillaIcon.ArrowRight).Component(IconStyle.Transparent);

    /// <inheritdoc />
    protected override void Activate()
    {
        // Events
        this.Events.Subscribe<ButtonsChangedEventArgs>(this.OnButtonsChanged);
        this.Events.Subscribe<ButtonPressedEventArgs>(this.OnButtonPressed);
        this.Events.Subscribe<MouseWheelScrolledEventArgs>(this.OnMouseWheelScrolled);
        this.Events.Subscribe<RenderedActiveMenuEventArgs>(this.OnRenderedActiveMenu);
        this.Events.Subscribe<InventoryMenuChangedEventArgs>(this.OnInventoryMenuChanged);
        this.Events.Subscribe<ItemHighlightingEventArgs>(this.OnItemHighlighting);
        this.Events.Subscribe<SearchChangedEventArgs>(this.OnSearchChanged);
    }

    /// <inheritdoc />
    protected override void Deactivate()
    {
        // Events
        this.Events.Unsubscribe<ButtonsChangedEventArgs>(this.OnButtonsChanged);
        this.Events.Unsubscribe<ButtonPressedEventArgs>(this.OnButtonPressed);
        this.Events.Unsubscribe<MouseWheelScrolledEventArgs>(this.OnMouseWheelScrolled);
        this.Events.Unsubscribe<RenderedActiveMenuEventArgs>(this.OnRenderedActiveMenu);
        this.Events.Unsubscribe<InventoryMenuChangedEventArgs>(this.OnInventoryMenuChanged);
        this.Events.Unsubscribe<ItemHighlightingEventArgs>(this.OnItemHighlighting);
        this.Events.Unsubscribe<SearchChangedEventArgs>(this.OnSearchChanged);
    }

    private void OnButtonPressed(ButtonPressedEventArgs e)
    {
        if (this.currentContainer.Value is null
            || e.Button is not (SButton.MouseLeft or SButton.ControllerA)
            || !this.menuHandler.TryGetFocus(this, out var focus))
        {
            return;
        }

        var cursor = e.Cursor.GetScaledScreenPixels();
        if (this.currentContainer.Value.bounds.Contains(cursor))
        {
            this.inputHelper.Suppress(e.Button);
            this.isActive.Value = !this.isActive.Value;
            if (this.isActive.Value)
            {
                if (this.menuHandler.Top.Container is not null)
                {
                    var currentIndex = this.currentContainers.Value.IndexOf(this.menuHandler.Top.Container);

                    this.offset.Value = Math.Clamp(
                        currentIndex,
                        0,
                        Math.Max(0, this.currentContainers.Value.Count - 10));
                }
                else
                {
                    this.offset.Value = 0;
                }
            }
            else
            {
                focus.Release();
            }

            return;
        }

        focus.Release();
        if (this.menuHandler.Top.Container is null)
        {
            // Do nothing
        }
        else if (this.LeftArrow.bounds.Contains(cursor))
        {
            this.inputHelper.Suppress(e.Button);
            this.isActive.Value = !this.isActive.Value;
            var previousIndex = this.currentContainers.Value.IndexOf(this.menuHandler.Top.Container) - 1;
            if (previousIndex < 0)
            {
                previousIndex = this.currentContainers.Value.Count - 1;
            }

            var previousContainer = this.currentContainers.Value.ElementAtOrDefault(previousIndex);
            previousContainer?.ShowMenu();
            return;
        }
        else if (this.RightArrow.bounds.Contains(cursor))
        {
            this.inputHelper.Suppress(e.Button);
            this.isActive.Value = !this.isActive.Value;
            var nextIndex = this.currentContainers.Value.IndexOf(this.menuHandler.Top.Container) + 1;
            if (nextIndex >= this.currentContainers.Value.Count)
            {
                nextIndex = 0;
            }

            var nextContainer = this.currentContainers.Value.ElementAtOrDefault(nextIndex);
            nextContainer?.ShowMenu();
            return;
        }

        if (!this.bounds.Value.Contains(cursor) || !this.isActive.Value)
        {
            this.isActive.Value = false;
            return;
        }

        this.inputHelper.Suppress(e.Button);
        var item = this.items.Value.FirstOrDefault(i => i.bounds.Contains(cursor));
        if (item is null)
        {
            return;
        }

        var selectedIndex = int.Parse(item.name, CultureInfo.InvariantCulture) + this.offset.Value;
        var selectedContainer = this.currentContainers.Value.ElementAtOrDefault(selectedIndex);
        selectedContainer?.ShowMenu();
    }

    private void OnButtonsChanged(ButtonsChangedEventArgs e)
    {
        if (Context.IsPlayerFree && this.Config.Controls.AccessChests.JustPressed())
        {
            // Access First Chest
            this.inputHelper.SuppressActiveKeybinds(this.Config.Controls.AccessChests);
            this.containerFactory.GetAll(this.Predicate).MinBy(c => c.ToString())?.ShowMenu();
            return;
        }

        if (this.currentContainer.Value is null
            || this.menuHandler.Top.Container is null
            || !this.menuHandler.CanFocus(this))
        {
            return;
        }

        if (this.Config.Controls.AccessPreviousChest.JustPressed())
        {
            var previousIndex = this.currentContainers.Value.IndexOf(this.menuHandler.Top.Container) - 1;
            if (previousIndex < 0)
            {
                previousIndex = this.currentContainers.Value.Count - 1;
            }

            var nextContainer = this.currentContainers.Value.ElementAtOrDefault(previousIndex);
            nextContainer?.ShowMenu();
            return;
        }

        if (this.Config.Controls.AccessNextChest.JustPressed())
        {
            var nextIndex = this.currentContainers.Value.IndexOf(this.menuHandler.Top.Container) + 1;
            if (nextIndex >= this.currentContainers.Value.Count)
            {
                nextIndex = 0;
            }

            var nextContainer = this.currentContainers.Value.ElementAtOrDefault(nextIndex);
            nextContainer?.ShowMenu();
        }
    }

    private void OnInventoryMenuChanged(InventoryMenuChangedEventArgs e)
    {
        this.isActive.Value = false;
        var top = this.menuHandler.Top;
        if (top.Container?.AccessChest is RangeOption.Disabled or null)
        {
            this.currentContainer.Value = null;
            return;
        }

        var name = string.IsNullOrWhiteSpace(top.Container.StorageName)
            ? top.Container.DisplayName
            : top.Container.StorageName;

        var x = Math.Max(IClickableMenu.borderWidth / 2, (Game1.uiViewport.Width / 2) - (Game1.tileSize * 10));
        var y = IClickableMenu.borderWidth / 2;

        this.LeftArrow.bounds.X = x;
        this.LeftArrow.bounds.Y = y + Game1.tileSize + 20;
        this.LeftArrow.bounds.Y = y + 10;

        this.RightArrow.bounds.X = x + (Game1.tileSize * 2);
        this.RightArrow.bounds.Y = y + Game1.tileSize + 20;
        this.RightArrow.bounds.Y = y + 10;

        var (width, height) = Game1.smallFont.MeasureString(name);
        this.currentContainer.Value = new ClickableComponent(
            new Rectangle(
                x + (Game1.tileSize * 3),
                y,
                (int)width + IClickableMenu.borderWidth,
                (int)height + IClickableMenu.borderWidth),
            name,
            top.Container.ToString());

        this.ReinitializeContainers();
    }

    private void OnItemHighlighting(ItemHighlightingEventArgs e)
    {
        if (this.isActive.Value)
        {
            e.UnHighlight();
        }
    }

    private void OnMouseWheelScrolled(MouseWheelScrolledEventArgs e)
    {
        if (this.currentContainer.Value is null || !this.isActive.Value)
        {
            return;
        }

        var cursor = this.inputHelper.GetCursorPosition().GetScaledScreenPixels();
        if (!this.bounds.Value.Contains(cursor))
        {
            return;
        }

        switch (e.Delta)
        {
            case > 0:
                this.offset.Value--;
                break;
            case < 0:
                this.offset.Value++;
                break;
            default: return;
        }

        this.offset.Value = Math.Min(
            Math.Max(this.offset.Value, 0),
            Math.Max(this.currentContainers.Value.Count - 10, 0));
    }

    [Priority(int.MinValue + 1)]
    private void OnRenderedActiveMenu(RenderedActiveMenuEventArgs e)
    {
        if (this.currentContainer.Value is null)
        {
            return;
        }

        var cursor = this.inputHelper.GetCursorPosition().GetScaledScreenPixels();

        // Draw current container index
        if (this.menuHandler.Top.Container is not null && this.Config.AccessChestsShowArrows)
        {
            var currentIndex = this.currentContainers.Value.IndexOf(this.menuHandler.Top.Container);
            if (currentIndex != -1)
            {
                var textIndex = currentIndex.ToString(CultureInfo.InvariantCulture);
                var width = SpriteText.getWidthOfString(textIndex);
                SpriteText.drawString(
                    e.SpriteBatch,
                    textIndex,
                    (this.LeftArrow.bounds.Left + this.RightArrow.bounds.Left + Game1.tileSize - width) / 2,
                    this.LeftArrow.bounds.Y - 4,
                    999999,
                    -1,
                    999999,
                    1f,
                    1f,
                    false,
                    3,
                    string.Empty,
                    SpriteText.color_White);

                this.LeftArrow.scale = this.LeftArrow.bounds.Contains(cursor)
                    ? Math.Min(Game1.pixelZoom * 1.1f, this.LeftArrow.scale + 0.05f)
                    : Math.Max(Game1.pixelZoom, this.LeftArrow.scale - 0.05f);

                this.RightArrow.scale = this.RightArrow.bounds.Contains(cursor)
                    ? Math.Min(Game1.pixelZoom * 1.1f, this.RightArrow.scale + 0.05f)
                    : Math.Max(Game1.pixelZoom, this.RightArrow.scale - 0.05f);

                this.LeftArrow.draw(e.SpriteBatch);
                this.RightArrow.draw(e.SpriteBatch);
            }
        }

        // Draw current container icon
        IIcon? icon = null;
        if (this.menuHandler.Top.Container is not null
            && !string.IsNullOrWhiteSpace(this.menuHandler.Top.Container.StorageIcon)
            && this.iconRegistry.TryGetIcon(this.menuHandler.Top.Container.StorageIcon, out var storageIcon))
        {
            icon = storageIcon;
        }

        // Draw current container name
        IClickableMenu.drawHoverText(
            e.SpriteBatch,
            (icon is null ? string.Empty : "     ") + this.currentContainer.Value.name,
            Game1.smallFont,
            overrideX: this.currentContainer.Value.bounds.X,
            overrideY: this.currentContainer.Value.bounds.Y);

        if (icon is not null)
        {
            e.SpriteBatch.Draw(
                icon.Texture(IconStyle.Transparent),
                new Vector2(this.currentContainer.Value.bounds.X, this.currentContainer.Value.bounds.Y),
                icon.Area,
                Color.White,
                0,
                Vector2.Zero,
                Game1.pixelZoom,
                SpriteEffects.None,
                1f);
        }

        // Draw dropdown
        if (!this.isActive.Value)
        {
            return;
        }

        IClickableMenu.drawTextureBox(
            e.SpriteBatch,
            Game1.mouseCursors,
            OptionsDropDown.dropDownBGSource,
            this.bounds.Value.X,
            this.bounds.Value.Y,
            this.bounds.Value.Width,
            this.bounds.Value.Height,
            Color.White,
            Game1.pixelZoom,
            false,
            0.97f);

        // Draw Values
        foreach (var item in this.items.Value)
        {
            var index = int.Parse(item.name, CultureInfo.InvariantCulture) + this.offset.Value;
            var container = this.currentContainers.Value.ElementAtOrDefault(index);
            if (container is null)
            {
                continue;
            }

            if (item.bounds.Contains(cursor))
            {
                e.SpriteBatch.Draw(
                    Game1.staminaRect,
                    new Rectangle(item.bounds.X, item.bounds.Y, this.bounds.Value.Width - 16, item.bounds.Height),
                    new Rectangle(0, 0, 1, 1),
                    Color.Wheat,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0.975f);
            }

            var xOffset = 0;
            if (!string.IsNullOrWhiteSpace(container.StorageIcon)
                && this.iconRegistry.TryGetIcon(container.StorageIcon, out icon))
            {
                xOffset = 32;
                e.SpriteBatch.Draw(
                    icon.Texture(IconStyle.Transparent),
                    new Vector2(item.bounds.X, item.bounds.Y),
                    icon.Area,
                    Color.White,
                    0,
                    Vector2.Zero,
                    2,
                    SpriteEffects.None,
                    1f);
            }

            e.SpriteBatch.DrawString(
                Game1.smallFont,
                container.ToString(),
                new Vector2(item.bounds.X + xOffset, item.bounds.Y),
                Game1.textColor);
        }

        // Draw up indicator
        if (this.offset.Value > 0)
        {
            e.SpriteBatch.Draw(
                Game1.staminaRect,
                new Rectangle(this.bounds.Value.X + 8, this.bounds.Value.Top + 8, this.bounds.Value.Width - 16, 4),
                Game1.textColor);
        }

        // Draw down indicator
        if (this.currentContainers.Value.Count - this.offset.Value > 11)
        {
            e.SpriteBatch.Draw(
                Game1.staminaRect,
                new Rectangle(this.bounds.Value.X + 8, this.bounds.Value.Bottom - 12, this.bounds.Value.Width - 16, 4),
                Game1.textColor);
        }
    }

    private void OnSearchChanged(SearchChangedEventArgs e)
    {
        if (this.currentContainer.Value is not null)
        {
            this.ReinitializeContainers();
        }
    }

    private bool Predicate(IStorageContainer container) =>
        container is not FarmerContainer
        && container.AccessChest is not RangeOption.Disabled
        && (this.searchExpression.Value is null
            || this.searchExpression.Value.Equals(container.ToString())
            || this.searchExpression.Value.Equals(container.Items))
        && container.AccessChest.WithinRange(-1, container.Location, container.TileLocation);

    private void ReinitializeContainers()
    {
        var top = this.menuHandler.Top;
        if (this.currentContainer.Value is null || top.Container is null)
        {
            return;
        }

        this.currentContainers.Value.Clear();
        this.currentContainers.Value.AddRange(
            this
                .containerFactory.GetAll(this.Predicate)
                .OrderBy(container => container.AccessChestPriority)
                .ThenBy(container => container.ToString()!));

        if (this.currentContainers.Value.Count == 0)
        {
            this.isActive.Value = false;
            return;
        }

        var maxWidth = 0;
        var maxHeight = 0;
        foreach (var container in this.currentContainers.Value)
        {
            var textValue = container.ToString()!;
            var textBounds = Game1.smallFont.MeasureString(textValue).ToPoint();
            if (!string.IsNullOrWhiteSpace(container.StorageIcon)
                && this.iconRegistry.TryGetIcon(container.StorageIcon, out _))
            {
                textBounds.X += 32;
            }

            maxWidth = Math.Max(maxWidth, textBounds.X);
            maxHeight = Math.Max(maxHeight, textBounds.Y);
        }

        this.bounds.Value = new Rectangle(
            this.currentContainer.Value.bounds.X,
            this.currentContainer.Value.bounds.Bottom,
            maxWidth + 16,
            (maxHeight * Math.Min(10, this.currentContainers.Value.Count)) + 32);

        this.items.Value = Enumerable
            .Range(0, 10)
            .Select(
                i => new ClickableComponent(
                    new Rectangle(
                        this.bounds.Value.X + 8,
                        this.bounds.Value.Y + 16 + (maxHeight * i),
                        this.bounds.Value.Width,
                        maxHeight),
                    i.ToString(CultureInfo.InvariantCulture)))
            .ToList();
    }
}