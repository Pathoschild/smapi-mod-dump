/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using Harmony;
using ImJustMatt.Common.Patches;
using ImJustMatt.ExpandedStorage.Framework.Controllers;
using ImJustMatt.ExpandedStorage.Framework.Extensions;
using ImJustMatt.ExpandedStorage.Framework.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Objects;

namespace ImJustMatt.ExpandedStorage.Framework.Patches
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class ItemGrabMenuPatches : MenuPatches
    {
        public ItemGrabMenuPatches(IMod mod, HarmonyInstance harmony) : base(mod, harmony)
        {
            var constructor = AccessTools.Constructor(typeof(ItemGrabMenu),
                new[]
                {
                    typeof(IList<Item>),
                    typeof(bool),
                    typeof(bool),
                    typeof(InventoryMenu.highlightThisItem),
                    typeof(ItemGrabMenu.behaviorOnItemSelect),
                    typeof(string),
                    typeof(ItemGrabMenu.behaviorOnItemSelect),
                    typeof(bool),
                    typeof(bool),
                    typeof(bool),
                    typeof(bool),
                    typeof(bool),
                    typeof(int),
                    typeof(Item),
                    typeof(int),
                    typeof(object)
                });

            harmony.Patch(
                constructor,
                transpiler: new HarmonyMethod(GetType(), nameof(ConstructorTranspiler))
            );

            harmony.Patch(
                constructor,
                postfix: new HarmonyMethod(GetType(), nameof(ConstructorPostfix))
            );

            harmony.Patch(
                AccessTools.Method(typeof(ItemGrabMenu), nameof(ItemGrabMenu.draw), new[] {typeof(SpriteBatch)}),
                transpiler: new HarmonyMethod(GetType(), nameof(DrawTranspiler))
            );

            harmony.Patch(
                AccessTools.Method(typeof(ItemGrabMenu), nameof(ItemGrabMenu.FillOutStacks)),
                postfix: new HarmonyMethod(GetType(), nameof(FillOutStacksPostfix))
            );

            harmony.Patch(
                AccessTools.Method(typeof(ItemGrabMenu), nameof(ItemGrabMenu.gameWindowSizeChanged)),
                postfix: new HarmonyMethod(GetType(), nameof(GameWindowSizeChangedPostfix))
            );

            harmony.Patch(
                AccessTools.Method(typeof(ItemGrabMenu), nameof(ItemGrabMenu.setSourceItem)),
                postfix: new HarmonyMethod(GetType(), nameof(SetSourceItemPostfix))
            );

            harmony.Patch(
                AccessTools.Method(typeof(ItemGrabMenu), nameof(ItemGrabMenu.RepositionSideButtons)),
                postfix: new HarmonyMethod(GetType(), nameof(RepositionSideButtonsPostfix))
            );
        }

        /// <summary>Loads default chest InventoryMenu when storage has modded capacity.</summary>
        private static IEnumerable<CodeInstruction> ConstructorTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var patternPatches = new PatternPatches(instructions, Monitor);

            patternPatches
                .Find(
                    new CodeInstruction(OpCodes.Isinst, typeof(Chest)),
                    new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(Chest), nameof(Chest.GetActualCapacity))),
                    new CodeInstruction(OpCodes.Ldc_I4_S),
                    new CodeInstruction(OpCodes.Beq)
                )
                .Log("Changing jump condition from Beq 36 to Bge 10.")
                .Patch(delegate(LinkedList<CodeInstruction> list)
                {
                    var jumpCode = list.Last.Value;
                    list.RemoveLast();
                    list.RemoveLast();
                    list.AddLast(new CodeInstruction(OpCodes.Ldc_I4_S, (byte) 10));
                    list.AddLast(new CodeInstruction(OpCodes.Bge, jumpCode.operand));
                });

            var inventoryMenuConstructor = AccessTools.Constructor(typeof(InventoryMenu), new[]
            {
                typeof(int),
                typeof(int),
                typeof(bool),
                typeof(IList<Item>),
                typeof(InventoryMenu.highlightThisItem),
                typeof(int),
                typeof(int),
                typeof(int),
                typeof(int),
                typeof(bool)
            });

            patternPatches
                .Find(
                    new CodeInstruction(OpCodes.Newobj, inventoryMenuConstructor),
                    new CodeInstruction(OpCodes.Stfld, AccessTools.Field(typeof(ItemGrabMenu), nameof(ItemGrabMenu.ItemsToGrabMenu)))
                )
                .Find(
                    new CodeInstruction(OpCodes.Ldc_I4_M1),
                    new CodeInstruction(OpCodes.Ldc_I4_3)
                )
                .Log("Overriding default values for capacity and rows.")
                .Patch(delegate(LinkedList<CodeInstruction> list)
                {
                    list.RemoveLast();
                    list.RemoveLast();
                    list.AddLast(new CodeInstruction(OpCodes.Ldarg_S, (byte) 16));
                    list.AddLast(new CodeInstruction(OpCodes.Call, MenuCapacity));
                    list.AddLast(new CodeInstruction(OpCodes.Ldarg_S, (byte) 16));
                    list.AddLast(new CodeInstruction(OpCodes.Call, MenuRows));
                });

            foreach (var patternPatch in patternPatches)
                yield return patternPatch;

            if (!patternPatches.Done)
                Monitor.Log($"Failed to apply all patches in {nameof(ConstructorTranspiler)}", LogLevel.Warn);
        }

        private static void ConstructorPostfix(ItemGrabMenu __instance, ref ItemGrabMenu.behaviorOnItemSelect ___behaviorFunction)
        {
            if (!Mod.AssetController.TryGetStorage(__instance.context, out var storage) || __instance.context is ShippingBin)
                return;

            var menuConfig = storage.Config.Menu;

            __instance.setBackgroundTransparency(false);
            __instance.ItemsToGrabMenu.rows = Config.ExpandInventoryMenu ? menuConfig.Rows : 3;
            if (Config.ExpandInventoryMenu && menuConfig.Capacity > 0)
                __instance.ItemsToGrabMenu.capacity = menuConfig.Capacity;

            if (__instance.context is not Chest chest)
                chest = null;

            if (Mod.ChestController.HeldChest != null
                && chest != null
                && !ReferenceEquals(Mod.ChestController.HeldChest, chest))
            {
                ___behaviorFunction = delegate(Item item, Farmer who)
                {
                    var tmp = chest.addItem(item);
                    if (tmp == null)
                    {
                        Mod.ChestController.HeldChest?.GetItemsForPlayer(who.UniqueMultiplayerID).Remove(item);
                        Mod.ChestController.HeldChest?.clearNulls();
                        Mod.ActiveMenu.Value.RefreshItems();
                    }

                    chest.ShowMenu();
                    if (Game1.activeClickableMenu is ItemGrabMenu menu)
                        menu.heldItem = tmp;
                };

                __instance.behaviorOnItemGrab = delegate(Item item, Farmer who)
                {
                    __instance.heldItem = Mod.ChestController?.HeldChest.addItem(item);
                    if (__instance.heldItem == null)
                    {
                        chest.GetItemsForPlayer(who.UniqueMultiplayerID).Remove(item);
                        chest.clearNulls();
                        Mod.ActiveMenu.Value.RefreshItems();
                    }
                };

                __instance.inventory = new InventoryMenu(
                    __instance.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2,
                    __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + 192 - 16,
                    false,
                    Mod.ChestController.HeldChest.GetItemsForPlayer(Game1.player.UniqueMultiplayerID),
                    chest.HighlightMethod(storage));
            }
            else
            {
                __instance.inventory.highlightMethod = chest.HighlightMethod(storage);
            }

            if (__instance.chestColorPicker != null)
            {
                if (!storage.PlayerColor || storage.Config.Option("ShowColorPicker", true) != StorageConfigController.Choice.Enable)
                    __instance.colorPickerToggleButton = null;
                if (Config.ColorPicker)
                {
                    __instance.chestColorPicker = null;
                    __instance.discreteColorPickerCC = null;
                }

                __instance.populateClickableComponentList();
                __instance.SetupBorderNeighbors();
            }
            else if (storage.PlayerColor && storage.Config.Option("ShowColorPicker", true) == StorageConfigController.Choice.Enable)
            {
                __instance.colorPickerToggleButton = new ClickableTextureComponent(
                    new Rectangle(__instance.xPositionOnScreen + __instance.width,
                        __instance.yPositionOnScreen + __instance.height / 3 - 64 + -160, 64, 64),
                    Game1.mouseCursors,
                    new Rectangle(119, 469, 16, 16),
                    4f)
                {
                    hoverText = Game1.content.LoadString("Strings\\UI:Toggle_ColorPicker"),
                    myID = 27346,
                    downNeighborID = -99998,
                    leftNeighborID = 53921,
                    region = 15923
                };
                __instance.populateClickableComponentList();
                __instance.SetupBorderNeighbors();
            }

            if (storage.Config.Option("ShowSearchBar", true) == StorageConfigController.Choice.Enable)
            {
                __instance.yPositionOnScreen -= menuConfig.Padding;
                __instance.height += menuConfig.Padding;
                if (__instance.chestColorPicker != null)
                    __instance.chestColorPicker.yPositionOnScreen -= menuConfig.Padding;
            }

            if (Config.ExpandInventoryMenu)
            {
                __instance.height += menuConfig.Offset;
                __instance.inventory.movePosition(0, menuConfig.Offset);
                if (__instance.okButton != null)
                    __instance.okButton.bounds.Y += menuConfig.Offset;
                if (__instance.trashCan != null)
                    __instance.trashCan.bounds.Y += menuConfig.Offset;
                if (__instance.dropItemInvisibleButton != null)
                    __instance.dropItemInvisibleButton.bounds.Y += menuConfig.Offset;
                __instance.RepositionSideButtons();
            }
        }

        /// <summary>Refresh Items when Stacks are filled</summary>
        private static void FillOutStacksPostfix()
        {
            Mod.ChestController.HeldChest?.clearNulls(); 
        }

        /// <summary>Set color picker to HSL Color Picker.</summary>
        private static void GameWindowSizeChangedPostfix(ItemGrabMenu __instance)
        {
            if (Mod.AssetController.TryGetStorage(__instance.context, out var storage) && __instance.context is not ShippingBin && Config.ColorPicker)
            {
                __instance.chestColorPicker = storage.PlayerColor && storage.Config.Option("ShowColorPicker", true) == StorageConfigController.Choice.Enable ? HSLColorPicker.Instance.Value : null;
            }
        }

        /// <summary>Set color picker to HSL Color Picker.</summary>
        private static void SetSourceItemPostfix(ItemGrabMenu __instance)
        {
            if (Mod.AssetController.TryGetStorage(__instance.context, out var storage) && __instance.context is not ShippingBin && Config.ColorPicker)
            {
                __instance.chestColorPicker = storage.PlayerColor && storage.Config.Option("ShowColorPicker", true) == StorageConfigController.Choice.Enable ? HSLColorPicker.Instance.Value : null;
            }
        }

        /// <summary>Reposition side buttons with offset.</summary>
        private static void RepositionSideButtonsPostfix(ItemGrabMenu __instance)
        {
            if (!Mod.AssetController.TryGetStorage(__instance.context, out var storage) || __instance.context is ShippingBin)
                return;

            var menuConfig = storage.Config.Menu;

            if (!Config.ExpandInventoryMenu || menuConfig.Offset >= 0) return;
            if (__instance.colorPickerToggleButton != null)
                __instance.colorPickerToggleButton.bounds.Y += menuConfig.Offset / 2;
            if (__instance.fillStacksButton != null)
                __instance.fillStacksButton.bounds.Y += menuConfig.Offset / 2;
            if (__instance.organizeButton != null)
                __instance.organizeButton.bounds.Y += menuConfig.Offset / 2;
        }

        /// <summary>Patch UI elements for ItemGrabMenu.</summary>
        private static IEnumerable<CodeInstruction> DrawTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var patternPatches = new PatternPatches(instructions, Monitor);

            // Offset backpack icon
            patternPatches
                .Find(
                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ItemGrabMenu), nameof(ItemGrabMenu.showReceivingMenu)))
                )
                .Find(
                    new CodeInstruction(OpCodes.Ldfld, IClickableMenuYPositionOnScreen)
                )
                .Log("Adding Offset to yPositionOnScreen for Backpack sprite.")
                .Patch(OffsetPatch(MenuOffset, OpCodes.Add))
                .Repeat(3);

            // Add top padding
            patternPatches
                .Find(
                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ItemGrabMenu), nameof(ItemGrabMenu.ItemsToGrabMenu))),
                    new CodeInstruction(OpCodes.Ldfld, IClickableMenuYPositionOnScreen),
                    new CodeInstruction(OpCodes.Ldsfld, IClickableMenuBorderWidth),
                    new CodeInstruction(OpCodes.Sub),
                    new CodeInstruction(OpCodes.Ldsfld, IClickableMenuSpaceToClearTopBorder),
                    new CodeInstruction(OpCodes.Sub)
                )
                .Log("Adding top padding offset to drawDialogueBox.y.")
                .Patch(OffsetPatch(MenuPadding, OpCodes.Sub));

            patternPatches
                .Find(
                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ItemGrabMenu), nameof(ItemGrabMenu.ItemsToGrabMenu))),
                    new CodeInstruction(OpCodes.Ldfld, IClickableMenuHeight),
                    new CodeInstruction(OpCodes.Ldsfld, IClickableMenuSpaceToClearTopBorder),
                    new CodeInstruction(OpCodes.Add),
                    new CodeInstruction(OpCodes.Ldsfld, IClickableMenuBorderWidth),
                    new CodeInstruction(OpCodes.Ldc_I4_2),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Add)
                )
                .Log("Adding top padding offset to drawDialogueBox.height.")
                .Patch(OffsetPatch(MenuPadding, OpCodes.Add));

            foreach (var patternPatch in patternPatches)
                yield return patternPatch;

            if (!patternPatches.Done)
                Monitor.Log($"Failed to apply all patches in {nameof(DrawTranspiler)}", LogLevel.Warn);
        }
    }
}