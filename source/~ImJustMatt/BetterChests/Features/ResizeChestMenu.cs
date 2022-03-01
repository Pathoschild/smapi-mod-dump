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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using Common.Extensions;
using Common.Helpers;
using Common.Helpers.PatternPatcher;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewMods.BetterChests.Enums;
using StardewMods.BetterChests.Interfaces.Config;
using StardewMods.FuryCore.Attributes;
using StardewMods.FuryCore.Enums;
using StardewMods.FuryCore.Interfaces;
using StardewMods.FuryCore.Interfaces.CustomEvents;
using StardewMods.FuryCore.Models;
using StardewMods.FuryCore.UI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

/// <inheritdoc />
internal class ResizeChestMenu : Feature
{
    private readonly PerScreen<int> _currentOffset = new();
    private readonly Lazy<IHarmonyHelper> _harmony;
    private readonly PerScreen<MenuWithInventory> _menu = new();
    private readonly PerScreen<int?> _menuCapacity = new();
    private readonly PerScreen<int?> _menuOffset = new();
    private readonly PerScreen<int?> _menuRows = new();
    private readonly PerScreen<IStorageData> _storageData = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="ResizeChestMenu" /> class.
    /// </summary>
    /// <param name="config">Data for player configured mod options.</param>
    /// <param name="helper">SMAPI helper for events, input, and content.</param>
    /// <param name="services">Provides access to internal and external services.</param>
    public ResizeChestMenu(IConfigModel config, IModHelper helper, IModServices services)
        : base(config, helper, services)
    {
        ResizeChestMenu.Instance = this;
        this._harmony = services.Lazy<IHarmonyHelper>(
            harmony =>
            {
                var ctorItemGrabMenu = new[]
                {
                    typeof(IList<Item>), typeof(bool), typeof(bool), typeof(InventoryMenu.highlightThisItem), typeof(ItemGrabMenu.behaviorOnItemSelect), typeof(string), typeof(ItemGrabMenu.behaviorOnItemSelect), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(int), typeof(Item), typeof(int), typeof(object),
                };

                var drawMenuWithInventory = new[]
                {
                    typeof(SpriteBatch), typeof(bool), typeof(bool), typeof(int), typeof(int), typeof(int),
                };

                harmony.AddPatches(
                    this.Id,
                    new SavedPatch[]
                    {
                        new(
                            AccessTools.Constructor(typeof(ItemGrabMenu), ctorItemGrabMenu),
                            typeof(ResizeChestMenu),
                            nameof(ResizeChestMenu.ItemGrabMenu_constructor_transpiler),
                            PatchType.Transpiler),
                        new(
                            AccessTools.Method(
                                typeof(ItemGrabMenu),
                                nameof(ItemGrabMenu.draw),
                                new[]
                                {
                                    typeof(SpriteBatch),
                                }),
                            typeof(ResizeChestMenu),
                            nameof(ResizeChestMenu.ItemGrabMenu_draw_transpiler),
                            PatchType.Transpiler),
                        new(
                            AccessTools.Method(typeof(MenuWithInventory), nameof(MenuWithInventory.draw), drawMenuWithInventory),
                            typeof(ResizeChestMenu),
                            nameof(ResizeChestMenu.MenuWithInventory_draw_transpiler),
                            PatchType.Transpiler),
                    });
            });
    }

    private static ResizeChestMenu Instance { get; set; }

    private int CurrentOffset
    {
        set
        {
            if (this._currentOffset.Value == value)
            {
                return;
            }

            var relativeOffset = value - this._currentOffset.Value;
            this._currentOffset.Value = value;
            if (this.Menu is null || relativeOffset == 0)
            {
                return;
            }

            this.Menu.height += relativeOffset;
            this.Menu.inventory.movePosition(0, relativeOffset);
            if (this.Menu.okButton is not null)
            {
                this.Menu.okButton.bounds.Y += relativeOffset;
            }

            if (this.Menu.trashCan is not null)
            {
                this.Menu.trashCan.bounds.Y += relativeOffset;
            }

            if (this.Menu.dropItemInvisibleButton is not null)
            {
                this.Menu.dropItemInvisibleButton.bounds.Y += relativeOffset;
            }
        }
    }

    private IHarmonyHelper HarmonyHelper
    {
        get => this._harmony.Value;
    }

    private MenuWithInventory Menu
    {
        get => this._menu.Value;
        set
        {
            this._menu.Value = value;
            this._menuCapacity.Value = null;
            this._menuRows.Value = null;
            this._menuOffset.Value = null;
        }
    }

    private int MenuCapacity
    {
        get
        {
            return this._menuCapacity.Value ??= this.StorageData is not null
                ? this.StorageData.ResizeChestCapacity switch
                {
                    0 or Chest.capacity => -1,
                    < 0 => this.StorageData.ResizeChestMenuRows * 12,
                    < 72 => Math.Min(this.StorageData.ResizeChestMenuRows * 12, this.StorageData.ResizeChestCapacity.RoundUp(12)),
                    _ => this.StorageData.ResizeChestMenuRows * 12,
                }
                : -1;
        }
    }

    private int MenuOffset
    {
        get => Game1.tileSize * (this.MenuRows - 3);
    }

    private int MenuRows
    {
        get => this._menuRows.Value ??=
            this.StorageData?.ResizeChestCapacity switch
            {
                null => 3,
                0 => 3,
                < 0 => this.StorageData.ResizeChestMenuRows,
                < 72 => (int)Math.Min(this.StorageData.ResizeChestMenuRows, Math.Ceiling(this.StorageData.ResizeChestCapacity / 12f)),
                _ => this.StorageData.ResizeChestMenuRows,
            };
    }

    private IStorageData StorageData
    {
        get => this._storageData.Value;
        set => this._storageData.Value = value;
    }

    /// <inheritdoc />
    protected override void Activate()
    {
        this.HarmonyHelper.ApplyPatches(this.Id);
        this.CustomEvents.ClickableMenuChanged += this.OnClickableMenuChanged;
    }

    /// <inheritdoc />
    protected override void Deactivate()
    {
        this.HarmonyHelper.UnapplyPatches(this.Id);
        this.CustomEvents.ClickableMenuChanged -= this.OnClickableMenuChanged;
    }

    private static int GetMenuCapacity(MenuWithInventory menu)
    {
        if (!ReferenceEquals(ResizeChestMenu.Instance.Menu, menu))
        {
            ResizeChestMenu.Instance.Menu = menu;
            ResizeChestMenu.Instance.StorageData = menu switch
            {
                ItemSelectionMenu when ResizeChestMenu.Instance.Config.DefaultChest.ResizeChestMenu == FeatureOption.Enabled => ResizeChestMenu.Instance.Config.DefaultChest,
                ItemGrabMenu { context: not null } itemGrabMenu when ResizeChestMenu.Instance.ManagedObjects.TryGetManagedStorage(itemGrabMenu.context, out var managedStorage) && managedStorage.ResizeChestMenu == FeatureOption.Enabled => managedStorage,
                _ => null,
            };
        }

        return ResizeChestMenu.Instance.MenuCapacity;
    }

    private static int GetMenuOffset(MenuWithInventory menu)
    {
        if (!ReferenceEquals(ResizeChestMenu.Instance.Menu, menu))
        {
            ResizeChestMenu.Instance.Menu = menu;
            ResizeChestMenu.Instance.StorageData = menu switch
            {
                ItemSelectionMenu when ResizeChestMenu.Instance.Config.DefaultChest.ResizeChestMenu == FeatureOption.Enabled => ResizeChestMenu.Instance.Config.DefaultChest,
                ItemGrabMenu { context: not null } itemGrabMenu when ResizeChestMenu.Instance.ManagedObjects.TryGetManagedStorage(itemGrabMenu.context, out var managedStorage) && managedStorage.ResizeChestMenu == FeatureOption.Enabled => managedStorage,
                _ => null,
            };
        }

        return ResizeChestMenu.Instance.MenuOffset;
    }

    private static int GetMenuRows(MenuWithInventory menu)
    {
        if (!ReferenceEquals(ResizeChestMenu.Instance.Menu, menu))
        {
            ResizeChestMenu.Instance.Menu = menu;
            ResizeChestMenu.Instance.StorageData = menu switch
            {
                ItemSelectionMenu when ResizeChestMenu.Instance.Config.DefaultChest.ResizeChestMenu == FeatureOption.Enabled => ResizeChestMenu.Instance.Config.DefaultChest,
                ItemGrabMenu { context: not null } itemGrabMenu when ResizeChestMenu.Instance.ManagedObjects.TryGetManagedStorage(itemGrabMenu.context, out var managedStorage) && managedStorage.ResizeChestMenu == FeatureOption.Enabled => managedStorage,
                _ => null,
            };
        }

        return ResizeChestMenu.Instance.MenuRows;
    }

    /// <summary>Generate additional slots/rows for top inventory menu.</summary>
    [SuppressMessage("ReSharper", "HeapView.BoxingAllocation", Justification = "Boxing allocation is required for Harmony.")]
    private static IEnumerable<CodeInstruction> ItemGrabMenu_constructor_transpiler(IEnumerable<CodeInstruction> instructions)
    {
        Log.Trace($"Applying patches to {nameof(ItemGrabMenu)}.ctor");
        IPatternPatcher<CodeInstruction> patcher = new PatternPatcher<CodeInstruction>((c1, c2) => c1.opcode.Equals(c2.opcode) && (c1.operand is null || c1.OperandIs(c2.operand)));

        // ****************************************************************************************
        // Jump Condition Patch
        // Original:
        //      if (source == 1 && sourceItem != null && sourceItem is Chest && (sourceItem as Chest).GetActualCapacity() != 36)
        // Patched:
        //      if (source == 1 && sourceItem != null && sourceItem is Chest && (sourceItem as Chest).GetActualCapacity() >= 10)
        //
        // This forces (InventoryMenu) ItemsToGrabMenu to be instantiated with the a capacity of 36
        // and prevents large capacity chests from freezing the game and leaking memory
        patcher.AddPatch(
            code =>
            {
                Log.Trace("Changing jump condition from Beq 36 to Bge 10.", true);
                var top = code[^1];
                code.RemoveAt(code.Count - 1);
                code.RemoveAt(code.Count - 1);
                code.Add(new(OpCodes.Ldc_I4_S, (sbyte)10));
                code.Add(new(OpCodes.Bge_S, top?.operand));
            },
            new(OpCodes.Isinst, typeof(Chest)),
            new(OpCodes.Callvirt, AccessTools.Method(typeof(Chest), nameof(Chest.GetActualCapacity))),
            new(OpCodes.Ldc_I4_S, (sbyte)36),
            new(OpCodes.Beq_S));

        // Original:
        //      this.ItemsToGrabMenu = new InventoryMenu(base.xPositionOnScreen + 32, base.yPositionOnScreen, false, inventory, highlightFunction, -1, 3, 0, 0, true);
        // Patched:
        //      this.ItemsToGrabMenu = new InventoryMenu(base.xPositionOnScreen + 32, base.yPositionOnScreen, false, inventory, highlightFunction, ResizeChestMenu.GetMenuCapacity(), ResizeChestMenu.GetMenuRows(), 0, 0, true);
        //
        // This replaces the default capacity/rows of -1 and 3 with ResizeChestMenu methods to
        // allow customized capacity and rows
        patcher.AddSeek(
            new(
                OpCodes.Newobj,
                AccessTools.Constructor(
                    typeof(InventoryMenu),
                    new[]
                    {
                        typeof(int), typeof(int), typeof(bool), typeof(IList<Item>), typeof(InventoryMenu.highlightThisItem), typeof(int), typeof(int), typeof(int), typeof(int), typeof(bool),
                    })),
            new(OpCodes.Stfld, AccessTools.Field(typeof(ItemGrabMenu), nameof(ItemGrabMenu.ItemsToGrabMenu))));

        patcher.AddPatch(
            code =>
            {
                Log.Trace("Overriding default values for capacity and rows.", true);
                code.RemoveAt(code.Count - 1);
                code.RemoveAt(code.Count - 1);
                code.Add(new(OpCodes.Ldarg_0));
                code.Add(new(OpCodes.Call, AccessTools.Method(typeof(ResizeChestMenu), nameof(ResizeChestMenu.GetMenuCapacity))));
                code.Add(new(OpCodes.Ldarg_0));
                code.Add(new(OpCodes.Call, AccessTools.Method(typeof(ResizeChestMenu), nameof(ResizeChestMenu.GetMenuRows))));
            },
            new(OpCodes.Ldc_I4_M1),
            new(OpCodes.Ldc_I4_3));

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

    /// <summary>Move/resize backpack by expanded menu height.</summary>
    private static IEnumerable<CodeInstruction> ItemGrabMenu_draw_transpiler(IEnumerable<CodeInstruction> instructions)
    {
        Log.Trace($"Applying patches to {nameof(ItemGrabMenu)}.{nameof(ItemGrabMenu.draw)}");
        IPatternPatcher<CodeInstruction> patcher = new PatternPatcher<CodeInstruction>((c1, c2) => c1.opcode.Equals(c2.opcode) && (c1.operand is null || c1.OperandIs(c2.operand)));

        // ****************************************************************************************
        // Draw Backpack Patch
        // This adds ResizeChestMenu.GetMenuOffset() to the y-coordinate of the backpack sprite
        patcher.AddSeek(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ItemGrabMenu), nameof(ItemGrabMenu.showReceivingMenu))));
        patcher.AddPatch(
                   code =>
                   {
                       Log.Trace("Moving backpack icon down by expanded menu extra height.", true);
                       code.Add(new(OpCodes.Ldarg_0));
                       code.Add(new(OpCodes.Call, AccessTools.Method(typeof(ResizeChestMenu), nameof(ResizeChestMenu.GetMenuOffset))));
                       code.Add(new(OpCodes.Add));
                   },
                   new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(IClickableMenu), nameof(IClickableMenu.yPositionOnScreen))))
               .Repeat(2);

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

    /// <summary>Move/resize bottom dialogue box by search bar height.</summary>
    [SuppressMessage("ReSharper", "HeapView.BoxingAllocation", Justification = "Boxing allocation is required for Harmony.")]
    private static IEnumerable<CodeInstruction> MenuWithInventory_draw_transpiler(IEnumerable<CodeInstruction> instructions)
    {
        Log.Trace($"Applying patches to {nameof(MenuWithInventory)}.{nameof(MenuWithInventory.draw)}", true);
        IPatternPatcher<CodeInstruction> patcher = new PatternPatcher<CodeInstruction>((c1, c2) => c1.opcode.Equals(c2.opcode) && (c1.operand is null || c1.OperandIs(c2.operand)));

        // ****************************************************************************************
        // Move Dialogue Patch
        // This adds ResizeChestMenu.GetMenuOffset() to the y-coordinate of the inventory dialogue
        patcher.AddPatch(
            code =>
            {
                Log.Trace("Moving bottom dialogue box down by expanded menu height.", true);
                code.Add(new(OpCodes.Ldarg_0));
                code.Add(new(OpCodes.Call, AccessTools.Method(typeof(ResizeChestMenu), nameof(ResizeChestMenu.GetMenuOffset))));
                code.Add(new(OpCodes.Add));
            },
            new(OpCodes.Ldfld, AccessTools.Field(typeof(IClickableMenu), nameof(IClickableMenu.yPositionOnScreen))),
            new(OpCodes.Ldsfld, AccessTools.Field(typeof(IClickableMenu), nameof(IClickableMenu.borderWidth))),
            new(OpCodes.Add),
            new(OpCodes.Ldsfld, AccessTools.Field(typeof(IClickableMenu), nameof(IClickableMenu.spaceToClearTopBorder))),
            new(OpCodes.Add),
            new(OpCodes.Ldc_I4_S, (sbyte)64),
            new(OpCodes.Add));

        // ****************************************************************************************
        // Shrink Dialogue Patch
        // This subtracts ResizeChestMenu.GetMenuOffset() from the height of the inventory dialogue
        patcher.AddPatch(
            code =>
            {
                Log.Trace("Shrinking bottom dialogue box height by expanded menu height.", true);
                code.Add(new(OpCodes.Ldarg_0));
                code.Add(new(OpCodes.Call, AccessTools.Method(typeof(ResizeChestMenu), nameof(ResizeChestMenu.GetMenuOffset))));
                code.Add(new(OpCodes.Add));
            },
            new(OpCodes.Ldfld, AccessTools.Field(typeof(IClickableMenu), nameof(IClickableMenu.height))),
            new(OpCodes.Ldsfld, AccessTools.Field(typeof(IClickableMenu), nameof(IClickableMenu.borderWidth))),
            new(OpCodes.Ldsfld, AccessTools.Field(typeof(IClickableMenu), nameof(IClickableMenu.spaceToClearTopBorder))),
            new(OpCodes.Add),
            new(OpCodes.Ldc_I4, 192),
            new(OpCodes.Add));

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

    [SortedEventPriority(EventPriority.High)]
    private void OnClickableMenuChanged(object sender, IClickableMenuChangedEventArgs e)
    {
        this.Menu = e.Menu as MenuWithInventory;
        this.StorageData = e.Menu switch
        {
            ItemSelectionMenu when this.Config.DefaultChest.ResizeChestMenu == FeatureOption.Enabled => this.Config.DefaultChest,
            ItemGrabMenu when e.Context is not null && this.ManagedObjects.TryGetManagedStorage(e.Context, out var managedStorage) && managedStorage.ResizeChestMenu == FeatureOption.Enabled => managedStorage,
            _ => null,
        };

        if (e.IsNew || this.Menu is null)
        {
            this._currentOffset.Value = 0;
            this.CurrentOffset = this.MenuOffset;
        }

        if (this.StorageData is null || this.Menu is not ItemGrabMenu { ItemsToGrabMenu.inventory: { } topRow, inventory.inventory: { } bottomRow })
        {
            return;
        }

        // Set upNeighborId for first row of player inventory
        topRow = topRow.Take(12).ToList();
        bottomRow = bottomRow.TakeLast(12).ToList();
        for (var index = 0; index < 12; index++)
        {
            var topSlot = topRow.ElementAtOrDefault(index);
            var bottomSlot = bottomRow.ElementAtOrDefault(index);
            if (topSlot is not null && bottomSlot is not null)
            {
                topSlot.upNeighborID = bottomSlot.myID;
            }
        }
    }
}