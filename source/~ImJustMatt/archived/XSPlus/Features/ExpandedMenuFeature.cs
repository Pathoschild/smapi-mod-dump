/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

#nullable disable

namespace XSPlus.Features;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using Common.Extensions;
using Common.Helpers;
using CommonHarmony;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using Services;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

/// <inheritdoc />
internal class ExpandedMenuFeature : FeatureWithParam<int>
{
    private static ExpandedMenuFeature Instance;
    private readonly PerScreen<ItemGrabMenuChangedEventArgs> _menu = new();
    private DisplayedItems _displayedInventory;
    private HarmonyHelper _harmony;
    private ItemGrabMenuChanged _itemGrabMenuChanged;
    private ModConfigService _modConfig;

    private ExpandedMenuFeature(ServiceLocator serviceLocator)
        : base("ExpandedMenu", serviceLocator)
    {
        ExpandedMenuFeature.Instance ??= this;

        // Dependencies
        this.AddDependency<ModConfigService>(service => this._modConfig = service as ModConfigService);
        this.AddDependency<DisplayedItems>(service => this._displayedInventory = service as DisplayedItems);
        this.AddDependency<ItemGrabMenuChanged>(service => this._itemGrabMenuChanged = service as ItemGrabMenuChanged);
        this.AddDependency<HarmonyHelper>(
            service =>
            {
                // Init
                this._harmony = service as HarmonyHelper;
                var ctorItemGrabMenu = new[]
                {
                    typeof(IList<Item>), typeof(bool), typeof(bool), typeof(InventoryMenu.highlightThisItem), typeof(ItemGrabMenu.behaviorOnItemSelect), typeof(string), typeof(ItemGrabMenu.behaviorOnItemSelect), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(int), typeof(Item), typeof(int), typeof(object),
                };

                var drawMenuWithInventory = new[]
                {
                    typeof(SpriteBatch), typeof(bool), typeof(bool), typeof(int), typeof(int), typeof(int),
                };

                // Patches
                this._harmony?.AddPatch(
                    this.ServiceName,
                    AccessTools.Constructor(typeof(ItemGrabMenu), ctorItemGrabMenu),
                    typeof(ExpandedMenuFeature),
                    nameof(ExpandedMenuFeature.ItemGrabMenu_constructor_transpiler),
                    PatchType.Transpiler);

                this._harmony?.AddPatch(
                    this.ServiceName,
                    AccessTools.Method(
                        typeof(ItemGrabMenu),
                        nameof(ItemGrabMenu.draw),
                        new[]
                        {
                            typeof(SpriteBatch),
                        }),
                    typeof(ExpandedMenuFeature),
                    nameof(ExpandedMenuFeature.ItemGrabMenu_draw_transpiler),
                    PatchType.Transpiler);

                this._harmony?.AddPatch(
                    this.ServiceName,
                    AccessTools.Method(typeof(MenuWithInventory), nameof(MenuWithInventory.draw), drawMenuWithInventory),
                    typeof(ExpandedMenuFeature),
                    nameof(ExpandedMenuFeature.MenuWithInventory_draw_transpiler),
                    PatchType.Transpiler);
            });
    }

    /// <inheritdoc />
    public override void Activate()
    {
        // Events
        this._itemGrabMenuChanged.AddHandler(this.OnItemGrabMenuChanged);
        this.Helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
        this.Helper.Events.Input.MouseWheelScrolled += this.OnMouseWheelScrolled;

        // Patches
        this._harmony.ApplyPatches(this.ServiceName);
    }

    /// <inheritdoc />
    public override void Deactivate()
    {
        // Events
        this._itemGrabMenuChanged.RemoveHandler(this.OnItemGrabMenuChanged);
        this.Helper.Events.Input.ButtonsChanged -= this.OnButtonsChanged;
        this.Helper.Events.Input.MouseWheelScrolled -= this.OnMouseWheelScrolled;

        // Patches
        this._harmony.UnapplyPatches(this.ServiceName);
    }

    /// <summary>Generate additional slots/rows for top inventory menu.</summary>
    [SuppressMessage("ReSharper", "HeapView.BoxingAllocation", Justification = "Boxing allocation is required for Harmony.")]
    private static IEnumerable<CodeInstruction> ItemGrabMenu_constructor_transpiler(IEnumerable<CodeInstruction> instructions)
    {
        Log.Trace("Changing jump condition from Beq 36 to Bge 10.");
        var jumpPatch = new PatternPatch();
        jumpPatch
            .Find(
                new[]
                {
                    new CodeInstruction(OpCodes.Isinst, typeof(Chest)), new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(Chest), nameof(Chest.GetActualCapacity))), new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)36), new CodeInstruction(OpCodes.Beq_S),
                })
            .Patch(
                delegate(LinkedList<CodeInstruction> list)
                {
                    var jumpCode = list.Last.Value;
                    list.RemoveLast();
                    list.RemoveLast();
                    list.AddLast(new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)10));
                    list.AddLast(new CodeInstruction(OpCodes.Bge_S, jumpCode.operand));
                });

        Log.Trace("Overriding default values for capacity and rows.");
        var capacityPatch = new PatternPatch();
        capacityPatch
            .Find(
                new[]
                {
                    new CodeInstruction(
                        OpCodes.Newobj,
                        AccessTools.Constructor(
                            typeof(InventoryMenu),
                            new[]
                            {
                                typeof(int), typeof(int), typeof(bool), typeof(IList<Item>), typeof(InventoryMenu.highlightThisItem), typeof(int), typeof(int), typeof(int), typeof(int), typeof(bool),
                            })),
                    new CodeInstruction(OpCodes.Stfld, AccessTools.Field(typeof(ItemGrabMenu), nameof(ItemGrabMenu.ItemsToGrabMenu))),
                })
            .Find(
                new[]
                {
                    new CodeInstruction(OpCodes.Ldc_I4_M1), new CodeInstruction(OpCodes.Ldc_I4_3),
                })
            .Patch(
                delegate(LinkedList<CodeInstruction> list)
                {
                    list.RemoveLast();
                    list.RemoveLast();
                    list.AddLast(new CodeInstruction(OpCodes.Ldarg_0));
                    list.AddLast(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ExpandedMenuFeature), nameof(ExpandedMenuFeature.MenuCapacity))));
                    list.AddLast(new CodeInstruction(OpCodes.Ldarg_0));
                    list.AddLast(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ExpandedMenuFeature), nameof(ExpandedMenuFeature.MenuRows))));
                });

        var patternPatches = new PatternPatches(instructions);
        patternPatches.AddPatch(jumpPatch);
        patternPatches.AddPatch(capacityPatch);

        foreach (var patternPatch in patternPatches)
        {
            yield return patternPatch;
        }

        if (!patternPatches.Done)
        {
            Log.Warn($"Failed to apply all patches in {typeof(ExpandedMenuFeature)}::{nameof(ExpandedMenuFeature.ItemGrabMenu_constructor_transpiler)}");
        }
    }

    /// <summary>Move/resize backpack by expanded menu height.</summary>
    private static IEnumerable<CodeInstruction> ItemGrabMenu_draw_transpiler(IEnumerable<CodeInstruction> instructions)
    {
        Log.Trace("Moving backpack icon down by expanded menu extra height.");
        var moveBackpackPatch = new PatternPatch();
        moveBackpackPatch
            .Find(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ItemGrabMenu), nameof(ItemGrabMenu.showReceivingMenu))))
            .Find(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(IClickableMenu), nameof(IClickableMenu.yPositionOnScreen))))
            .Patch(
                delegate(LinkedList<CodeInstruction> list)
                {
                    list.AddLast(new CodeInstruction(OpCodes.Ldarg_0));
                    list.AddLast(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ExpandedMenuFeature), nameof(ExpandedMenuFeature.MenuOffset))));
                    list.AddLast(new CodeInstruction(OpCodes.Add));
                })
            .Repeat(3);

        var patternPatches = new PatternPatches(instructions, moveBackpackPatch);

        foreach (var patternPatch in patternPatches)
        {
            yield return patternPatch;
        }

        if (!patternPatches.Done)
        {
            Log.Warn($"Failed to apply all patches in {typeof(ItemGrabMenu)}::{nameof(ItemGrabMenu.draw)}.");
        }
    }

    /// <summary>Move/resize bottom dialogue box by search bar height.</summary>
    [SuppressMessage("ReSharper", "HeapView.BoxingAllocation", Justification = "Boxing allocation is required for Harmony.")]
    private static IEnumerable<CodeInstruction> MenuWithInventory_draw_transpiler(IEnumerable<CodeInstruction> instructions)
    {
        Log.Trace("Moving bottom dialogue box down by expanded menu height.");
        var moveDialogueBoxPatch = new PatternPatch();
        moveDialogueBoxPatch
            .Find(
                new[]
                {
                    new CodeInstruction(
                        OpCodes.Ldfld,
                        AccessTools.Field(
                            typeof(IClickableMenu),
                            nameof(IClickableMenu.yPositionOnScreen))),
                    new CodeInstruction(
                        OpCodes.Ldsfld,
                        AccessTools.Field(
                            typeof(IClickableMenu),
                            nameof(IClickableMenu.borderWidth))),
                    new CodeInstruction(OpCodes.Add),
                    new CodeInstruction(
                        OpCodes.Ldsfld,
                        AccessTools.Field(
                            typeof(IClickableMenu),
                            nameof(IClickableMenu.spaceToClearTopBorder))),
                    new CodeInstruction(OpCodes.Add),
                    new CodeInstruction(
                        OpCodes.Ldc_I4_S,
                        (sbyte)64),
                    new CodeInstruction(OpCodes.Add),
                })
            .Patch(
                list =>
                {
                    list.AddLast(new CodeInstruction(OpCodes.Ldarg_0));
                    list.AddLast(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ExpandedMenuFeature), nameof(ExpandedMenuFeature.MenuOffset))));
                    list.AddLast(new CodeInstruction(OpCodes.Add));
                });

        Log.Trace("Shrinking bottom dialogue box height by expanded menu height.");
        var resizeDialogueBoxPatch = new PatternPatch();
        resizeDialogueBoxPatch
            .Find(
                new[]
                {
                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(IClickableMenu), nameof(IClickableMenu.height))), new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(IClickableMenu), nameof(IClickableMenu.borderWidth))), new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(IClickableMenu), nameof(IClickableMenu.spaceToClearTopBorder))), new CodeInstruction(OpCodes.Add), new CodeInstruction(OpCodes.Ldc_I4, 192), new CodeInstruction(OpCodes.Add),
                })
            .Patch(
                list =>
                {
                    list.AddLast(new CodeInstruction(OpCodes.Ldarg_0));
                    list.AddLast(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ExpandedMenuFeature), nameof(ExpandedMenuFeature.MenuOffset))));
                    list.AddLast(new CodeInstruction(OpCodes.Add));
                });

        var patternPatches = new PatternPatches(instructions);
        patternPatches.AddPatch(moveDialogueBoxPatch);
        patternPatches.AddPatch(resizeDialogueBoxPatch);

        foreach (var patternPatch in patternPatches)
        {
            yield return patternPatch;
        }

        if (!patternPatches.Done)
        {
            Log.Warn($"Failed to apply all patches in {typeof(MenuWithInventory)}::{nameof(MenuWithInventory.draw)}.");
        }
    }

    private static int MenuCapacity(MenuWithInventory menu)
    {
        if (menu is not ItemGrabMenu {context: Chest {playerChest.Value: true} chest} || !ExpandedMenuFeature.Instance.IsEnabledForItem(chest))
        {
            return -1; // Vanilla
        }

        var capacity = chest.GetActualCapacity();
        var maxMenuRows = ExpandedMenuFeature.Instance._modConfig.ModConfig.MenuRows;
        return capacity switch
        {
            < 72 => Math.Min(maxMenuRows * 12, capacity.RoundUp(12)), // Variable
            _ => maxMenuRows * 12, // Large
        };
    }

    private static int MenuRows(MenuWithInventory menu)
    {
        if (menu is not ItemGrabMenu {context: Chest {playerChest.Value: true} chest} || !ExpandedMenuFeature.Instance.IsEnabledForItem(chest))
        {
            return 3; // Vanilla
        }

        var capacity = chest.GetActualCapacity();
        var maxMenuRows = ExpandedMenuFeature.Instance._modConfig.ModConfig.MenuRows;
        return capacity switch
        {
            < 72 => (int)Math.Min(maxMenuRows, Math.Ceiling(capacity / 12f)),
            _ => maxMenuRows,
        };
    }

    private static int MenuOffset(MenuWithInventory menu)
    {
        if (menu is not ItemGrabMenu {context: Chest {playerChest.Value: true} chest} || !ExpandedMenuFeature.Instance.IsEnabledForItem(chest))
        {
            return 0; // Vanilla
        }

        var rows = ExpandedMenuFeature.MenuRows(menu);
        return Game1.tileSize * (rows - 3);
    }

    private void OnItemGrabMenuChanged(object sender, ItemGrabMenuChangedEventArgs e)
    {
        if (e.ItemGrabMenu is null || e.Chest is null || !this.IsEnabledForItem(e.Chest))
        {
            this._menu.Value = null;
            return;
        }

        if (e.IsNew)
        {
            var offset = ExpandedMenuFeature.MenuOffset(e.ItemGrabMenu);
            e.ItemGrabMenu.height += offset;
            e.ItemGrabMenu.inventory.movePosition(0, offset);
            if (e.ItemGrabMenu.okButton is not null)
            {
                e.ItemGrabMenu.okButton.bounds.Y += offset;
            }

            if (e.ItemGrabMenu.trashCan is not null)
            {
                e.ItemGrabMenu.trashCan.bounds.Y += offset;
            }

            if (e.ItemGrabMenu.dropItemInvisibleButton is not null)
            {
                e.ItemGrabMenu.dropItemInvisibleButton.bounds.Y += offset;
            }

            e.ItemGrabMenu.RepositionSideButtons();
        }

        this._menu.Value = e;
    }

    private void OnMouseWheelScrolled(object sender, MouseWheelScrolledEventArgs e)
    {
        if (this._menu.Value is null || this._menu.Value.ScreenId != Context.ScreenId)
        {
            return;
        }

        switch (e.Delta)
        {
            case > 0:
                this._displayedInventory.Offset--;
                break;
            case < 0:
                this._displayedInventory.Offset++;
                break;
            default:
                return;
        }
    }

    private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
    {
        if (this._menu.Value is null || this._menu.Value.ScreenId != Context.ScreenId)
        {
            return;
        }

        if (this._modConfig.ModConfig.ScrollUp.JustPressed())
        {
            this._displayedInventory.Offset--;
            this.Helper.Input.SuppressActiveKeybinds(this._modConfig.ModConfig.ScrollUp);
            return;
        }

        if (this._modConfig.ModConfig.ScrollDown.JustPressed())
        {
            this._displayedInventory.Offset++;
            this.Helper.Input.SuppressActiveKeybinds(this._modConfig.ModConfig.ScrollDown);
        }
    }
}