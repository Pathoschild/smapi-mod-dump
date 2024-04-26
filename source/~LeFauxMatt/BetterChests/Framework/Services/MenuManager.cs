/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework.Services;

using System.Globalization;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewMods.BetterChests.Framework.Interfaces;
using StardewMods.BetterChests.Framework.Models.Containers;
using StardewMods.BetterChests.Framework.Models.Events;
using StardewMods.BetterChests.Framework.Services.Factory;
using StardewMods.Common.Enums;
using StardewMods.Common.Interfaces;
using StardewMods.Common.Models;
using StardewMods.Common.Services;
using StardewMods.Common.Services.Integrations.BetterChests.Enums;
using StardewMods.Common.Services.Integrations.FauxCore;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Objects;

/// <summary>Manages the item grab menu in the game.</summary>
internal sealed class MenuManager : BaseService<MenuManager>
{
    private static MenuManager instance = null!;

    private readonly PerScreen<InventoryMenuManager> bottomMenu;
    private readonly ContainerFactory containerFactory;
    private readonly PerScreen<IClickableMenu?> currentMenu = new();
    private readonly IEventManager eventManager;
    private readonly PerScreen<ServiceLock?> focus = new();
    private readonly PerScreen<InventoryMenuManager> topMenu;

    /// <summary>Initializes a new instance of the <see cref="MenuManager" /> class.</summary>
    /// <param name="containerFactory">Dependency used for accessing containers.</param>
    /// <param name="eventManager">Dependency used for managing events.</param>
    /// <param name="log">Dependency used for logging debug information to the console.</param>
    /// <param name="manifest">Dependency for accessing mod manifest.</param>
    /// <param name="modConfig">Dependency used for accessing config data.</param>
    /// <param name="inputHelper">Dependency used for checking and changing input state.</param>
    /// <param name="patchManager">Dependency used for managing patches.</param>
    public MenuManager(
        ContainerFactory containerFactory,
        IEventManager eventManager,
        ILog log,
        IManifest manifest,
        IModConfig modConfig,
        IInputHelper inputHelper,
        IPatchManager patchManager)
        : base(log, manifest)
    {
        // Init
        MenuManager.instance = this;
        this.containerFactory = containerFactory;
        this.eventManager = eventManager;

        this.topMenu = new PerScreen<InventoryMenuManager>(
            () => new InventoryMenuManager(eventManager, inputHelper, log, manifest, modConfig));

        this.bottomMenu = new PerScreen<InventoryMenuManager>(
            () => new InventoryMenuManager(eventManager, inputHelper, log, manifest, modConfig));

        // Events
        eventManager.Subscribe<RenderingActiveMenuEventArgs>(this.OnRenderingActiveMenu);
        eventManager.Subscribe<RenderedActiveMenuEventArgs>(this.OnRenderedActiveMenu);
        eventManager.Subscribe<UpdateTickingEventArgs>(this.OnUpdateTicking);
        eventManager.Subscribe<UpdateTickedEventArgs>(this.OnUpdateTicked);
        eventManager.Subscribe<WindowResizedEventArgs>(this.OnWindowResized);

        // Patches
        patchManager.Add(
            this.UniqueId,
            new SavedPatch(
                AccessTools.DeclaredMethod(typeof(IClickableMenu), nameof(IClickableMenu.SetChildMenu)),
                AccessTools.DeclaredMethod(
                    typeof(MenuManager),
                    nameof(MenuManager.IClickableMenu_SetChildMenu_postfix)),
                PatchType.Postfix),
            new SavedPatch(
                AccessTools.DeclaredMethod(
                    typeof(InventoryMenu),
                    nameof(InventoryMenu.draw),
                    [typeof(SpriteBatch), typeof(int), typeof(int), typeof(int)]),
                AccessTools.DeclaredMethod(typeof(MenuManager), nameof(MenuManager.InventoryMenu_draw_prefix)),
                PatchType.Prefix),
            new SavedPatch(
                AccessTools.DeclaredMethod(
                    typeof(InventoryMenu),
                    nameof(InventoryMenu.draw),
                    [typeof(SpriteBatch), typeof(int), typeof(int), typeof(int)]),
                AccessTools.DeclaredMethod(typeof(MenuManager), nameof(MenuManager.InventoryMenu_draw_postfix)),
                PatchType.Postfix),
            new SavedPatch(
                AccessTools
                    .GetDeclaredConstructors(typeof(ItemGrabMenu))
                    .Single(ctor => ctor.GetParameters().Length > 5),
                AccessTools.DeclaredMethod(
                    typeof(MenuManager),
                    nameof(MenuManager.ItemGrabMenu_constructor_transpiler)),
                PatchType.Transpiler));

        patchManager.Patch(this.UniqueId);
    }

    /// <summary>Gets the current menu.</summary>
    public IClickableMenu? CurrentMenu
    {
        get => this.currentMenu.Value;
        private set => this.currentMenu.Value = value;
    }

    /// <summary>Gets the inventory menu manager for the top inventory menu.</summary>
    public IInventoryMenuManager Top => this.topMenu.Value;

    /// <summary>Gets the inventory menu manager for the bottom inventory menu.</summary>
    public IInventoryMenuManager Bottom => this.bottomMenu.Value;

    /// <summary>Determines if the specified source object can receive focus.</summary>
    /// <param name="source">The object to check if it can receive focus.</param>
    /// <returns>true if the source object can receive focus; otherwise, false.</returns>
    public bool CanFocus(object source) => this.focus.Value is null || this.focus.Value.Source == source;

    /// <summary>Tries to request focus for a specific object.</summary>
    /// <param name="source">The object that needs focus.</param>
    /// <param name="serviceLock">
    /// An optional output parameter representing the acquired service lock, or null if failed to
    /// acquire.
    /// </param>
    /// <returns>true if focus was successfully acquired; otherwise, false.</returns>
    public bool TryGetFocus(object source, [NotNullWhen(true)] out IServiceLock? serviceLock)
    {
        serviceLock = null;
        if (this.focus.Value is not null && this.focus.Value.Source != source)
        {
            return false;
        }

        if (this.focus.Value is not null && this.focus.Value.Source == source)
        {
            serviceLock = this.focus.Value;
            return true;
        }

        this.focus.Value = new ServiceLock(source, this);
        serviceLock = this.focus.Value;
        return true;
    }

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Harmony")]
    [SuppressMessage("StyleCop", "SA1313", Justification = "Harmony")]
    private static void IClickableMenu_SetChildMenu_postfix() => MenuManager.instance.UpdateMenu();

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Harmony")]
    [SuppressMessage("ReSharper", "RedundantAssignment", Justification = "Harmony")]
    [SuppressMessage("StyleCop", "SA1313", Justification = "Harmony")]
    private static void InventoryMenu_draw_prefix(InventoryMenu __instance, ref InventoryMenuManager? __state)
    {
        __state = __instance.Equals(MenuManager.instance.topMenu.Value.Menu)
            ? MenuManager.instance.topMenu.Value
            : __instance.Equals(MenuManager.instance.bottomMenu.Value.Menu)
                ? MenuManager.instance.bottomMenu.Value
                : null;

        if (__state?.Container is null)
        {
            return;
        }

        // Apply operations
        var itemsDisplayingEventArgs = new ItemsDisplayingEventArgs(__state.Container);
        MenuManager.instance.eventManager.Publish(itemsDisplayingEventArgs);
        __instance.actualInventory = itemsDisplayingEventArgs.Items.ToList();

        var defaultName = int.MaxValue.ToString(CultureInfo.InvariantCulture);
        var emptyIndex = -1;
        for (var index = 0; index < __instance.inventory.Count; ++index)
        {
            if (index >= __instance.actualInventory.Count)
            {
                __instance.inventory[index].name = defaultName;
                continue;
            }

            if (__instance.actualInventory[index] is null)
            {
                // Iterate to next empty index
                while (++emptyIndex < __state.Container.Items.Count
                    && __state.Container.Items[emptyIndex] is not null) { }

                if (emptyIndex >= __state.Container.Items.Count)
                {
                    __instance.inventory[index].name = defaultName;
                    continue;
                }

                __instance.inventory[index].name = emptyIndex.ToString(CultureInfo.InvariantCulture);
                continue;
            }

            var actualIndex = __state.Container.Items.IndexOf(__instance.actualInventory[index]);
            __instance.inventory[index].name =
                actualIndex > -1 ? actualIndex.ToString(CultureInfo.InvariantCulture) : defaultName;
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Harmony")]
    [SuppressMessage("ReSharper", "RedundantAssignment", Justification = "Harmony")]
    [SuppressMessage("StyleCop", "SA1313", Justification = "Harmony")]
    private static void InventoryMenu_draw_postfix(InventoryMenu __instance, ref InventoryMenuManager? __state)
    {
        __state = __instance.Equals(MenuManager.instance.topMenu.Value.Menu)
            ? MenuManager.instance.topMenu.Value
            : __instance.Equals(MenuManager.instance.bottomMenu.Value.Menu)
                ? MenuManager.instance.bottomMenu.Value
                : null;

        if (__state?.Container is null)
        {
            return;
        }

        // Restore original
        __instance.actualInventory = __state.Container.Items;
    }

    private static IEnumerable<CodeInstruction>
        ItemGrabMenu_constructor_transpiler(IEnumerable<CodeInstruction> instructions) =>
        new CodeMatcher(instructions)
            .MatchStartForward(new CodeMatch(OpCodes.Stloc_1))
            .Advance(-1)
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_S, (short)16),
                new CodeInstruction(
                    OpCodes.Call,
                    AccessTools.DeclaredMethod(typeof(MenuManager), nameof(MenuManager.GetChestContext))))
            .MatchStartForward(
                new CodeMatch(
                    instruction => instruction.Calls(
                        AccessTools.DeclaredMethod(typeof(Chest), nameof(Chest.GetActualCapacity)))))
            .Advance(1)
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_S, (short)16),
                new CodeInstruction(
                    OpCodes.Call,
                    AccessTools.DeclaredMethod(typeof(MenuManager), nameof(MenuManager.GetMenuCapacity))))
            .MatchStartForward(
                new CodeMatch(
                    instruction => instruction.Calls(
                        AccessTools.DeclaredMethod(typeof(Chest), nameof(Chest.GetActualCapacity)))))
            .Advance(1)
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_S, (short)16),
                new CodeInstruction(
                    OpCodes.Call,
                    AccessTools.DeclaredMethod(typeof(MenuManager), nameof(MenuManager.GetMenuCapacity))))
            .InstructionEnumeration();

    private static object? GetChestContext(Item? sourceItem, object? context) =>
        context switch
        {
            Chest chest => chest,
            SObject
            {
                heldObject.Value: Chest heldChest,
            } => heldChest,
            Building building when building.buildingChests.Any() => building.buildingChests.First(),
            GameLocation location when location.GetFridge() is Chest fridge => fridge,
            _ => sourceItem,
        };

    private static int GetMenuCapacity(int capacity, object? context)
    {
        switch (context)
        {
            case Item item when MenuManager.instance.containerFactory.TryGetOne(item, out var container):
            case Building building when MenuManager.instance.containerFactory.TryGetOne(building, out container):
                return container.Options.ResizeChest switch
                {
                    ChestMenuOption.Small => 9,
                    ChestMenuOption.Medium => 36,
                    ChestMenuOption.Large => 70,
                    _ when capacity is > 70 or -1 => 70,
                    _ => capacity,
                };

            default: return capacity > 70 ? 70 : capacity;
        }
    }

    private void OnUpdateTicking(UpdateTickingEventArgs e) => this.UpdateMenu();

    private void OnUpdateTicked(UpdateTickedEventArgs e) => this.UpdateMenu();

    private void UpdateHighlightMethods()
    {
        switch (this.CurrentMenu)
        {
            case ItemGrabMenu itemGrabMenu:
                if (itemGrabMenu.ItemsToGrabMenu.highlightMethod != this.topMenu.Value.HighlightMethod)
                {
                    this.topMenu.Value.OriginalHighlightMethod = itemGrabMenu.ItemsToGrabMenu.highlightMethod;
                    itemGrabMenu.ItemsToGrabMenu.highlightMethod = this.topMenu.Value.HighlightMethod;
                }

                if (itemGrabMenu.inventory.highlightMethod != this.bottomMenu.Value.HighlightMethod)
                {
                    this.bottomMenu.Value.OriginalHighlightMethod = itemGrabMenu.inventory.highlightMethod;
                    itemGrabMenu.inventory.highlightMethod = this.bottomMenu.Value.HighlightMethod;
                }

                break;

            case InventoryPage inventoryPage:
                if (inventoryPage.inventory.highlightMethod != this.bottomMenu.Value.HighlightMethod)
                {
                    this.bottomMenu.Value.OriginalHighlightMethod = inventoryPage.inventory.highlightMethod;
                    inventoryPage.inventory.highlightMethod = this.bottomMenu.Value.HighlightMethod;
                }

                break;
        }
    }

    private void OnWindowResized(WindowResizedEventArgs e) => this.Top.Container?.ShowMenu();

    private void UpdateMenu()
    {
        var menu = Game1.activeClickableMenu switch
        {
            { } menuWithChild when menuWithChild.GetChildMenu() is
                { } childMenu => childMenu,
            GameMenu gameMenu => gameMenu.GetCurrentPage(),
            _ => Game1.activeClickableMenu,
        };

        if (menu == this.CurrentMenu)
        {
            this.UpdateHighlightMethods();
            return;
        }

        this.CurrentMenu = menu;
        this.focus.Value = null;
        IClickableMenu? parentMenu = null;
        InventoryMenu? top = null;
        InventoryMenu? bottom = null;
        var itemGrabMenu = menu as ItemGrabMenu;

        if (itemGrabMenu is not null)
        {
            parentMenu = itemGrabMenu;
            top = itemGrabMenu.ItemsToGrabMenu;
            bottom = itemGrabMenu.inventory;

            // Disable background fade
            itemGrabMenu.setBackgroundTransparency(false);
        }
        else if (menu is InventoryPage inventoryPage)
        {
            parentMenu = inventoryPage;
            bottom = inventoryPage.inventory;
        }

        this.topMenu.Value.Reset(parentMenu, top);
        this.bottomMenu.Value.Reset(parentMenu, bottom);

        if (parentMenu is null)
        {
            this.eventManager.Publish(new InventoryMenuChangedEventArgs());
            return;
        }

        // Update top menu
        if (this.containerFactory.TryGetOne(out var topContainer) && itemGrabMenu is not null)
        {
            // Relaunch shipping bin menu
            if (itemGrabMenu.shippingBin
                && topContainer is BuildingContainer
                {
                    Options.ResizeChest: not (ChestMenuOption.Default or ChestMenuOption.Disabled),
                })
            {
                topContainer.ShowMenu();
                return;
            }

            itemGrabMenu.behaviorFunction = topContainer.GrabItemFromInventory;
            itemGrabMenu.behaviorOnItemGrab = topContainer.GrabItemFromChest;
        }

        this.topMenu.Value.Container = topContainer;

        // Update bottom menu
        if (bottom?.actualInventory.Equals(Game1.player.Items) != true
            || !this.containerFactory.TryGetOne(Game1.player, out var bottomContainer))
        {
            bottomContainer = null;
        }

        this.bottomMenu.Value.Container = bottomContainer;

        // Reset filters
        this.UpdateHighlightMethods();
        this.eventManager.Publish(new InventoryMenuChangedEventArgs());
    }

    [Priority(int.MaxValue)]
    private void OnRenderingActiveMenu(RenderingActiveMenuEventArgs e)
    {
        if (Game1.options.showClearBackgrounds)
        {
            return;
        }

        switch (this.CurrentMenu)
        {
            case ItemGrabMenu:
                // Redraw background
                e.SpriteBatch.Draw(
                    Game1.fadeToBlackRect,
                    new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height),
                    Color.Black * 0.5f);

                break;

            case InventoryPage: break;
            default: return;
        }

        Game1.mouseCursorTransparency = 0f;
    }

    [Priority(int.MinValue)]
    private void OnRenderedActiveMenu(RenderedActiveMenuEventArgs e)
    {
        switch (this.CurrentMenu)
        {
            case ItemGrabMenu itemGrabMenu:
                // Draw overlay
                this.topMenu.Value.Draw(e.SpriteBatch);
                this.bottomMenu.Value.Draw(e.SpriteBatch);

                // Redraw foreground
                if (this.focus.Value is null)
                {
                    if (itemGrabMenu.hoverText != null
                        && (itemGrabMenu.hoveredItem == null || itemGrabMenu.ItemsToGrabMenu == null))
                    {
                        if (itemGrabMenu.hoverAmount > 0)
                        {
                            IClickableMenu.drawToolTip(
                                e.SpriteBatch,
                                itemGrabMenu.hoverText,
                                string.Empty,
                                null,
                                true,
                                -1,
                                0,
                                null,
                                -1,
                                null,
                                itemGrabMenu.hoverAmount);
                        }
                        else
                        {
                            IClickableMenu.drawHoverText(e.SpriteBatch, itemGrabMenu.hoverText, Game1.smallFont);
                        }
                    }

                    if (itemGrabMenu.hoveredItem != null)
                    {
                        IClickableMenu.drawToolTip(
                            e.SpriteBatch,
                            itemGrabMenu.hoveredItem.getDescription(),
                            itemGrabMenu.hoveredItem.DisplayName,
                            itemGrabMenu.hoveredItem,
                            itemGrabMenu.heldItem != null);
                    }
                    else if (itemGrabMenu.hoveredItem != null && itemGrabMenu.ItemsToGrabMenu != null)
                    {
                        IClickableMenu.drawToolTip(
                            e.SpriteBatch,
                            itemGrabMenu.ItemsToGrabMenu.descriptionText,
                            itemGrabMenu.ItemsToGrabMenu.descriptionTitle,
                            itemGrabMenu.hoveredItem,
                            itemGrabMenu.heldItem != null);
                    }

                    itemGrabMenu.heldItem?.drawInMenu(
                        e.SpriteBatch,
                        new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8),
                        1f);
                }

                break;

            case InventoryPage inventoryPage:
                // Draw overlay
                this.topMenu.Value.Draw(e.SpriteBatch);
                this.bottomMenu.Value.Draw(e.SpriteBatch);

                // Redraw foreground
                if (this.focus.Value is null)
                {
                    if (!string.IsNullOrEmpty(inventoryPage.hoverText))
                    {
                        if (inventoryPage.hoverAmount > 0)
                        {
                            IClickableMenu.drawToolTip(
                                e.SpriteBatch,
                                inventoryPage.hoverText,
                                inventoryPage.hoverTitle,
                                null,
                                true,
                                -1,
                                0,
                                null,
                                -1,
                                null,
                                inventoryPage.hoverAmount);
                        }
                        else
                        {
                            IClickableMenu.drawToolTip(
                                e.SpriteBatch,
                                inventoryPage.hoverText,
                                inventoryPage.hoverTitle,
                                inventoryPage.hoveredItem,
                                Game1.player.CursorSlotItem is not null);
                        }
                    }
                }

                break;

            default: return;
        }

        Game1.mouseCursorTransparency = 1f;
        Game1.activeClickableMenu.drawMouse(e.SpriteBatch);
    }

    private sealed class ServiceLock(object source, MenuManager menuManager) : IServiceLock
    {
        public object Source => source;

        public void Release()
        {
            if (menuManager.focus.Value == this)
            {
                menuManager.focus.Value = null;
            }
        }
    }
}