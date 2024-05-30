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

using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewMods.BetterChests.Framework.Interfaces;
using StardewMods.BetterChests.Framework.Models.Events;
using StardewMods.Common.Interfaces;
using StardewMods.Common.Services.Integrations.BetterChests;
using StardewMods.Common.Services.Integrations.FauxCore;
using StardewValley.Menus;

/// <summary>Customize how an inventory gets sorted.</summary>
internal sealed class SortInventory : BaseFeature<SortInventory>
{
    private readonly ContainerHandler containerHandler;
    private readonly IExpressionHandler expressionHandler;
    private readonly IIconRegistry iconRegistry;
    private readonly IInputHelper inputHelper;
    private readonly MenuHandler menuHandler;
    private readonly PerScreen<ClickableTextureComponent?> organizeButton = new();

    /// <summary>Initializes a new instance of the <see cref="SortInventory" /> class.</summary>
    /// <param name="containerHandler">Dependency used for handling operations by containers.</param>
    /// <param name="eventManager">Dependency used for managing events.</param>
    /// <param name="expressionHandler">Dependency used for parsing expressions.</param>
    /// <param name="iconRegistry">Dependency used for registering and retrieving icons.</param>
    /// <param name="inputHelper">Dependency used for checking and changing input state.</param>
    /// <param name="menuHandler">Dependency used for managing the current menu.</param>
    /// <param name="modConfig">Dependency used for accessing config data.</param>
    public SortInventory(
        ContainerHandler containerHandler,
        IEventManager eventManager,
        IExpressionHandler expressionHandler,
        IIconRegistry iconRegistry,
        IInputHelper inputHelper,
        MenuHandler menuHandler,
        IModConfig modConfig)
        : base(eventManager, modConfig)
    {
        this.containerHandler = containerHandler;
        this.expressionHandler = expressionHandler;
        this.iconRegistry = iconRegistry;
        this.inputHelper = inputHelper;
        this.menuHandler = menuHandler;
    }

    /// <inheritdoc />
    public override bool ShouldBeActive => this.Config.DefaultOptions.SortInventory != FeatureOption.Disabled;

    /// <inheritdoc />
    protected override void Activate()
    {
        // Events
        this.Events.Subscribe<ButtonPressedEventArgs>(this.OnButtonPressed);
        this.Events.Subscribe<ContainerSortingEventArgs>(this.OnContainerSorting);
        this.Events.Subscribe<InventoryMenuChangedEventArgs>(this.OnInventoryMenuChanged);
        this.Events.Subscribe<RenderedActiveMenuEventArgs>(this.OnRenderedActiveMenu);
    }

    /// <inheritdoc />
    protected override void Deactivate()
    {
        // Events
        this.Events.Unsubscribe<ButtonPressedEventArgs>(this.OnButtonPressed);
        this.Events.Unsubscribe<ContainerSortingEventArgs>(this.OnContainerSorting);
        this.Events.Unsubscribe<InventoryMenuChangedEventArgs>(this.OnInventoryMenuChanged);
        this.Events.Unsubscribe<RenderedActiveMenuEventArgs>(this.OnRenderedActiveMenu);
    }

    private void OnButtonPressed(ButtonPressedEventArgs e)
    {
        if (!this.menuHandler.CanFocus(this))
        {
            return;
        }

        var cursor = e.Cursor.GetScaledScreenPixels();
        var container = this.menuHandler.CurrentMenu switch
        {
            ItemGrabMenu itemGrabMenu when itemGrabMenu.organizeButton?.bounds.Contains(cursor) == true =>
                this.menuHandler.Top.Container,
            InventoryPage inventoryPage when inventoryPage.organizeButton?.bounds.Contains(cursor) == true =>
                this.menuHandler.Bottom.Container,
            not null when this.organizeButton.Value?.bounds.Contains(cursor) == true => this.menuHandler.Bottom
                .Container,
            _ => null,
        };

        if (container?.SortInventory is not FeatureOption.Enabled)
        {
            return;
        }

        switch (e.Button)
        {
            case SButton.MouseLeft or SButton.ControllerA:
                this.containerHandler.Sort(container);
                break;

            case SButton.MouseRight or SButton.ControllerB:
                this.containerHandler.Sort(container, true);
                break;

            default: return;
        }

        this.inputHelper.Suppress(e.Button);
        Game1.playSound("Ship");
    }

    private void OnContainerSorting(ContainerSortingEventArgs e)
    {
        if (e.Container.SortInventory is not FeatureOption.Enabled
            || !this.expressionHandler.TryParseExpression(e.Container.SortInventoryBy, out var expression))
        {
            return;
        }

        var copy = e.Container.Items.ToList();
        copy.Sort(expression);
        e.Container.Items.OverwriteWith(copy);
    }

    private void OnInventoryMenuChanged(InventoryMenuChangedEventArgs e)
    {
        var container = this.menuHandler.Bottom.Container;
        var bottom = this.menuHandler.Bottom;

        if (this.menuHandler.CurrentMenu is not ItemGrabMenu itemGrabMenu
            || bottom.InventoryMenu is null
            || container?.SortInventory is not FeatureOption.Enabled)
        {
            this.organizeButton.Value = null;
            return;
        }

        // Add new organize button to the bottom inventory menu
        var x = itemGrabMenu.okButton.bounds.X;
        var y = itemGrabMenu.okButton.bounds.Y - Game1.tileSize - 16;
        this.organizeButton.Value =
            this
                .iconRegistry.Icon(VanillaIcon.Organize)
                .Component(
                    IconStyle.Transparent,
                    x,
                    y,
                    hoverText: Game1.content.LoadString("Strings\\UI:ItemGrab_Organize"));

        itemGrabMenu.trashCan.bounds.Y -= Game1.tileSize;
        itemGrabMenu.okButton.upNeighborID = this.organizeButton.Value.myID;
        itemGrabMenu.trashCan.downNeighborID = this.organizeButton.Value.myID;
        itemGrabMenu.allClickableComponents.Add(this.organizeButton.Value);
    }

    private void OnRenderedActiveMenu(RenderedActiveMenuEventArgs e)
    {
        if (this.organizeButton.Value is null)
        {
            return;
        }

        var cursor = this.inputHelper.GetCursorPosition().GetScaledScreenPixels().ToPoint();
        this.organizeButton.Value.tryHover(cursor.X, cursor.Y);
        this.organizeButton.Value.draw(e.SpriteBatch);
        if (!this.organizeButton.Value.bounds.Contains(cursor))
        {
            return;
        }

        switch (this.menuHandler.CurrentMenu)
        {
            case ItemGrabMenu itemGrabMenu:
                itemGrabMenu.hoverText = this.organizeButton.Value.hoverText;
                return;

            case InventoryPage inventoryPage:
                inventoryPage.hoverText = this.organizeButton.Value.hoverText;
                return;
        }
    }
}