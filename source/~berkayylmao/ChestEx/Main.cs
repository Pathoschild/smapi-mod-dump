/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/berkayylmao/StardewValleyMods
**
*************************************************/

//
//    Copyright (C) 2020 Berkay Yigit <berkaytgy@gmail.com>
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as published
//    by the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program. If not, see <https://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using Harmony;

using StardewModdingAPI;

using StardewValley.Menus;
using StardewValley.Objects;

namespace ChestEx {
   public class Main : Mod {
      public static class Patches {
         public static class MainPatches {
            [HarmonyPatch(typeof(Chest))]
            public static class PatchesFor_SVChest {
               [HarmonyTranspiler]
               public static IEnumerable<CodeInstruction> TranspilerPatchFor_addItem(IEnumerable<CodeInstruction> instructions) {
                  Boolean patched = false;

                  foreach (var instruction in instructions) {
                     if (instruction.opcode == OpCodes.Ldc_I4_S && (SByte)instruction.operand == Chest.capacity) {
                        yield return new CodeInstruction(OpCodes.Ldc_I4, Math.Min(Int32.MaxValue, Config.instance.getCapacity()));

                        patched = true;
                        continue;
                     }
                     yield return instruction;
                  }

                  if (patched)
                     GlobalVars.SMAPIMonitor.Log("Successfully patched 'StardewValley.Chest.addItem()'.", LogLevel.Info);
                  else
                     GlobalVars.SMAPIMonitor.Log("Could not patch 'StardewValley.Chest.addItem()' to raise chest capacity!", LogLevel.Error);
               }

               [HarmonyTranspiler]
               public static IEnumerable<CodeInstruction> TranspilerPatches_RedirectingToChestExMenu(IEnumerable<CodeInstruction> instructions) {
                  var orgCtor = AccessTools.Constructor(typeof(ItemGrabMenu),
                                                        new Type[] { typeof(IList<StardewValley.Item>), typeof(Boolean), typeof(Boolean), typeof(InventoryMenu.highlightThisItem), typeof(ItemGrabMenu.behaviorOnItemSelect), typeof(String), typeof(ItemGrabMenu.behaviorOnItemSelect), typeof(Boolean), typeof(Boolean), typeof(Boolean), typeof(Boolean), typeof(Boolean), typeof(Int32), typeof(StardewValley.Item), typeof(Int32), typeof(Object) });
                  var newCtor = AccessTools.Constructor(typeof(Types.CustomTypes.ChestExMenu.MainMenu),
                                                        new Type[] { typeof(IList<StardewValley.Item>), typeof(Boolean), typeof(Boolean), typeof(InventoryMenu.highlightThisItem), typeof(ItemGrabMenu.behaviorOnItemSelect), typeof(String), typeof(ItemGrabMenu.behaviorOnItemSelect), typeof(Boolean), typeof(Boolean), typeof(Boolean), typeof(Boolean), typeof(Boolean), typeof(Int32), typeof(StardewValley.Item), typeof(Int32), typeof(Object) });

                  foreach (var instruction in instructions) {
                     if (instruction.opcode == OpCodes.Newobj) {
                        // ItemGrabMenu.ctor[2]
                        if ((ConstructorInfo)instruction.operand == orgCtor) {
                           yield return new CodeInstruction(OpCodes.Newobj, newCtor);
                           continue;
                        }
                     }
                     // IsInst ItemGrabMenu
                     else if (instruction.opcode == OpCodes.Isinst
                         && (Type)instruction.operand == typeof(ItemGrabMenu)) {
                        yield return new CodeInstruction(OpCodes.Isinst, typeof(Types.BaseTypes.ICustomItemGrabMenu));
                        continue;
                     }
                     yield return instruction;
                  }
               }

               public static void ApplyAll(HarmonyInstance harmony) {
                  var methodsToPatch = new List<MethodInfo>() {
                     AccessTools.Method(typeof(Chest), "updateWhenCurrentLocation"),
                     AccessTools.Method(typeof(Chest), "grabItemFromChest"),
                     AccessTools.Method(typeof(Chest), "grabItemFromInventory")
                  };

                  // Force arbitration to ChestExMenu
                  foreach (var method in methodsToPatch)
                     harmony.Patch(method, null, null, new HarmonyMethod(typeof(PatchesFor_SVChest).GetMethod("TranspilerPatches_RedirectingToChestExMenu")));

                  // Set new capacity
                  harmony.Patch(AccessTools.Method(typeof(Chest), "addItem"), null, null,
                                new HarmonyMethod(typeof(PatchesFor_SVChest).GetMethod("TranspilerPatchFor_addItem")));
               }
            }

            [HarmonyPatch(typeof(ItemGrabMenu))]
            public static class PatchesFor_ItemGrabMenu {
               [HarmonyTranspiler]
               public static IEnumerable<CodeInstruction> TranspilerPatchFor_ctor(IEnumerable<CodeInstruction> instructions, ILGenerator ilg) {
                  var baseCtor = AccessTools.Constructor(typeof(MenuWithInventory),
                                                         new Type[] { typeof(InventoryMenu.highlightThisItem), typeof(Boolean), typeof(Boolean), typeof(Int32), typeof(Int32), typeof(Int32) });
                  Boolean patched = false;
                  var lblSkip = ilg.DefineLabel();

                  foreach (var instruction in instructions) {
                     if (!patched && instruction.opcode == OpCodes.Call
                        && (MethodBase)instruction.operand == baseCtor) {
                        yield return new CodeInstruction(OpCodes.Ldarg_2);
                        yield return new CodeInstruction(OpCodes.Isinst, typeof(Types.BaseTypes.ICustomItemGrabMenu.MenuWithInventoryCtorParams));
                        yield return new CodeInstruction(OpCodes.Brfalse, lblSkip);
                        // is MenuWithInventoryCtorParams
                        {
                           yield return new CodeInstruction(OpCodes.Pop);
                           yield return new CodeInstruction(OpCodes.Pop);
                           yield return new CodeInstruction(OpCodes.Pop);
                           yield return new CodeInstruction(OpCodes.Pop);
                           yield return new CodeInstruction(OpCodes.Pop);
                           yield return new CodeInstruction(OpCodes.Pop);
                           yield return new CodeInstruction(OpCodes.Pop);
                           yield return new CodeInstruction(OpCodes.Ldarg_0);
                           yield return new CodeInstruction(OpCodes.Ldarg_2);
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Types.BaseTypes.ICustomItemGrabMenu.MenuWithInventoryCtorParams), "HighlighterMethod"));
                           yield return new CodeInstruction(OpCodes.Ldarg_2);
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Types.BaseTypes.ICustomItemGrabMenu.MenuWithInventoryCtorParams), "OKButton"));
                           yield return new CodeInstruction(OpCodes.Ldarg_2);
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Types.BaseTypes.ICustomItemGrabMenu.MenuWithInventoryCtorParams), "TrashCan"));
                           yield return new CodeInstruction(OpCodes.Ldarg_2);
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Types.BaseTypes.ICustomItemGrabMenu.MenuWithInventoryCtorParams), "InventoryXOffset"));
                           yield return new CodeInstruction(OpCodes.Ldarg_2);
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Types.BaseTypes.ICustomItemGrabMenu.MenuWithInventoryCtorParams), "InventoryYOffset"));
                           yield return new CodeInstruction(OpCodes.Ldarg_2);
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Types.BaseTypes.ICustomItemGrabMenu.MenuWithInventoryCtorParams), "MenuOffsetHack"));
                           yield return instruction;
                           yield return new CodeInstruction(OpCodes.Ret);
                        }

                        // add branching for if condition was not true
                        var jmp = new CodeInstruction(instruction);
                        jmp.labels.Add(lblSkip);
                        yield return jmp;

                        patched = true;
                     } else {
                        yield return instruction;
                     }
                  }

                  if (patched)
                     GlobalVars.SMAPIMonitor.Log("Successfully patched 'StardewValley.Menus.ItemGrabMenu.ctor(IList<StardewValley.Item>, System.Object)'.", LogLevel.Info);
                  else
                     GlobalVars.SMAPIMonitor.Log("Could not patch 'StardewValley.Menus.ItemGrabMenu.ctor(IList<StardewValley.Item>, System.Object)' to redirect 'ICustomItemGrabMenu.ctor' to 'MenuWithInventory.ctor'!", LogLevel.Error);
               }

               public static void ApplyAll(HarmonyInstance harmony) {
                  // Force skip ItemGrabMenu ctor when called from ChestExMenu
                  harmony.Patch(
                     AccessTools.Constructor(typeof(ItemGrabMenu), new Type[] { typeof(IList<StardewValley.Item>), typeof(Object) }),
                     null, null, new HarmonyMethod(typeof(PatchesFor_ItemGrabMenu).GetMethod("TranspilerPatchFor_ctor")));

               }
            }
         }

         public static class CompatibilityPatches {
            public static class Automate {
               public const String CONST_UID = "Pathoschild.Automate";
               public const String CONST_VERSION = "1.18.0";

               [HarmonyPatch]
               public static class PatchesFor_ChestContainer {
                  private static Int32 returnNewCapacity() {
                     return Math.Min(Int32.MaxValue, Config.instance.getCapacity());
                  }

                  [HarmonyTranspiler]
                  public static IEnumerable<CodeInstruction> TranspilerPatchFor_ctor(IEnumerable<CodeInstruction> instructions) {
                     Boolean patched = false;

                     foreach (var instruction in instructions) {
                        // compiler-generated lambda {return 36;}
                        if (!patched && instruction.opcode == OpCodes.Ldftn) {
                           yield return new CodeInstruction(OpCodes.Ldftn, AccessTools.Method(typeof(PatchesFor_ChestContainer), "returnNewCapacity"));

                           patched = true;
                           continue;
                        }
                        yield return instruction;
                     }

                     if (patched)
                        GlobalVars.SMAPIMonitor.Log("Successfully patched 'Automate.ChestContainer.ctor'.", LogLevel.Info);
                     else
                        GlobalVars.SMAPIMonitor.Log("Could not patch 'Automate.ChestContainer.ctor' to raise chest capacity for Automate!", LogLevel.Error);
                  }
               }

               public static void ApplyAll(HarmonyInstance harmony) {
                  if (GlobalVars.SMAPIHelper.ModRegistry.Get(CONST_UID) is IModInfo automateModInfo) {
                     GlobalVars.SMAPIMonitor.Log("Automate is found, installing dynamic compatibility patches...", LogLevel.Info);
                     GlobalVars.SMAPIMonitor.Log($"The target version of Automate for the compatibility patch is '{CONST_VERSION}'.", LogLevel.Debug);

                     if (String.Equals(automateModInfo.Manifest.Version.ToString(), CONST_VERSION, StringComparison.OrdinalIgnoreCase)) {
                        // user is running the same version of Automate
                        GlobalVars.SMAPIMonitor.Log("ChestEx is fully compatible with this Automate version.", LogLevel.Info);
                     } else if (automateModInfo.Manifest.Version.IsNewerThan(CONST_VERSION)) {
                        // user is running a newer version of Automate
                        GlobalVars.SMAPIMonitor.Log("You seem to be running a newer version of Automate! This warning can safely be ignored if you don't experience any issues. However, if you do experience any issues, please report it to me on Discord or on Nexus.", LogLevel.Warn);
                     } else if (automateModInfo.Manifest.Version.IsOlderThan(CONST_VERSION)) {
                        // user is running an older version of Automate
                        GlobalVars.SMAPIMonitor.Log("You seem to be running an older version of Automate! There is a high chance that you will experience issues, please update your copy of Automate.", LogLevel.Warn);
                     }

                     harmony.Patch(
                        AccessTools.Constructor(
                           Type.GetType("Pathoschild.Stardew.Automate.Framework.Storage.ChestContainer, Automate"),
                           new Type[] { typeof(Chest), typeof(StardewValley.GameLocation), typeof(Microsoft.Xna.Framework.Vector2), typeof(IReflectionHelper) }),
                        null,
                        null,
                        new HarmonyMethod(typeof(PatchesFor_ChestContainer).GetMethod("TranspilerPatchFor_ctor")));
                  }
               }
            }

            public static class ChestsAnywhere {
               public const String CONST_UID = "Pathoschild.ChestsAnywhere";
               public const String CONST_VERSION = "1.17.1";

               [HarmonyPatch]
               public static class PatchesFor_ChestContainer {
                  [HarmonyTranspiler]
                  public static IEnumerable<CodeInstruction> TranspilerPatchFor_OpenMenu(IEnumerable<CodeInstruction> instructions) {
                     var orgCtor = AccessTools.Constructor(typeof(ItemGrabMenu),
                                                        new Type[] { typeof(IList<StardewValley.Item>), typeof(Boolean), typeof(Boolean), typeof(InventoryMenu.highlightThisItem), typeof(ItemGrabMenu.behaviorOnItemSelect), typeof(String), typeof(ItemGrabMenu.behaviorOnItemSelect), typeof(Boolean), typeof(Boolean), typeof(Boolean), typeof(Boolean), typeof(Boolean), typeof(Int32), typeof(StardewValley.Item), typeof(Int32), typeof(Object) });
                     var newCtor = AccessTools.Constructor(typeof(Types.CustomTypes.ChestExMenu.MainMenu),
                                                        new Type[] { typeof(IList<StardewValley.Item>), typeof(Boolean), typeof(Boolean), typeof(InventoryMenu.highlightThisItem), typeof(ItemGrabMenu.behaviorOnItemSelect), typeof(String), typeof(ItemGrabMenu.behaviorOnItemSelect), typeof(Boolean), typeof(Boolean), typeof(Boolean), typeof(Boolean), typeof(Boolean), typeof(Int32), typeof(StardewValley.Item), typeof(Int32), typeof(Object) });
                     Boolean patched = false;

                     foreach (var instruction in instructions) {
                        if (instruction.opcode == OpCodes.Newobj) {
                           // ItemGrabMenu.ctor[2]
                           if ((ConstructorInfo)instruction.operand == orgCtor) {
                              yield return new CodeInstruction(OpCodes.Newobj, newCtor);

                              patched = true;
                              continue;
                           }
                        }
                        yield return instruction;
                     }

                     if (patched)
                        GlobalVars.SMAPIMonitor.Log("Successfully patched 'ChestsAnywhere.ChestContainer.OpenMenu'.", LogLevel.Info);
                     else
                        GlobalVars.SMAPIMonitor.Log("Could not patch 'ChestsAnywhere.ChestContainer.OpenMenu' to redirect 'ItemGrabMenu.ctor' to 'ChestExMenu.MainMenu.ctor'!", LogLevel.Error);
                  }
               }

               [HarmonyPatch]
               public static class PatchesFor_BaseChestOverlay {
                  [HarmonyTranspiler]
                  public static IEnumerable<CodeInstruction> TranspilerPatchFor_ReinitializeComponents(IEnumerable<CodeInstruction> instructions, ILGenerator ilg) {
                     var target_type = Type.GetType("Pathoschild.Stardew.ChestsAnywhere.Menus.Overlays.BaseChestOverlay, ChestsAnywhere");
                     var xna_Rectangle_ctor = AccessTools.Constructor(typeof(Microsoft.Xna.Framework.Rectangle),
                                                                      new Type[] { typeof(Int32), typeof(Int32), typeof(Int32), typeof(Int32) });

                     Boolean patched = false;
                     var lblSkip = ilg.DefineLabel();

                     foreach (var instruction in instructions) {
                        if (!patched && instruction.opcode == OpCodes.Call && (MethodBase)instruction.operand == xna_Rectangle_ctor) {
                           yield return instruction;

                           yield return new CodeInstruction(OpCodes.Ldarg_0);
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(target_type, "Menu"));
                           yield return new CodeInstruction(OpCodes.Isinst, typeof(Types.CustomTypes.ChestExMenu.MainMenu));
                           yield return new CodeInstruction(OpCodes.Brfalse, lblSkip);

                           yield return new CodeInstruction(OpCodes.Ldloca_S, 0);
                           yield return new CodeInstruction(OpCodes.Ldarg_0);
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(target_type, "Menu"));
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(MenuWithInventory), "inventory"));
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(IClickableMenu), "xPositionOnScreen"));
                           yield return new CodeInstruction(OpCodes.Ldarg_0);
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(target_type, "Menu"));
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(MenuWithInventory), "inventory"));
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(IClickableMenu), "yPositionOnScreen"));
                           yield return new CodeInstruction(OpCodes.Ldarg_0);
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(target_type, "Menu"));
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(IClickableMenu), "width"));
                           yield return new CodeInstruction(OpCodes.Ldarg_0);
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(target_type, "Menu"));
                           yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(IClickableMenu), "height"));
                           yield return instruction;

                           var jmp = new CodeInstruction(OpCodes.Nop);
                           jmp.labels.Add(lblSkip);
                           yield return jmp;

                           patched = true;
                           continue;
                        }
                        yield return instruction;
                     }


                     if (patched)
                        GlobalVars.SMAPIMonitor.Log("Successfully patched 'ChestsAnywhere.Menus.Overlays.BaseChestOverlay.ReinitializeComponents'.", LogLevel.Info);
                     else
                        GlobalVars.SMAPIMonitor.Log("Could not patch 'ChestsAnywhere.Menus.Overlays.BaseChestOverlay.ReinitializeComponents' to move ChestsAnywhere buttons!", LogLevel.Error);
                  }
               }

               [HarmonyPatch]
               public static class PatchesFor_ChestOverlay {
                  [HarmonyTranspiler]
                  public static IEnumerable<CodeInstruction> TranspilerPatchFor_ReinitializeComponents(IEnumerable<CodeInstruction> instructions) {
                     var target_type = Type.GetType("Pathoschild.Stardew.ChestsAnywhere.Framework.ModConfig, ChestsAnywhere");
                     var patch_fn = AccessTools.Method(target_type, "get_AddOrganizePlayerInventoryButton");

                     Boolean found_target = false;
                     Boolean patched = false;

                     foreach (var instruction in instructions) {
                        if (!found_target) {
                           found_target = instruction.opcode == OpCodes.Callvirt && (MethodBase)instruction.operand == patch_fn;
                        } else if (!patched) {
                           if (instruction.opcode == OpCodes.Brfalse) {
                              instruction.opcode = OpCodes.Br;
                              patched = true;
                           } else if (instruction.opcode == OpCodes.Brfalse_S) {
                              instruction.opcode = OpCodes.Br_S;
                              patched = true;
                           }

                           if (patched)
                              yield return new CodeInstruction(OpCodes.Pop);
                        }
                        yield return instruction;
                     }


                     if (patched)
                        GlobalVars.SMAPIMonitor.Log("Successfully patched 'ChestsAnywhere.Menus.Overlays.ChestOverlay.ReinitializeComponents'.", LogLevel.Info);
                     else
                        GlobalVars.SMAPIMonitor.Log("Could not patch 'ChestsAnywhere.Menus.Overlays.ChestOverlay.ReinitializeComponents' to make ChestAnywhere skip creating player inventory organize button!", LogLevel.Error);
                  }
               }

               public static void ApplyAll(HarmonyInstance harmony) {
                  if (GlobalVars.SMAPIHelper.ModRegistry.Get(CONST_UID) is IModInfo chestsAnywhereModInfo) {
                     GlobalVars.SMAPIMonitor.Log("ChestsAnywhere is found, installing dynamic compatibility patches...", LogLevel.Info);
                     GlobalVars.SMAPIMonitor.Log($"The target version of ChestsAnywhere for the compatibility patch is '{CONST_VERSION}'.", LogLevel.Debug);

                     if (String.Equals(chestsAnywhereModInfo.Manifest.Version.ToString(), CONST_VERSION, StringComparison.OrdinalIgnoreCase)) {
                        // user is running the same version of ChestsAnywhere
                        GlobalVars.SMAPIMonitor.Log("ChestEx is fully compatible with this ChestsAnywhere version.", LogLevel.Info);
                     } else if (chestsAnywhereModInfo.Manifest.Version.IsNewerThan(CONST_VERSION)) {
                        // user is running a newer version of ChestsAnywhere
                        GlobalVars.SMAPIMonitor.Log("You seem to be running a newer version of ChestsAnywhere! This warning can safely be ignored if you don't experience any issues. However, if you do experience any issues, please report it to me on Discord or on Nexus.", LogLevel.Warn);
                     } else if (chestsAnywhereModInfo.Manifest.Version.IsOlderThan(CONST_VERSION)) {
                        // user is running an older version of ChestsAnywhere
                        GlobalVars.SMAPIMonitor.Log("You seem to be running an older version of ChestsAnywhere! There is a high chance that you will experience issues, please update your copy of ChestsAnywhere.", LogLevel.Warn);
                     }

                     harmony.Patch(
                        AccessTools.Method(Type.GetType("Pathoschild.Stardew.ChestsAnywhere.Framework.Containers.ChestContainer, ChestsAnywhere"), "OpenMenu"),
                        null,
                        null,
                        new HarmonyMethod(typeof(PatchesFor_ChestContainer).GetMethod("TranspilerPatchFor_OpenMenu")));
                     harmony.Patch(
                        AccessTools.Method(Type.GetType("Pathoschild.Stardew.ChestsAnywhere.Menus.Overlays.BaseChestOverlay, ChestsAnywhere"), "ReinitializeComponents"),
                        null,
                        null,
                        new HarmonyMethod(typeof(PatchesFor_BaseChestOverlay).GetMethod("TranspilerPatchFor_ReinitializeComponents")));
                     harmony.Patch(
                        AccessTools.Method(Type.GetType("Pathoschild.Stardew.ChestsAnywhere.Menus.Overlays.ChestOverlay, ChestsAnywhere"), "ReinitializeComponents"),
                        null,
                        null,
                        new HarmonyMethod(typeof(PatchesFor_ChestOverlay).GetMethod("TranspilerPatchFor_ReinitializeComponents")));
                  }
               }
            }
         }
      }

      public override void Entry(IModHelper helper) {
         Config.instance = helper.ReadConfig<Config>();
         GlobalVars.SMAPIHelper = helper;
         GlobalVars.SMAPIMonitor = this.Monitor;

         var harmony = HarmonyInstance.Create("mod.berkayylmao.ChestEx");
         Patches.MainPatches.PatchesFor_ItemGrabMenu.ApplyAll(harmony);
         Patches.MainPatches.PatchesFor_SVChest.ApplyAll(harmony);
         Patches.CompatibilityPatches.Automate.ApplyAll(harmony);
         Patches.CompatibilityPatches.ChestsAnywhere.ApplyAll(harmony);
      }
   }
}
