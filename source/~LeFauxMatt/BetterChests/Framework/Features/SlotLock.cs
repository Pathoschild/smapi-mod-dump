/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework.Features;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewMods.Common.Helpers;
using StardewValley.Menus;

/// <summary>
///     Locks items in inventory so they cannot be stashed.
/// </summary>
internal sealed class SlotLock : Feature
{
    private const string Id = "furyx639.BetterChests/SlotLock";

    private static readonly MethodBase InventoryMenuDraw = AccessTools.Method(
        typeof(InventoryMenu),
        nameof(InventoryMenu.draw),
        new[]
        {
            typeof(SpriteBatch),
            typeof(int),
            typeof(int),
            typeof(int),
        });

#nullable disable
    private static SlotLock Instance;
#nullable enable

    private readonly ModConfig _config;
    private readonly Harmony _harmony;
    private readonly IModHelper _helper;

    private SlotLock(IModHelper helper, ModConfig config)
    {
        this._helper = helper;
        this._config = config;
        this._harmony = new(SlotLock.Id);
    }

    /// <summary>
    ///     Initializes <see cref="SlotLock" />.
    /// </summary>
    /// <param name="helper">SMAPI helper for events, input, and content.</param>
    /// <param name="config">Mod config data.</param>
    /// <returns>Returns an instance of the <see cref="SlotLock" /> class.</returns>
    public static Feature Init(IModHelper helper, ModConfig config)
    {
        return SlotLock.Instance ??= new(helper, config);
    }

    /// <inheritdoc />
    protected override void Activate()
    {
        // Events
        this._helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        this._helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;

        // Harmony
        this._harmony.Patch(
            SlotLock.InventoryMenuDraw,
            transpiler: new(typeof(SlotLock), nameof(SlotLock.InventoryMenu_draw_transpiler)));
    }

    /// <inheritdoc />
    protected override void Deactivate()
    {
        // Events
        this._helper.Events.Input.ButtonPressed -= this.OnButtonPressed;
        this._helper.Events.Input.ButtonsChanged -= this.OnButtonsChanged;

        // Harmony
        this._harmony.Unpatch(
            SlotLock.InventoryMenuDraw,
            AccessTools.Method(typeof(SlotLock), nameof(SlotLock.InventoryMenu_draw_transpiler)));
    }

    private static IEnumerable<CodeInstruction> InventoryMenu_draw_transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var instruction in instructions)
        {
            yield return instruction;
            if (instruction.opcode != OpCodes.Ldloc_0)
            {
                continue;
            }

            yield return new(OpCodes.Ldarg_0);
            yield return new(OpCodes.Ldloc_S, (byte)4);
            yield return CodeInstruction.Call(typeof(SlotLock), nameof(SlotLock.Tint));
        }
    }

    private static Color Tint(Color tint, InventoryMenu menu, int index)
    {
        return menu.actualInventory.ElementAtOrDefault(index)?.modData.ContainsKey("furyx639.BetterChests/LockedSlot")
            == true
                ? SlotLock.Instance._config.SlotLockColor.ToColor()
                : tint;
    }

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (!this._config.SlotLockHold
            || e.Button is not SButton.MouseLeft
            || !this._config.ControlScheme.LockSlot.IsDown())
        {
            return;
        }

        var (x, y) = Game1.getMousePosition(true);
        var menu = Game1.activeClickableMenu switch
        {
            ItemGrabMenu { inventory: { } inventory } when inventory.isWithinBounds(x, y) => inventory,
            ItemGrabMenu { ItemsToGrabMenu: { } itemsToGrabMenu } when itemsToGrabMenu.isWithinBounds(x, y) =>
                itemsToGrabMenu,
            GameMenu gameMenu when gameMenu.GetCurrentPage() is InventoryPage { inventory: { } inventoryPage } =>
                inventoryPage,
            _ => null,
        };

        var slot = menu?.inventory.FirstOrDefault(slot => slot.containsPoint(x, y));
        if (slot is null || !int.TryParse(slot.name, out var index))
        {
            return;
        }

        var item = menu?.actualInventory.ElementAtOrDefault(index);
        if (item is null)
        {
            return;
        }

        if (item.modData.ContainsKey("furyx639.BetterChests/LockedSlot"))
        {
            item.modData.Remove("furyx639.BetterChests/LockedSlot");
        }
        else
        {
            item.modData["furyx639.BetterChests/LockedSlot"] = true.ToString();
        }

        this._helper.Input.Suppress(e.Button);
    }

    private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (this._config.SlotLockHold || !this._config.ControlScheme.LockSlot.JustPressed())
        {
            return;
        }

        var (x, y) = Game1.getMousePosition(true);
        var menu = Game1.activeClickableMenu switch
        {
            ItemGrabMenu { inventory: { } inventory } when inventory.isWithinBounds(x, y) => inventory,
            ItemGrabMenu { ItemsToGrabMenu: { } itemsToGrabMenu } when itemsToGrabMenu.isWithinBounds(x, y) =>
                itemsToGrabMenu,
            GameMenu gameMenu when gameMenu.GetCurrentPage() is InventoryPage { inventory: { } inventoryPage } =>
                inventoryPage,
            _ => null,
        };

        var slot = menu?.inventory.FirstOrDefault(slot => slot.containsPoint(x, y));
        if (slot is null || !int.TryParse(slot.name, out var index))
        {
            return;
        }

        var item = menu?.actualInventory.ElementAtOrDefault(index);
        if (item is null)
        {
            return;
        }

        if (item.modData.ContainsKey("furyx639.BetterChests/LockedSlot"))
        {
            item.modData.Remove("furyx639.BetterChests/LockedSlot");
        }
        else
        {
            item.modData["furyx639.BetterChests/LockedSlot"] = true.ToString();
        }

        this._helper.Input.SuppressActiveKeybinds(this._config.ControlScheme.LockSlot);
    }
}