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
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewMods.Common.Helpers;
using StardewMods.Common.Helpers.PatternPatcher;
using StardewMods.CommonHarmony.Enums;
using StardewMods.CommonHarmony.Helpers;
using StardewMods.CommonHarmony.Models;
using StardewValley;
using StardewValley.Menus;

/// <summary>
///     Locks items in inventory so they cannot be stashed.
/// </summary>
internal class SlotLock : IFeature
{
    private const string Id = "furyx639.BetterChests/SlotLock";

    private SlotLock(IModHelper helper, ModConfig config)
    {
        this.Helper = helper;
        this.Config = config;
        HarmonyHelper.AddPatches(
            SlotLock.Id,
            new SavedPatch[]
            {
                new(
                    AccessTools.Method(typeof(InventoryMenu), nameof(InventoryMenu.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(int) }),
                    typeof(SlotLock),
                    nameof(SlotLock.InventoryMenu_draw_transpiler),
                    PatchType.Transpiler),
            });
    }

    private static SlotLock? Instance { get; set; }

    private ModConfig Config { get; }

    private IModHelper Helper { get; }

    private bool IsActivated { get; set; }

    /// <summary>
    ///     Initializes <see cref="SlotLock" />.
    /// </summary>
    /// <param name="helper">SMAPI helper for events, input, and content.</param>
    /// <param name="config">Mod config data.</param>
    /// <returns>Returns an instance of the <see cref="SlotLock" /> class.</returns>
    public static SlotLock Init(IModHelper helper, ModConfig config)
    {
        return SlotLock.Instance ??= new(helper, config);
    }

    /// <inheritdoc />
    public void Activate()
    {
        if (!this.IsActivated)
        {
            this.IsActivated = true;
            HarmonyHelper.ApplyPatches(SlotLock.Id);
            this.Helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }
    }

    /// <inheritdoc />
    public void Deactivate()
    {
        if (this.IsActivated)
        {
            this.IsActivated = false;
            HarmonyHelper.UnapplyPatches(SlotLock.Id);
            this.Helper.Events.Input.ButtonPressed -= this.OnButtonPressed;
        }
    }

    private static IEnumerable<CodeInstruction> InventoryMenu_draw_transpiler(IEnumerable<CodeInstruction> instructions)
    {
        Log.Trace($"Applying patches to {nameof(InventoryMenu)}.{nameof(InventoryMenu.draw)} from {nameof(SlotLock)}");
        IPatternPatcher<CodeInstruction> patcher = new PatternPatcher<CodeInstruction>((c1, c2) => c1.opcode.Equals(c2.opcode) && (c1.operand is null || c1.OperandIs(c2.operand)));

        // ****************************************************************************************
        // Item Tint Patch
        // Replaces all actualInventory with ItemsDisplayed.DisplayedItems(actualInventory)
        // Replaces the tint value for the item slot with SlotLock.Tint to highlight locked slots.
        patcher.AddPatchLoop(
            code =>
            {
                code.RemoveAt(code.Count - 1);
                code.Add(new(OpCodes.Ldarg_0));
                code.Add(new(OpCodes.Ldloc_0));
                code.Add(new(OpCodes.Ldloc_S, (byte)4));
                code.Add(new(OpCodes.Call, AccessTools.Method(typeof(SlotLock), nameof(SlotLock.Tint))));
            },
            new(OpCodes.Call, AccessTools.Method(typeof(Game1), nameof(Game1.getSourceRectForStandardTileSheet))),
            new(OpCodes.Newobj),
            new(OpCodes.Ldloc_0));

        // Fill code buffer
        foreach (var inCode in instructions)
        {
            // Return patched code segments
            foreach (var outCode in patcher.From(inCode))
            {
                yield return outCode;
            }
        }

        // Return remaining code
        foreach (var outCode in patcher.FlushBuffer())
        {
            yield return outCode;
        }

        Log.Trace($"{patcher.AppliedPatches.ToString()} / {patcher.TotalPatches.ToString()} patches applied.");
        if (patcher.AppliedPatches < patcher.TotalPatches)
        {
            Log.Warn("Failed to applied all patches!");
        }
    }

    private static Color Tint(InventoryMenu menu, Color tint, int index)
    {
        switch (Game1.activeClickableMenu)
        {
            case ItemGrabMenu { inventory: { } itemGrabMenu } when ReferenceEquals(itemGrabMenu, menu):
            case GameMenu gameMenu when gameMenu.pages[gameMenu.currentTab] is InventoryPage { inventory: { } inventoryPage } && ReferenceEquals(inventoryPage, menu):
                return menu.actualInventory.ElementAtOrDefault(index)?.modData.ContainsKey("furyx639.BetterChests/LockedSlot") == true
                    ? Color.Red
                    : tint;
            default:
                return tint;
        }
    }

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        var menu = Game1.activeClickableMenu switch
        {
            ItemGrabMenu { inventory: { } itemGrabMenu } => itemGrabMenu,
            GameMenu gameMenu when gameMenu.pages[gameMenu.currentTab] is InventoryPage { inventory: { } inventoryPage } => inventoryPage,
            _ => null,
        };

        if (menu is null)
        {
            return;
        }

        if (!(this.Config.SlotLockHold && e.Button == SButton.MouseLeft && e.IsDown(this.Config.ControlScheme.LockSlot))
            && !(!this.Config.SlotLockHold && e.Button == this.Config.ControlScheme.LockSlot))
        {
            return;
        }

        var (x, y) = Game1.getMousePosition(true);
        var slot = menu.inventory.FirstOrDefault(slot => slot.containsPoint(x, y));
        if (slot is null || !int.TryParse(slot.name, out var index))
        {
            return;
        }

        var item = menu.actualInventory.ElementAtOrDefault(index);
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

        this.Helper.Input.Suppress(e.Button);
    }
}