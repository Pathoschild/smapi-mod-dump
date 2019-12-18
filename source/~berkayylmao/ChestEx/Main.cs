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
using System.Reflection;
using System.Reflection.Emit;

using Harmony;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace ChestEx {
   public class Main : Mod {
      public const String strAutomateUID = "Pathoschild.Automate";
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
                           new Type[] { typeof(IList<Item>), typeof(Boolean), typeof(Boolean), typeof(InventoryMenu.highlightThisItem), typeof(ItemGrabMenu.behaviorOnItemSelect), typeof(String), typeof(ItemGrabMenu.behaviorOnItemSelect), typeof(Boolean), typeof(Boolean), typeof(Boolean), typeof(Boolean), typeof(Boolean), typeof(Int32), typeof(Item), typeof(Int32), typeof(System.Object) }, null),
                        typeof(ItemGrabMenu.behaviorOnItemSelect).GetConstructor(AccessTools.all, null,
                           new Type[] { typeof(Item), typeof(Farmer) }, null)
               };
               MethodBase[] _newCalls = {
                        typeof(ChestExMenu).GetConstructor(AccessTools.all, null,
                           new Type[] { typeof(IList<Item>), typeof(Boolean), typeof(Boolean), typeof(InventoryMenu.highlightThisItem), typeof(ChestExMenu.behaviorOnItemSelect), typeof(String), typeof(ChestExMenu.behaviorOnItemSelect), typeof(Boolean), typeof(Boolean), typeof(Boolean), typeof(Boolean), typeof(Boolean), typeof(Int32), typeof(Item), typeof(Int32), typeof(System.Object) }, null),
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
                      && (SByte)instruction.operand == Chest.capacity) {
                     yield return new CodeInstruction(OpCodes.Ldc_I4, Math.Min(Int32.MaxValue, Config.instance.getCapacity()));
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
                           new Type[] { typeof(IList<Item>), typeof(Boolean), typeof(Boolean), typeof(InventoryMenu.highlightThisItem), typeof(ItemGrabMenu.behaviorOnItemSelect), typeof(String), typeof(ItemGrabMenu.behaviorOnItemSelect), typeof(Boolean), typeof(Boolean), typeof(Boolean), typeof(Boolean), typeof(Boolean), typeof(Int32), typeof(Item), typeof(Int32), typeof(System.Object) }, null);
               MethodBase _newCall =
                        typeof(ChestExMenu).GetConstructor(AccessTools.all, null,
                           new Type[] { typeof(IList<Item>), typeof(Boolean), typeof(Boolean), typeof(InventoryMenu.highlightThisItem), typeof(ChestExMenu.behaviorOnItemSelect), typeof(String), typeof(ChestExMenu.behaviorOnItemSelect), typeof(Boolean), typeof(Boolean), typeof(Boolean), typeof(Boolean), typeof(Boolean), typeof(Int32), typeof(Item), typeof(Int32), typeof(System.Object) }, null);

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
               var typeChestsAnywhere_ManageChestOverlay = Type.GetType("Pathoschild.Stardew.ChestsAnywhere.Menus.Overlays.BaseChestOverlay, ChestsAnywhere");
               var xna_Rectangle_ctor = typeof(Microsoft.Xna.Framework.Rectangle).GetConstructor(AccessTools.all, null,
                  new Type[] { typeof(Int32), typeof(Int32), typeof(Int32), typeof(Int32) }, null);

               Boolean didAllPatches = false;
               Label lblNotChestExMenu = ilg.DefineLabel();

               foreach (var instruction in instructions) {
                  if (!didAllPatches) {
                     if (instruction.opcode == OpCodes.Call &&
                           (MethodBase)instruction.operand == xna_Rectangle_ctor) {
                        yield return instruction;

                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeChestsAnywhere_ManageChestOverlay, "Menu"));
                        yield return new CodeInstruction(OpCodes.Isinst, typeof(ChestExMenu));
                        yield return new CodeInstruction(OpCodes.Brfalse, lblNotChestExMenu);

                        yield return new CodeInstruction(OpCodes.Ldloca_S, 0);
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeChestsAnywhere_ManageChestOverlay, "Menu"));
                        yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StardewValley.Menus.MenuWithInventory), "inventory"));
                        yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StardewValley.Menus.IClickableMenu), "xPositionOnScreen"));
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeChestsAnywhere_ManageChestOverlay, "Menu"));
                        yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StardewValley.Menus.MenuWithInventory), "inventory"));
                        yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StardewValley.Menus.IClickableMenu), "yPositionOnScreen"));
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeChestsAnywhere_ManageChestOverlay, "Menu"));
                        yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StardewValley.Menus.IClickableMenu), "width"));
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeChestsAnywhere_ManageChestOverlay, "Menu"));
                        yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StardewValley.Menus.IClickableMenu), "height"));
                        yield return instruction;

                        var skipCode = new CodeInstruction(OpCodes.Nop);
                        skipCode.labels.Add(lblNotChestExMenu);
                        yield return skipCode;
                        didAllPatches = true;

                        continue;
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
            var ctor = typeof(ItemGrabMenu).GetConstructor(AccessTools.all, null, new Type[] { typeof(IList<Item>), typeof(System.Object) }, null);
            var patch = new HarmonyMethod(typeof(ChestExtensions.ItemGrabMenuArbitration.CtorPatches).GetMethod("TranspilerPatches"));

            harmony.Patch(ctor, null, null, patch);
         }

         if (helper.ModRegistry.IsLoaded(strAutomateUID)) {
            this.Monitor.Log("Automate is found, installing dynamic compatibility patches...", LogLevel.Info);
            this.Monitor.Log($"Automate version: {helper.ModRegistry.Get(strAutomateUID).Manifest.Version}, dynamic compability patch is made with v1.14.0 (Release) as base.", LogLevel.Debug);
            var typeAutomate_ChestContainer = Type.GetType("Pathoschild.Stardew.Automate.Framework.Storage.ChestContainer, Automate");
            harmony.Patch(AccessTools.Method(typeAutomate_ChestContainer, "Store"), null, null, new HarmonyMethod(typeof(ChestExtensions.CapacityRaiser).GetMethod("TranspilerPatches")));
         }
         if (helper.ModRegistry.IsLoaded(strChestsAnywhereUID)) {
            this.Monitor.Log("ChestsAnywhere is found, installing dynamic compatibility patches...", LogLevel.Info);
            this.Monitor.Log($"ChestsAnywhere version: {helper.ModRegistry.Get(strChestsAnywhereUID).Manifest.Version}, dynamic compability patch is made with v1.17.1 (Release) as base.", LogLevel.Debug);
            var typeChestsAnywhere_ChestContainer = Type.GetType("Pathoschild.Stardew.ChestsAnywhere.Framework.Containers.ChestContainer, ChestsAnywhere");
            var typeChestsAnywhere_ManageChestOverlay = Type.GetType("Pathoschild.Stardew.ChestsAnywhere.Menus.Overlays.BaseChestOverlay, ChestsAnywhere");
            harmony.Patch(AccessTools.Method(typeChestsAnywhere_ChestContainer, "OpenMenu"), null, null, new HarmonyMethod(typeof(ChestsAnywhereCompatibility.ChestContainer).GetMethod("OpenMenu_TranspilerPatches")));
            harmony.Patch(AccessTools.Method(typeChestsAnywhere_ManageChestOverlay, "ReinitializeComponents"), null, null, new HarmonyMethod(typeof(ChestsAnywhereCompatibility.ManageChestOverlay).GetMethod("ReinitialiseComponents_TranspilerPatches")));
         }
      }
   }
}
