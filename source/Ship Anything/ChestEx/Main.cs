/*
   MIT License

   Copyright (c) 2019 Berkay Yigit <berkay2578@gmail.com>
       Copyright holder detail: Nickname(s) used by the copyright holder: 'berkay2578', 'berkayylmao'.

   Permission is hereby granted, free of charge, to any person obtaining a copy
   of this software and associated documentation files (the "Software"), to deal
   in the Software without restriction, including without limitation the rights
   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
   copies of the Software, and to permit persons to whom the Software is
   furnished to do so, subject to the following conditions:

   The above copyright notice and this permission notice shall be included in all
   copies or substantial portions of the Software.

   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
   SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using Harmony;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace ChestEx {
   public class Main : Mod {
      public const String strChestsAnywhereUID = "Pathoschild.ChestsAnywhere";

      public static class ChestExtensions {
         public static class ItemGrabMenuArbitration {
            public static class CtorPatches {
               [HarmonyTranspiler]
               public static IEnumerable<CodeInstruction> TranspilerPatches(IEnumerable<CodeInstruction> instructions) {
                  MethodBase baseCtorCall = typeof(MenuWithInventory).GetConstructor(AccessTools.all, null,
                        new Type[] { typeof(InventoryMenu.highlightThisItem), typeof(Boolean), typeof(Boolean), typeof(Int32), typeof(Int32) }, null);
                  var lblSkip = new Label();

                  foreach (var instruction in instructions) {
                     if (instruction.opcode == OpCodes.Call
                        && (MethodBase)instruction.operand == baseCtorCall) {
                        yield return new CodeInstruction(OpCodes.Ldarg_2);
                        yield return new CodeInstruction(OpCodes.Isinst, typeof(ChestExMenu.ArbitrationObjects));
                        yield return new CodeInstruction(OpCodes.Brfalse, lblSkip);
                        // is ArbitrationObjects
                        {
                           yield return new CodeInstruction(OpCodes.Pop);
                           yield return new CodeInstruction(OpCodes.Pop);
                           yield return new CodeInstruction(OpCodes.Pop);
                           yield return new CodeInstruction(OpCodes.Pop);
                           yield return new CodeInstruction(OpCodes.Pop);
                           yield return new CodeInstruction(OpCodes.Pop);
                           yield return new CodeInstruction(OpCodes.Ldarg_0);
                           yield return new CodeInstruction(OpCodes.Ldarg_2);
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ChestExMenu.ArbitrationObjects), "highlightFunction"));
                           yield return new CodeInstruction(OpCodes.Ldarg_2);
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ChestExMenu.ArbitrationObjects), "okButton"));
                           yield return new CodeInstruction(OpCodes.Ldarg_2);
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ChestExMenu.ArbitrationObjects), "trashCan"));
                           yield return new CodeInstruction(OpCodes.Ldarg_2);
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ChestExMenu.ArbitrationObjects), "inventoryXOffset"));
                           yield return new CodeInstruction(OpCodes.Ldarg_2);
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ChestExMenu.ArbitrationObjects), "inventoryYOffset"));
                           yield return instruction;
                           yield return new CodeInstruction(OpCodes.Ret);
                        }
                        var _ = new CodeInstruction(instruction);
                        _.labels.Add(lblSkip);
                        yield return _;
                     } else {
                        yield return instruction;
                     }
                  }
               }
            }

            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> TranspilerPatches(IEnumerable<CodeInstruction> instructions) {
               MethodBase[] _orgCalls = {
                        typeof(ItemGrabMenu).GetConstructor(AccessTools.all, null,
                           new Type[] { typeof(IList<Item>), typeof(bool), typeof(bool), typeof(InventoryMenu.highlightThisItem), typeof(ItemGrabMenu.behaviorOnItemSelect), typeof(string), typeof(ItemGrabMenu.behaviorOnItemSelect), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(int), typeof(Item), typeof(int), typeof(object) }, null),
                        typeof(ItemGrabMenu.behaviorOnItemSelect).GetConstructor(AccessTools.all, null,
                           new Type[] { typeof(Item), typeof(Farmer) }, null)
               };
               MethodBase[] _newCalls = {
                        typeof(ChestExMenu).GetConstructor(AccessTools.all, null,
                           new Type[] { typeof(IList<Item>), typeof(bool), typeof(bool), typeof(InventoryMenu.highlightThisItem), typeof(ChestExMenu.behaviorOnItemSelect), typeof(string), typeof(ChestExMenu.behaviorOnItemSelect), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(int), typeof(Item), typeof(int), typeof(object) }, null),
                        typeof(ChestExMenu.behaviorOnItemSelect).GetConstructor(AccessTools.all, null,
                           new Type[] { typeof(Item), typeof(Farmer) }, null)
               };

               foreach (var instruction in instructions) {
                  if (instruction.opcode == OpCodes.Newobj) {
                     // ItemGrabMenu.ctor[2]
                     if ((ConstructorInfo)instruction.operand == _orgCalls[0]) {
                        yield return new CodeInstruction(OpCodes.Newobj, _newCalls[0]);
                        continue;
                     }
                     // ItemGrabMenu.behaviorOnItemSelect, just in case for later
                     else if ((ConstructorInfo)instruction.operand == _orgCalls[1]) {
                        yield return new CodeInstruction(OpCodes.Newobj, _newCalls[1]);
                        continue;
                     }
                  }
                  // IsInst ItemGrabMenu
                  else if (instruction.opcode == OpCodes.Isinst
                      && (Type)instruction.operand == typeof(ItemGrabMenu)) {
                     yield return new CodeInstruction(OpCodes.Isinst, typeof(ChestExMenu));
                     continue;
                  }
                  yield return instruction;
               }
            }
         }
         public static class CapacityRaiser {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> TranspilerPatches(IEnumerable<CodeInstruction> instructions) {
               foreach (var instruction in instructions) {
                  if (instruction.opcode == OpCodes.Ldc_I4_S
                      && (SByte)instruction.operand == 36) {
                     yield return new CodeInstruction(OpCodes.Ldc_I4_S, (SByte)Math.Min(SByte.MaxValue, Config.instance.getCapacity()));
                     continue;
                  }
                  yield return instruction;
               }
            }
         }
      }

      public static class ChestsAnywhereCompatibility {
         public static class ChestContainer {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> OpenMenu_TranspilerPatches(IEnumerable<CodeInstruction> instructions) {
               MethodBase _orgCall =
                        typeof(ItemGrabMenu).GetConstructor(AccessTools.all, null,
                           new Type[] { typeof(IList<Item>), typeof(bool), typeof(bool), typeof(InventoryMenu.highlightThisItem), typeof(ItemGrabMenu.behaviorOnItemSelect), typeof(string), typeof(ItemGrabMenu.behaviorOnItemSelect), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(int), typeof(Item), typeof(int), typeof(object) }, null);
               MethodBase _newCall =
                        typeof(ChestExMenu).GetConstructor(AccessTools.all, null,
                           new Type[] { typeof(IList<Item>), typeof(bool), typeof(bool), typeof(InventoryMenu.highlightThisItem), typeof(ChestExMenu.behaviorOnItemSelect), typeof(string), typeof(ChestExMenu.behaviorOnItemSelect), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(int), typeof(Item), typeof(int), typeof(object) }, null);

               foreach (var instruction in instructions) {
                  if (instruction.opcode == OpCodes.Newobj) {
                     // ItemGrabMenu.ctor[2]
                     if ((ConstructorInfo)instruction.operand == _orgCall) {
                        yield return new CodeInstruction(OpCodes.Newobj, _newCall);
                        continue;
                     }
                  }
                  yield return instruction;
               }
            }
         }
         public static class ManageChestOverlay {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> ReinitialiseComponents_TranspilerPatches(IEnumerable<CodeInstruction> instructions, ILGenerator ilg) {
               var typeChestsAnywhere_Tab = Type.GetType("Pathoschild.Stardew.ChestsAnywhere.Menus.Components.Tab, ChestsAnywhere");
               var typeChestsAnywhere_ManagedChest = Type.GetType("Pathoschild.Stardew.ChestsAnywhere.Framework.ManagedChest, ChestsAnywhere");
               var typeChestsAnywhere_ManageChestOverlay = Type.GetType("Pathoschild.Stardew.ChestsAnywhere.Menus.Overlays.ManageChestOverlay, ChestsAnywhere");
               var typeChestsAnywhere_ChestContainer = Type.GetType("Pathoschild.Stardew.ChestsAnywhere.Framework.Containers.ChestContainer, ChestsAnywhere");

               var getTabSizeMethodInfo = Harmony.AccessTools.Method(typeChestsAnywhere_Tab, "GetTabSize");
               var getContainerMethodInfo = Harmony.AccessTools.Method(typeChestsAnywhere_ManagedChest, "get_Container");
               var getDisplayNameMethodInfo = Harmony.AccessTools.Method(typeChestsAnywhere_ManagedChest, "get_DisplayName");
               var getSelectedCategoryMethodInfo = Harmony.AccessTools.Method(typeChestsAnywhere_ManageChestOverlay, "get_SelectedCategory");
               var getShowCategoryDropdownMethodInfo = Harmony.AccessTools.Method(typeChestsAnywhere_ManageChestOverlay, "get_ShowCategoryDropdown");

               Boolean skipOriginalPositionCode = false, skipToPatchAddress = false, doPatching = false, patchingCategoryTab = true;
               Boolean markPostPatchCode = false, didAllPatches = false;
               var lblEditSuccessful = ilg.DefineLabel();
               var lblEditUserSelectedNoCategoryDropDown = ilg.DefineLabel();

               foreach (var instruction in instructions) {
                  if (!didAllPatches) {
                     if (patchingCategoryTab) {
                        if (instruction.opcode == OpCodes.Call &&
                              (MethodBase)instruction.operand == getTabSizeMethodInfo) {
                           skipToPatchAddress = true;
                        }
                        if (doPatching) {
                           yield return new CodeInstruction(OpCodes.Ldarg_0);
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeChestsAnywhere_ManageChestOverlay, "Menu"));
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StardewValley.Menus.MenuWithInventory), "inventory"));
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StardewValley.Menus.IClickableMenu), "xPositionOnScreen"));
                           yield return new CodeInstruction(OpCodes.Ldarg_0);
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeChestsAnywhere_ManageChestOverlay, "Menu"));
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StardewValley.Menus.MenuWithInventory), "inventory"));
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StardewValley.Menus.IClickableMenu), "yPositionOnScreen"));
                           yield return new CodeInstruction(OpCodes.Ldc_I4, Game1.tileSize);
                           yield return new CodeInstruction(OpCodes.Sub);
                           skipOriginalPositionCode = true;
                           doPatching = false;
                        }
                        if (skipToPatchAddress) {
                           yield return instruction;
                           if (instruction.opcode != OpCodes.Call ||
                                 (MethodBase)instruction.operand != getSelectedCategoryMethodInfo) {
                              continue;
                           } else if (instruction.opcode == OpCodes.Call &&
                                 (MethodBase)instruction.operand == getSelectedCategoryMethodInfo) {
                              skipToPatchAddress = false;
                              doPatching = true;
                              continue;
                           }
                        }
                        if (skipOriginalPositionCode) {
                           if (instruction.opcode == OpCodes.Ldfld
                                 && (FieldInfo)instruction.operand == AccessTools.Field(typeChestsAnywhere_ManageChestOverlay, "Font")) {
                              yield return new CodeInstruction(OpCodes.Ldc_I4_1);
                              yield return new CodeInstruction(OpCodes.Ldarg_0);
                              yield return instruction;
                              patchingCategoryTab = skipToPatchAddress = skipOriginalPositionCode = false;
                           }
                           continue;
                        }
                     } else {
                        if (instruction.opcode == OpCodes.Callvirt &&
                              (MethodBase)instruction.operand == getDisplayNameMethodInfo) {
                           yield return new CodeInstruction(OpCodes.Pop);
                           yield return new CodeInstruction(OpCodes.Pop);
                           yield return new CodeInstruction(OpCodes.Ldarg_0);
                           yield return new CodeInstruction(OpCodes.Call, getShowCategoryDropdownMethodInfo);
                           yield return new CodeInstruction(OpCodes.Brfalse, lblEditUserSelectedNoCategoryDropDown);

                           yield return new CodeInstruction(OpCodes.Ldarg_0);
                           yield return new CodeInstruction(OpCodes.Ldarg_0);
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeChestsAnywhere_ManageChestOverlay, "Chest"));
                           yield return instruction;
                           yield return new CodeInstruction(OpCodes.Ldarg_0);
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeChestsAnywhere_ManageChestOverlay, "CategoryTab"));
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StardewValley.Menus.ClickableComponent), "bounds"));
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Microsoft.Xna.Framework.Rectangle), "X"));
                           yield return new CodeInstruction(OpCodes.Ldarg_0);
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeChestsAnywhere_ManageChestOverlay, "CategoryTab"));
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StardewValley.Menus.ClickableComponent), "bounds"));
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Microsoft.Xna.Framework.Rectangle), "Width"));
                           yield return new CodeInstruction(OpCodes.Add);
                           yield return new CodeInstruction(OpCodes.Ldc_I4, Game1.smallestTileSize);
                           yield return new CodeInstruction(OpCodes.Add);
                           yield return new CodeInstruction(OpCodes.Ldarg_0);
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeChestsAnywhere_ManageChestOverlay, "CategoryTab"));
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StardewValley.Menus.ClickableComponent), "bounds"));
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Microsoft.Xna.Framework.Rectangle), "Y"));
                           yield return new CodeInstruction(OpCodes.Br, lblEditSuccessful);

                           var skip_UserSelectedNoCategoryDropDown = new CodeInstruction(OpCodes.Nop);
                           skip_UserSelectedNoCategoryDropDown.labels.Add(lblEditUserSelectedNoCategoryDropDown);
                           yield return skip_UserSelectedNoCategoryDropDown;
                           yield return new CodeInstruction(OpCodes.Ldarg_0);
                           yield return new CodeInstruction(OpCodes.Ldarg_0);
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeChestsAnywhere_ManageChestOverlay, "Chest"));
                           yield return instruction;
                           yield return new CodeInstruction(OpCodes.Ldarg_0);
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeChestsAnywhere_ManageChestOverlay, "Menu"));
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StardewValley.Menus.MenuWithInventory), "inventory"));
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StardewValley.Menus.IClickableMenu), "xPositionOnScreen"));
                           yield return new CodeInstruction(OpCodes.Ldarg_0);
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeChestsAnywhere_ManageChestOverlay, "Menu"));
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StardewValley.Menus.MenuWithInventory), "inventory"));
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StardewValley.Menus.IClickableMenu), "yPositionOnScreen"));
                           yield return new CodeInstruction(OpCodes.Ldc_I4, Game1.tileSize);
                           yield return new CodeInstruction(OpCodes.Sub);
                           yield return new CodeInstruction(OpCodes.Br, lblEditSuccessful);

                           markPostPatchCode = true;
                        }
                        if (markPostPatchCode) {
                           if (instruction.opcode == OpCodes.Ldfld
                                 && (FieldInfo)instruction.operand == AccessTools.Field(typeChestsAnywhere_ManageChestOverlay, "Font")) {
                              var skip_EditSuccessful = new CodeInstruction(OpCodes.Ldc_I4_1);
                              skip_EditSuccessful.labels.Add(lblEditSuccessful);
                              yield return skip_EditSuccessful;
                              yield return new CodeInstruction(OpCodes.Ldarg_0);
                              yield return instruction;
                              markPostPatchCode = false;
                              didAllPatches = true;
                              continue;
                           }
                        }
                     }
                  }
                  yield return instruction;
               }
            }
         }
      }

      public override void Entry(IModHelper helper) {
         Config.instance = helper.ReadConfig<Config>();

         var harmony = HarmonyInstance.Create("mod.berkay2578.ChestEx");
         // Handle hardcoded chest capacity and do ItemGrabMenu->ChestExMenu arbitration
         {
            var methodsToPatch = new List<MethodInfo>() {
               Harmony.AccessTools.Method(typeof(Chest), "updateWhenCurrentLocation"),
               Harmony.AccessTools.Method(typeof(Chest), "grabItemFromChest"),
               Harmony.AccessTools.Method(typeof(Chest), "grabItemFromInventory")
            };

            var patch = new HarmonyMethod(typeof(ChestExtensions.CapacityRaiser).GetMethod("TranspilerPatches"));
            var patch2 = new HarmonyMethod(typeof(ChestExtensions.ItemGrabMenuArbitration).GetMethod("TranspilerPatches"));

            // Patch capacity
            harmony.Patch(Harmony.AccessTools.Method(typeof(Chest), "addItem"), null, null, patch);
            // Force arbitration to ChestExMenu
            foreach (var method in methodsToPatch)
               harmony.Patch(method, null, null, patch2);
         }
         // Handle default .ctor call
         {
            var ctor = typeof(ItemGrabMenu).GetConstructor(AccessTools.all, null, new Type[] { typeof(IList<Item>), typeof(object) }, null);
            var patch = new HarmonyMethod(typeof(ChestExtensions.ItemGrabMenuArbitration.CtorPatches).GetMethod("TranspilerPatches"));

            harmony.Patch(ctor, null, null, patch);
         }

         Boolean hasChestsAnywhere = false;
         foreach (var item in helper.ModRegistry.GetAll()) {
            if (item.Manifest.UniqueID == strChestsAnywhereUID) {
               hasChestsAnywhere = true;
               break;
            }
         }
         if (hasChestsAnywhere) {
            this.Monitor.Log("ChestsAnywhere is found, installing dynamic compatibility patches...", LogLevel.Info);
            this.Monitor.Log($"ChestsAnywhere version: {helper.ModRegistry.Get(strChestsAnywhereUID).Manifest.Version}, dynamic compability patch is made with v1.16.1 (Release) as base.", LogLevel.Debug);
            var typeChestsAnywhere_ChestContainer = Type.GetType("Pathoschild.Stardew.ChestsAnywhere.Framework.Containers.ChestContainer, ChestsAnywhere");
            var typeChestsAnywhere_ManageChestOverlay = Type.GetType("Pathoschild.Stardew.ChestsAnywhere.Menus.Overlays.ManageChestOverlay, ChestsAnywhere");
            harmony.Patch(AccessTools.Method(typeChestsAnywhere_ChestContainer, "OpenMenu"), null, null, new HarmonyMethod(typeof(ChestsAnywhereCompatibility.ChestContainer).GetMethod("OpenMenu_TranspilerPatches")));
            harmony.Patch(AccessTools.Method(typeChestsAnywhere_ManageChestOverlay, "ReinitialiseComponents"), null, null, new HarmonyMethod(typeof(ChestsAnywhereCompatibility.ManageChestOverlay).GetMethod("ReinitialiseComponents_TranspilerPatches")));
         }
      }
   }
}
