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
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewMods.BetterChests.Framework.Interfaces;
using StardewMods.BetterChests.Framework.Models.Events;
using StardewMods.Common.Helpers;
using StardewMods.Common.Interfaces;
using StardewMods.Common.Models;
using StardewMods.Common.Services.Integrations.BetterChests.Enums;
using StardewMods.Common.Services.Integrations.FauxCore;
using StardewValley.Menus;

/// <summary>Locks items in inventory so they cannot be stashed.</summary>
internal sealed class LockItem : BaseFeature<LockItem>
{
    private readonly IInputHelper inputHelper;
    private readonly MenuManager menuManager;

    /// <summary>Initializes a new instance of the <see cref="LockItem" /> class.</summary>
    /// <param name="eventManager">Dependency used for managing events.</param>
    /// <param name="inputHelper">Dependency used for checking and changing input state.</param>
    /// <param name="menuManager">Dependency used for managing the current menu.</param>
    /// <param name="log">Dependency used for logging debug information to the console.</param>
    /// <param name="manifest">Dependency for accessing mod manifest.</param>
    /// <param name="modConfig">Dependency used for accessing config data.</param>
    public LockItem(
        IEventManager eventManager,
        IInputHelper inputHelper,
        MenuManager menuManager,
        ILog log,
        IManifest manifest,
        IModConfig modConfig)
        : base(eventManager, log, manifest, modConfig)
    {
        this.inputHelper = inputHelper;
        this.menuManager = menuManager;
    }

    /// <inheritdoc />
    public override bool ShouldBeActive => this.Config.LockItem != FeatureOption.Disabled;

    /// <inheritdoc />
    protected override void Activate()
    {
        // Events
        this.Events.Subscribe<RenderedActiveMenuEventArgs>(this.OnRenderedActiveMenu);
        this.Events.Subscribe<ButtonPressedEventArgs>(this.OnButtonPressed);
        this.Events.Subscribe<ButtonsChangedEventArgs>(this.OnButtonsChanged);
        this.Events.Subscribe<ItemTransferringEventArgs>(this.OnItemTransferring);
        this.Events.Subscribe<ItemHighlightingEventArgs>(this.OnItemHighlighting);
    }

    /// <inheritdoc />
    protected override void Deactivate()
    {
        // Events
        this.Events.Unsubscribe<RenderedActiveMenuEventArgs>(this.OnRenderedActiveMenu);
        this.Events.Unsubscribe<ButtonPressedEventArgs>(this.OnButtonPressed);
        this.Events.Unsubscribe<ButtonsChangedEventArgs>(this.OnButtonsChanged);
        this.Events.Unsubscribe<ItemTransferringEventArgs>(this.OnItemTransferring);
        this.Events.Unsubscribe<ItemHighlightingEventArgs>(this.OnItemHighlighting);
    }

    private bool TryGetMenu(int mouseX, int mouseY, [NotNullWhen(true)] out InventoryMenu? inventoryMenu)
    {
        inventoryMenu = this.menuManager.CurrentMenu switch
        {
            ItemGrabMenu
            {
                inventory:
                { } inventory,
            } when inventory.isWithinBounds(mouseX, mouseY) => inventory,
            ItemGrabMenu
            {
                ItemsToGrabMenu:
                { } itemsToGrabMenu,
            } when itemsToGrabMenu.isWithinBounds(mouseX, mouseY) => itemsToGrabMenu,
            InventoryPage
            {
                inventory:
                { } inventoryPage,
            } => inventoryPage,
            _ => null,
        };

        return inventoryMenu is not null;
    }

    private void OnRenderedActiveMenu(RenderedActiveMenuEventArgs e)
    {
        if (this.menuManager.Top.Menu is not null)
        {
            this.DrawOverlay(e.SpriteBatch, this.menuManager.Top.Menu);
        }

        if (this.menuManager.Bottom.Menu is not null)
        {
            this.DrawOverlay(e.SpriteBatch, this.menuManager.Bottom.Menu);
        }
    }

    private void DrawOverlay(SpriteBatch spriteBatch, InventoryMenu inventoryMenu)
    {
        foreach (var slot in inventoryMenu.inventory)
        {
            var index = slot.name.GetInt(-1);
            if (index == -1)
            {
                continue;
            }

            var item = inventoryMenu.actualInventory.ElementAtOrDefault(index);
            if (item is null || this.IsUnlocked(item))
            {
                continue;
            }

            var x = slot.bounds.X + slot.bounds.Width - 18;
            var y = slot.bounds.Y + slot.bounds.Height - 18;
            spriteBatch.Draw(
                Game1.mouseCursors,
                new Vector2(x - 40, y - 40),
                new Rectangle(107, 442, 7, 8),
                Color.White,
                0f,
                Vector2.Zero,
                2,
                SpriteEffects.None,
                1f);
        }
    }

    private void OnButtonPressed(ButtonPressedEventArgs e)
    {
        if (!this.Config.LockItemHold || e.Button is not SButton.MouseLeft || !this.Config.Controls.LockSlot.IsDown())
        {
            return;
        }

        var (mouseX, mouseY) = Game1.getMousePosition(true);
        if (!this.TryGetMenu(mouseX, mouseY, out var inventoryMenu))
        {
            return;
        }

        var slot = inventoryMenu.inventory.FirstOrDefault(slot => slot.containsPoint(mouseX, mouseY));
        var index = slot?.name.GetInt(-1) ?? -1;
        if (index == -1)
        {
            return;
        }

        var item = inventoryMenu.actualInventory.ElementAtOrDefault(index);
        if (item is null)
        {
            return;
        }

        this.inputHelper.Suppress(e.Button);
        this.ToggleLock(item);
    }

    private void OnButtonsChanged(ButtonsChangedEventArgs e)
    {
        if (this.Config.LockItemHold || !this.Config.Controls.LockSlot.JustPressed())
        {
            return;
        }

        var (mouseX, mouseY) = Game1.getMousePosition(true);
        if (!this.TryGetMenu(mouseX, mouseY, out var inventoryMenu))
        {
            return;
        }

        var slot = inventoryMenu.inventory.FirstOrDefault(slot => slot.containsPoint(mouseX, mouseY));
        var index = slot?.name.GetInt(-1) ?? -1;
        if (index == -1)
        {
            return;
        }

        var item = inventoryMenu.actualInventory.ElementAtOrDefault(index);
        if (item is null)
        {
            return;
        }

        this.inputHelper.SuppressActiveKeybinds(this.Config.Controls.LockSlot);
        this.ToggleLock(item);
    }

    private void OnItemHighlighting(ItemHighlightingEventArgs e)
    {
        if (!this.IsUnlocked(e.Item))
        {
            e.UnHighlight();
        }
    }

    [Priority(int.MaxValue)]
    private void OnItemTransferring(ItemTransferringEventArgs e)
    {
        if (!this.IsUnlocked(e.Item))
        {
            e.PreventTransfer();
        }
    }

    private void ToggleLock(Item item)
    {
        if (this.IsUnlocked(item))
        {
            this.Log.Info("{0}: Locking item {1}", this.Id, item.DisplayName);
            item.modData[this.UniqueId] = "Locked";
        }
        else
        {
            this.Log.Info("{0}: Unlocking item {1}", this.Id, item.DisplayName);
            item.modData.Remove(this.UniqueId);
        }
    }

    private bool IsUnlocked(Item item) =>
        !item.modData.TryGetValue(this.UniqueId, out var locked) || locked != "Locked";
}