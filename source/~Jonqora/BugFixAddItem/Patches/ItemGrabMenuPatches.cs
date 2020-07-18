using Harmony;
using CIL = Harmony.CodeInstruction;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using Object = StardewValley.Object;
using Netcode;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using StardewValley.Objects;
using StardewValley.Buildings;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace BugFixAddItem
{
    class ItemGrabMenuPatches
    {
        private static IModHelper Helper => ModEntry.Instance.Helper;
        private static IMonitor Monitor => ModEntry.Instance.Monitor;

        private static HarmonyInstance Harmony => ModEntry.Instance.Harmony;


        public static void Apply()
        {
            // Add an inventory.onAddItem call if Game1.player.addItemToInventoryBool();
            Harmony.Patch(
                original: AccessTools.Method(typeof(ItemGrabMenu), nameof(ItemGrabMenu.receiveLeftClick)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(ItemGrabMenuPatches), nameof(ItemGrabMenuPatches.receiveLeftClick_Prefix))),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(ItemGrabMenuPatches), nameof(ItemGrabMenuPatches.receiveLeftClick_Transpiler)))
            );
            // Add an inventory.onAddItem call if Game1.player.addItemToInventoryBool();
            Harmony.Patch(
                original: AccessTools.Method(typeof(ItemGrabMenu), nameof(ItemGrabMenu.receiveRightClick)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(ItemGrabMenuPatches), nameof(ItemGrabMenuPatches.receiveRightClick_Prefix))),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(ItemGrabMenuPatches), nameof(ItemGrabMenuPatches.receiveRightClick_Transpiler)))
            );
            // Print console output on Android to see if this method is the problem.
            Harmony.Patch(
               original: AccessTools.Method(typeof(ItemGrabMenu), nameof(ItemGrabMenu.emergencyShutDown)),
               prefix: new HarmonyMethod(AccessTools.Method(typeof(ItemGrabMenuPatches), nameof(ItemGrabMenuPatches.emergencyShutDown_Prefix)))
               //transpiler: new HarmonyMethod(AccessTools.Method(typeof(ItemGrabMenuPatches), nameof(ItemGrabMenuPatches.emergencyShutDown_Transpiler)))
           );
        }

        public static bool receiveLeftClick_Prefix(int x, int y, bool playSound, // Original arguments
            ref Item ___sourceItem, ref TemporaryAnimatedSprite ___poof,
            ItemGrabMenu.behaviorOnItemSelect ___behaviorFunction, bool ___essential, // Private fields
            ItemGrabMenu __instance) // Special
        {
            try
            {
                Monitor.Log($"Invoked ItemGrabMenu.receiveLeftClick_Prefix.", LogLevel.Debug);

                // Reimplemented game code
                /**
                //base.receiveLeftClick(x, y, !__instance.destroyItemOnClick); // Reimplemented below
                var baseMethod = typeof(MenuWithInventory).GetMethod("receiveLeftClick");
                var ftn = baseMethod.MethodHandle.GetFunctionPointer();
                var baseReceiveLeftClick = (Action<int, int, bool>)Activator.CreateInstance(
                    typeof(Action<int, int, bool>), __instance, ftn);
                baseReceiveLeftClick(x, y, !__instance.destroyItemOnClick);
                
                // Shipping bin menu
                if (__instance.shippingBin && __instance.lastShippedHolder.containsPoint(x, y))
                {
                    if (Game1.getFarm().lastItemShipped == null)
                        return false; // Don't run original code
                    Game1.getFarm().getShippingBin(Game1.player).Remove(Game1.getFarm().lastItemShipped);
                    if (Game1.player.addItemToInventoryBool(Game1.getFarm().lastItemShipped, false))
                    {
                        Game1.playSound("coin");
                        Game1.getFarm().lastItemShipped = (Item)null;
                        if (Game1.player.ActiveObject == null)
                            return false; // Don't run original code
                        Game1.player.showCarrying();
                        Game1.player.Halt();
                    }
                    else
                        Game1.getFarm().getShippingBin(Game1.player).Add(Game1.getFarm().lastItemShipped);
                }
                else
                {
                    // Select color for chests
                    if (__instance.chestColorPicker != null)
                    {
                        __instance.chestColorPicker.receiveLeftClick(x, y, true);
                        if (___sourceItem != null && ___sourceItem is Chest)
                            (___sourceItem as Chest).playerChoiceColor.Value = __instance.chestColorPicker.getColorFromSelection(__instance.chestColorPicker.colorSelection);
                    }
                    if (__instance.colorPickerToggleButton != null && __instance.colorPickerToggleButton.containsPoint(x, y))
                    {
                        Game1.player.showChestColorPicker = !Game1.player.showChestColorPicker;
                        __instance.chestColorPicker.visible = Game1.player.showChestColorPicker;
                        try
                        {
                            Game1.playSound("drumkit6");
                        }
                        catch (Exception ex)
                        {
                        }
                        __instance.SetupBorderNeighbors();
                    }
                    // Junimo harvest toggle
                    if (__instance.whichSpecialButton != -1 && __instance.specialButton != null && __instance.specialButton.containsPoint(x, y))
                    {
                        Game1.playSound("drumkit6");
                        if (__instance.whichSpecialButton == 1 && __instance.context != null && __instance.context is JunimoHut)
                        {
                            (__instance.context as JunimoHut).noHarvest.Value = !(bool)(NetFieldBase<bool, NetBool>)(__instance.context as JunimoHut).noHarvest;
                            __instance.specialButton.sourceRect.X = (bool)(NetFieldBase<bool, NetBool>)(__instance.context as JunimoHut).noHarvest ? 124 : 108;
                        }
                    }
                    // Click with empty hand
                    if (__instance.heldItem == null && __instance.showReceivingMenu)
                    {
                        __instance.heldItem = __instance.ItemsToGrabMenu.leftClick(x, y, __instance.heldItem, false);
                        // behaviorOnItemGrab Action
                        if (__instance.heldItem != null && __instance.behaviorOnItemGrab != null)
                        {
                            __instance.behaviorOnItemGrab(__instance.heldItem, Game1.player);
                            // ???
                            if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ItemGrabMenu)
                            {
                                (Game1.activeClickableMenu as ItemGrabMenu).setSourceItem(___sourceItem);
                                if (Game1.options.SnappyMenus)
                                {
                                    (Game1.activeClickableMenu as ItemGrabMenu).currentlySnappedComponent = __instance.currentlySnappedComponent;
                                    (Game1.activeClickableMenu as ItemGrabMenu).snapCursorToCurrentSnappedComponent();
                                }
                            }
                        }
                        // Dwarvish translation guide
                        if (__instance.heldItem is StardewValley.Object && (int)(NetFieldBase<int, NetInt>)(__instance.heldItem as StardewValley.Object).parentSheetIndex == 326)
                        {
                            __instance.heldItem = (Item)null;
                            Game1.player.canUnderstandDwarves = true;
                            ___poof = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2((float)(x - x % 64 + 16), (float)(y - y % 64 + 16)), false, false);
                            Game1.playSound("fireball");
                        }
                        // Lost book
                        else if (__instance.heldItem is StardewValley.Object && (int)(NetFieldBase<int, NetInt>)(__instance.heldItem as StardewValley.Object).parentSheetIndex == 102)
                        {
                            __instance.heldItem = (Item)null;
                            Game1.player.foundArtifact(102, 1);
                            ___poof = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2((float)(x - x % 64 + 16), (float)(y - y % 64 + 16)), false, false);
                            Game1.playSound("fireball");
                        }
                        // Stardrop
                        if (__instance.heldItem is StardewValley.Object && Utility.IsNormalObjectAtParentSheetIndex(__instance.heldItem, 434))
                        {
                            StardewValley.Object heldItem = __instance.heldItem as StardewValley.Object;
                            __instance.heldItem = (Item)null;
                            __instance.exitThisMenu(false);
                            Game1.player.eatObject(heldItem, true);
                        }
                        // New Recipe
                        else if (__instance.heldItem is StardewValley.Object && (bool)(NetFieldBase<bool, NetBool>)(__instance.heldItem as StardewValley.Object).isRecipe)
                        {
                            string key = __instance.heldItem.Name.Substring(0, __instance.heldItem.Name.IndexOf("Recipe") - 1);
                            try
                            {
                                if ((__instance.heldItem as StardewValley.Object).Category == -7)
                                    Game1.player.cookingRecipes.Add(key, 0);
                                else
                                    Game1.player.craftingRecipes.Add(key, 0);
                                ___poof = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2((float)(x - x % 64 + 16), (float)(y - y % 64 + 16)), false, false);
                                Game1.playSound("newRecipe");
                            }
                            catch (Exception ex)
                            {
                            }
                            __instance.heldItem = (Item)null;
                        }
                        // Try to add to inventory - this is where it's going wrong for grandpa callbacks
                        else if (Game1.player.addItemToInventoryBool(__instance.heldItem, false)) // Can I pass in a delegate? No.
                        {
                            if (__instance.inventory.onAddItem != null) // MY ADDITIONS!
                                __instance.inventory.onAddItem(__instance.heldItem, Game1.player); // MY ADDITIONS!
                            __instance.heldItem = (Item)null;
                            Game1.playSound("coin");
                        }
                    }
                    // Hands are full. Check if reverse grab is allowed
                    else if ((__instance.reverseGrab || ___behaviorFunction != null) && __instance.isWithinBounds(x, y))
                    {
                        ___behaviorFunction(__instance.heldItem, Game1.player); // Action is executed? (Works for callbacks?)
                        if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ItemGrabMenu)
                        {
                            (Game1.activeClickableMenu as ItemGrabMenu).setSourceItem(___sourceItem);
                            if (Game1.options.SnappyMenus)
                            {
                                (Game1.activeClickableMenu as ItemGrabMenu).currentlySnappedComponent = __instance.currentlySnappedComponent;
                                (Game1.activeClickableMenu as ItemGrabMenu).snapCursorToCurrentSnappedComponent();
                            }
                        }
                        if (__instance.destroyItemOnClick)
                        {
                            __instance.heldItem = (Item)null;
                            return false; // Don't run original code
                        }
                    }
                    if (__instance.organizeButton != null && __instance.organizeButton.containsPoint(x, y))
                    {
                        ClickableComponent snappedComponent = __instance.currentlySnappedComponent;
                        ItemGrabMenu.organizeItemsInList(__instance.ItemsToGrabMenu.actualInventory);
                        Game1.activeClickableMenu = (IClickableMenu)new ItemGrabMenu(__instance.ItemsToGrabMenu.actualInventory, false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), ___behaviorFunction, (string)null, __instance.behaviorOnItemGrab, false, true, true, true, true, __instance.source, ___sourceItem, __instance.whichSpecialButton, __instance.context).setEssential(___essential);
                        if (snappedComponent != null)
                        {
                            Game1.activeClickableMenu.setCurrentlySnappedComponentTo(snappedComponent.myID);
                            if (Game1.options.SnappyMenus)
                                __instance.snapCursorToCurrentSnappedComponent();
                        }
                      (Game1.activeClickableMenu as ItemGrabMenu).heldItem = __instance.heldItem;
                        Game1.playSound("Ship");
                    }
                    if (__instance.fillStacksButton != null && __instance.fillStacksButton.containsPoint(x, y))
                    {
                        __instance.FillOutStacks();
                        Game1.playSound("Ship");
                    }
                    // This one seems to trigger statue callback behaviour. It calls inventory.onAddItem. Try with fishing artifact?
                    // I think the difference is for delegate onAddItem functions, versus in-built "new Artifact?" checking functions.
                    else
                    {
                        if (__instance.heldItem == null || __instance.isWithinBounds(x, y) || !__instance.heldItem.canBeTrashed())
                            return false; // Don't run original code
                        __instance.DropHeldItem();
                    }
                }
                return false;**/

                return true;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(receiveLeftClick_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // Run original code
            }
        }

        // Insertable check for OnAddItem behaviour where missing in game code.
        public static void OnAddItemCheck_Hook(ItemGrabMenu grabMenu, Farmer who)
        {
            try
            {
                if (grabMenu.inventory.onAddItem != null)
                {
                    grabMenu.inventory.onAddItem(grabMenu.heldItem, who);
                    Monitor.Log($"Ran patch for bug in game code: {nameof(OnAddItemCheck_Hook)}", LogLevel.Debug);
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(OnAddItemCheck_Hook)}:\n{ex}", LogLevel.Error);
            }
        }

        public static IEnumerable<CIL> receiveLeftClick_Transpiler(IEnumerable<CIL> instructions, ILGenerator gen)
        {
            try
            {
                var codes = new List<CodeInstruction>(instructions);

                for (int i = 0; i < codes.Count - 8; i++)
                {
                    // Find any sequence matching if(Game1.player.addItemToInventoryBool(this.heldItem, false)
                    //     which is followed by this.heldItem = null assignment if true.
                    
                    if (//call class StardewValley.Farmer StardewValley.Game1::get_player()
                        codes[i].opcode == OpCodes.Call &&
                        (MethodInfo)codes[i].operand == typeof(Game1).GetProperty("player").GetGetMethod() &&
                        //ldarg.0
                        codes[i + 1].opcode == OpCodes.Ldarg_0 &&
                        //ldfld class StardewValley.Item StardewValley.Menus.MenuWithInventory::heldItem
                        codes[i + 2].opcode == OpCodes.Ldfld &&
                        (FieldInfo)codes[i + 2].operand == typeof(MenuWithInventory).GetField("heldItem") &&
                        //ldc.i4.0
                        codes[i + 3].opcode == OpCodes.Ldc_I4_0 &&
                        //callvirt instance bool StardewValley.Farmer::addItemToInventoryBool(class StardewValley.Item, bool)
                        codes[i + 4].opcode == OpCodes.Callvirt &&
                        (MethodInfo)codes[i + 4].operand == typeof(Farmer).GetMethod("addItemToInventoryBool") &&
                        //brfalse IL_05dc || brfalse.s IL_02f0 (return)
                        (codes[i + 5].opcode == OpCodes.Brfalse || codes[i + 5].opcode == OpCodes.Brfalse_S) &&
                        //ldarg.0
                        codes[i + 6].opcode == OpCodes.Ldarg_0 && // INSERT NEW CODES AT THIS INDEX
                        //ldnull
                        codes[i + 7].opcode == OpCodes.Ldnull &&
                        //stfld class StardewValley.Item StardewValley.Menus.MenuWithInventory::heldItem
                        codes[i + 8].opcode == OpCodes.Stfld &&
                        (FieldInfo)codes[i + 8].operand == typeof(MenuWithInventory).GetField("heldItem"))
                    {
                        Monitor.Log($"Found a location to insert codes: {codes[i + 6]}", LogLevel.Debug);

                        // Compose the new instructions to inject
                        var codesToInsert = new List<CodeInstruction>
                        {
                            new CIL(OpCodes.Ldarg_0),
                            new CIL(OpCodes.Call, typeof(Game1).GetProperty("player").GetGetMethod()),
                            new CIL(OpCodes.Call, Helper.Reflection.GetMethod(
                                typeof(ItemGrabMenuPatches),nameof(OnAddItemCheck_Hook)).MethodInfo)
                        };

                        // Inject the instructions
                        codes.InsertRange(i + 6, codesToInsert);
                        foreach (CIL cil in codesToInsert)
                        {
                            Monitor.Log($"Inserted new OpCode: {cil}", LogLevel.Debug);
                        }
                    }
                }
                return codes.AsEnumerable();
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(receiveLeftClick_Transpiler)}:\n{ex}", LogLevel.Error);
                return instructions; // use original code
            }
        }

        public static bool receiveRightClick_Prefix(int x, int y, bool playSound, // Original arguments
            ref Item ___sourceItem, ref TemporaryAnimatedSprite ___poof,
            ItemGrabMenu.behaviorOnItemSelect ___behaviorFunction, bool ___essential, // Private fields
            ItemGrabMenu __instance) // Special
        {
            try
            {
                Monitor.Log($"Invoked ItemGrabMenu.receiveRightClick_Prefix", LogLevel.Debug);

                // Reimplemented game code
                return true;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(receiveRightClick_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // Run original code
            }
        }

        public static IEnumerable<CodeInstruction> receiveRightClick_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen)
        {
            try
            {
                var codes = new List<CodeInstruction>(instructions);

                for (int i = 0; i < codes.Count - 8; i++)
                {
                    // Find any sequence matching if(Game1.player.addItemToInventoryBool(this.heldItem, false)
                    //     which is followed by this.heldItem = null assignment if true.

                    if (//call class StardewValley.Farmer StardewValley.Game1::get_player()
                        codes[i].opcode == OpCodes.Call &&
                        (MethodInfo)codes[i].operand == typeof(Game1).GetProperty("player").GetGetMethod() &&
                        //ldarg.0
                        codes[i + 1].opcode == OpCodes.Ldarg_0 &&
                        //ldfld class StardewValley.Item StardewValley.Menus.MenuWithInventory::heldItem
                        codes[i + 2].opcode == OpCodes.Ldfld &&
                        (FieldInfo)codes[i + 2].operand == typeof(MenuWithInventory).GetField("heldItem") &&
                        //ldc.i4.0
                        codes[i + 3].opcode == OpCodes.Ldc_I4_0 &&
                        //callvirt instance bool StardewValley.Farmer::addItemToInventoryBool(class StardewValley.Item, bool)
                        codes[i + 4].opcode == OpCodes.Callvirt &&
                        (MethodInfo)codes[i + 4].operand == typeof(Farmer).GetMethod("addItemToInventoryBool") &&
                        //befalse IL_05dc || brfalse.s IL_02f0 (return)
                        (codes[i + 5].opcode == OpCodes.Brfalse || codes[i + 5].opcode == OpCodes.Brfalse_S) &&
                        //ldarg.0
                        codes[i + 6].opcode == OpCodes.Ldarg_0 && // INSERT NEW CODES AT THIS INDEX
                        //ldnull
                        codes[i + 7].opcode == OpCodes.Ldnull &&
                        //stfld class StardewValley.Item StardewValley.Menus.MenuWithInventory::heldItem
                        codes[i + 8].opcode == OpCodes.Stfld &&
                        (FieldInfo)codes[i + 8].operand == typeof(MenuWithInventory).GetField("heldItem"))
                    {
                        Monitor.Log($"Found a location to insert codes: {codes[i + 6]}", LogLevel.Debug);

                        // Compose the new instructions to inject
                        var codesToInsert = new List<CodeInstruction>
                        {
                            new CIL(OpCodes.Ldarg_0),
                            new CIL(OpCodes.Call, typeof(Game1).GetProperty("player").GetGetMethod()),
                            new CIL(OpCodes.Call, Helper.Reflection.GetMethod(
                                typeof(ItemGrabMenuPatches),nameof(OnAddItemCheck_Hook)).MethodInfo)
                        };

                        // Inject the instructions
                        codes.InsertRange(i + 6, codesToInsert);
                        foreach (CIL cil in codesToInsert)
                        {
                            Monitor.Log($"Inserted new OpCode: {cil}", LogLevel.Debug);
                        }
                    }
                }
                return codes.AsEnumerable();
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(receiveRightClick_Transpiler)}:\n{ex}", LogLevel.Error);
                return instructions; // use original code
            }
        }

        public static void emergencyShutDown_Prefix()
        {
            try
            {
                Monitor.Log($"Invoked ItemGrabMenu.emergencyShutDown_Prefix.", LogLevel.Debug);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(emergencyShutDown_Prefix)}:\n{ex}", LogLevel.Error);
            }
        }

    }
}
