/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/berkayylmao/StardewValleyMods
**
*************************************************/

#region License

// clang-format off
// 
//    ChestEx (StardewValleyMods)
//    Copyright (c) 2021 Berkay Yigit <berkaytgy@gmail.com>
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
// clang-format on

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;

using ChestEx.Types.BaseTypes;
using ChestEx.Types.CustomTypes.ChestExMenu;
using ChestEx.Types.CustomTypes.ExtendedSVObjects;

using Harmony;

using JetBrains.Annotations;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

using Object = System.Object;

namespace ChestEx {
  [UsedImplicitly]
  public class Main : Mod {
    public static class Patches {
      [HarmonyPatch(typeof(Chest))]
      public static class SVChest {
        [HarmonyPostfix]
        [UsedImplicitly]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static void postfixGetActualCapacity(ref Int32 __result) {
          if (__result == Chest.capacity) __result = Math.Min(Int32.MaxValue, Math.Max(0, Config.Get().GetCapacity()));
        }

        [HarmonyPrefix]
        [UsedImplicitly]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static Boolean prefixShowMenu(Chest __instance) {
          if (__instance.SpecialChestType != Chest.SpecialChestTypes.None || __instance.playerChest is null || !__instance.playerChest.Value) return true;
          if (Game1.activeClickableMenu is CustomItemGrabMenu menu) menu.exitThisMenu(false);
          Game1.activeClickableMenu = new MainMenu(__instance.GetItemsForPlayer(Game1.player.UniqueMultiplayerID),
                                                   false,
                                                   true,
                                                   InventoryMenu.highlightAllItems,
                                                   __instance.grabItemFromInventory,
                                                   null,
                                                   __instance.grabItemFromChest,
                                                   false,
                                                   true,
                                                   true,
                                                   true,
                                                   true,
                                                   1,
                                                   __instance.fridge is null ? null : __instance,
                                                   -1,
                                                   __instance);

          return false;
        }

        public static void ApplyAll(HarmonyInstance harmony) {
          // Force arbitration to ChestExMenu
          harmony.Patch(AccessTools.Method(typeof(Chest), "ShowMenu"), new HarmonyMethod(AccessTools.Method(typeof(SVChest), "prefixShowMenu")));

          // Set new capacity
          harmony.Patch(AccessTools.Method(typeof(Chest), "GetActualCapacity"), null, new HarmonyMethod(AccessTools.Method(typeof(SVChest), "postfixGetActualCapacity")));

          // Allow drawing custom hinges colour
          harmony.Patch(AccessTools.Method(typeof(Chest), "draw", new[] { typeof(SpriteBatch), typeof(Int32), typeof(Int32), typeof(Single) }),
                        new HarmonyMethod(typeof(ExtendedChest).GetMethod("BeforeDraw")));
        }
      }

      [HarmonyPatch(typeof(ItemGrabMenu))]
      public static class SVItemGrabMenu {
        [HarmonyTranspiler]
        [UsedImplicitly]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static IEnumerable<CodeInstruction> transpilerCtor(IEnumerable<CodeInstruction> instructions, ILGenerator ilg) {
          ConstructorInfo base_ctor = AccessTools.Constructor(typeof(MenuWithInventory),
                                                              new[] {
                                                                typeof(InventoryMenu.highlightThisItem),
                                                                typeof(Boolean),
                                                                typeof(Boolean),
                                                                typeof(Int32),
                                                                typeof(Int32),
                                                                typeof(Int32)
                                                              });
          Boolean patched  = false;
          Label   lbl_skip = ilg.DefineLabel();

          foreach (CodeInstruction instruction in instructions)
            if (!patched && instruction.opcode == OpCodes.Call && (MethodBase)instruction.operand == base_ctor) {
              yield return new CodeInstruction(OpCodes.Ldarg_2);
              yield return new CodeInstruction(OpCodes.Isinst, typeof(CustomItemGrabMenu.MenuWithInventoryCtorParams));
              yield return new CodeInstruction(OpCodes.Brfalse, lbl_skip);
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
                yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(CustomItemGrabMenu.MenuWithInventoryCtorParams), "mHighlighterMethod"));
                yield return new CodeInstruction(OpCodes.Ldarg_2);
                yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(CustomItemGrabMenu.MenuWithInventoryCtorParams), "mOKButton"));
                yield return new CodeInstruction(OpCodes.Ldarg_2);
                yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(CustomItemGrabMenu.MenuWithInventoryCtorParams), "mTrashCan"));
                yield return new CodeInstruction(OpCodes.Ldarg_2);
                yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(CustomItemGrabMenu.MenuWithInventoryCtorParams), "mInventoryXOffset"));
                yield return new CodeInstruction(OpCodes.Ldarg_2);
                yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(CustomItemGrabMenu.MenuWithInventoryCtorParams), "mInventoryYOffset"));
                yield return new CodeInstruction(OpCodes.Ldarg_2);
                yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(CustomItemGrabMenu.MenuWithInventoryCtorParams), "mMenuOffsetHack"));
                yield return instruction;
                yield return new CodeInstruction(OpCodes.Ret);
              }

              // add branching for if condition was not true
              var jmp = new CodeInstruction(instruction);
              jmp.labels.Add(lbl_skip);

              yield return jmp;

              patched = true;
            }
            else
              yield return instruction;

          if (patched) { GlobalVars.gSMAPIMonitor.Log("Successfully patched 'StardewValley.Menus.ItemGrabMenu.ctor(IList<StardewValley.Item>, System.Object)'.", LogLevel.Info); }
          else {
            GlobalVars.gSMAPIMonitor
                      .Log("Could not patch 'StardewValley.Menus.ItemGrabMenu.ctor(IList<StardewValley.Item>, System.Object)' to redirect 'ICustomItemGrabMenu.ctor' to 'MenuWithInventory.ctor'!",
                           LogLevel.Error);
          }
        }

        public static void ApplyAll(HarmonyInstance harmony) {
          // Force skip ItemGrabMenu ctor when called from ChestExMenu
          harmony.Patch(AccessTools.Constructor(typeof(ItemGrabMenu), new[] { typeof(IList<Item>), typeof(Object) }),
                        transpiler: new HarmonyMethod(AccessTools.Method(typeof(SVItemGrabMenu), "transpilerCtor")));
        }
      }

      [HarmonyPatch(typeof(StardewValley.Object))]
      public static class SVObject {
        [HarmonyPostfix]
        [UsedImplicitly]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static void postfixPlacementAction(GameLocation location, Int32 x, Int32 y, Boolean __result) {
          if (!Context.IsWorldReady) return;
          if (__result) GlobalVars.OnObjectPlaced(location, new Vector2(x / 64.0f, y / 64.0f));
        }

        [HarmonyPostfix]
        [UsedImplicitly]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static void postfixPerformRemoveAction(GameLocation environment, Vector2 tileLocation) {
          if (!Context.IsWorldReady) return;
          GlobalVars.OnObjectRemoved(environment, tileLocation);
        }

        public static void ApplyAll(HarmonyInstance harmony) {
          harmony.Patch(AccessTools.Method(typeof(StardewValley.Object), "placementAction"),
                        postfix: new HarmonyMethod(AccessTools.Method(typeof(SVObject), "postfixPlacementAction")));
          harmony.Patch(AccessTools.Method(typeof(StardewValley.Object), "performRemoveAction"),
                        postfix: new HarmonyMethod(AccessTools.Method(typeof(SVObject), "postfixPerformRemoveAction")));
        }
      }

      public static class AutomateCompatibility {
        private const String CONST_UID     = "Pathoschild.Automate";
        private const String CONST_VERSION = "1.22.1";

        public static void ApplyAll(HarmonyInstance harmony) {
          if (GlobalVars.gSMAPIHelper.ModRegistry.Get(CONST_UID) is not IModInfo automate_mod_info) return;

          GlobalVars.gSMAPIMonitor.Log("Automate is found!", LogLevel.Info);
          GlobalVars.gSMAPIMonitor.Log($"The target version of Automate for the compatibility patch is '{CONST_VERSION}'.", LogLevel.Debug);
          GlobalVars.gIsAutomateLoaded = true;

          if (String.Equals(automate_mod_info.Manifest.Version.ToString(), CONST_VERSION, StringComparison.OrdinalIgnoreCase))
            GlobalVars.gSMAPIMonitor.Log("ChestEx is fully compatible with this Automate version.", LogLevel.Info);
          else if (automate_mod_info.Manifest.Version.IsNewerThan(CONST_VERSION))
            GlobalVars.gSMAPIMonitor
                      .Log("You seem to be running a newer version of Automate! This warning can safely be ignored if you don't experience any issues. However, if you do experience any issues, please report it to me on Discord or on Nexus.",
                           LogLevel.Warn);
          else if (automate_mod_info.Manifest.Version.IsOlderThan(CONST_VERSION))
            GlobalVars.gSMAPIMonitor
                      .Log("You seem to be running an older version of Automate! There is a high chance that you will experience issues, please update your copy of Automate.",
                           LogLevel.Warn);
        }
      }

      public static class ChestsAnywhereCompatibility {
        private const String CONST_UID     = "Pathoschild.ChestsAnywhere";
        private const String CONST_VERSION = "1.20.13";

        [HarmonyPatch]
        private static class ChestContainer {
          [HarmonyPrefix]
          [UsedImplicitly]
          [SuppressMessage("ReSharper", "InconsistentNaming")]
          private static void prefixOpenMenu() {
            if (Game1.activeClickableMenu is CustomItemGrabMenu menu) menu.exitThisMenu(false);
          }

          [HarmonyTranspiler]
          [UsedImplicitly]
          [SuppressMessage("ReSharper", "InconsistentNaming")]
          private static void postfixOpenMenu(Chest ___Chest, Object ___Context, ref IClickableMenu __result) {
            if (___Chest.SpecialChestType != Chest.SpecialChestTypes.None || ___Chest.playerChest is null || !___Chest.playerChest.Value) return;

            __result?.exitThisMenu(false);
            __result = new MainMenu(___Chest.GetItemsForPlayer(Game1.player.UniqueMultiplayerID),
                                    false,
                                    true,
                                    (__result as ItemGrabMenu)?.inventory.highlightMethod,
                                    Traverse.Create(__result).Field<ItemGrabMenu.behaviorOnItemSelect>("behaviorFunction")?.Value,
                                    null,
                                    (__result as ItemGrabMenu)?.behaviorOnItemGrab,
                                    false,
                                    true,
                                    true,
                                    true,
                                    true,
                                    1,
                                    ___Chest,
                                    -1,
                                    ___Context);
          }
        }

        [HarmonyPatch]
        private static class BaseChestOverlay {
          [HarmonyTranspiler]
          [UsedImplicitly]
          [SuppressMessage("ReSharper", "InconsistentNaming")]
          private static IEnumerable<CodeInstruction> transpilerReinitializeComponents(IEnumerable<CodeInstruction> instructions, ILGenerator ilg) {
            var             target_type        = Type.GetType("Pathoschild.Stardew.ChestsAnywhere.Menus.Overlays.BaseChestOverlay, ChestsAnywhere");
            ConstructorInfo xna_rectangle_ctor = AccessTools.Constructor(typeof(Rectangle), new[] { typeof(Int32), typeof(Int32), typeof(Int32), typeof(Int32) });

            Boolean patched  = false;
            Label   lbl_skip = ilg.DefineLabel();

            foreach (CodeInstruction instruction in instructions) {
              if (!patched && instruction.opcode == OpCodes.Call && (MethodBase)instruction.operand == xna_rectangle_ctor) {
                yield return instruction;

                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(target_type, "Menu"));
                yield return new CodeInstruction(OpCodes.Isinst, typeof(MainMenu));
                yield return new CodeInstruction(OpCodes.Brfalse, lbl_skip);

                yield return new CodeInstruction(OpCodes.Ldloca_S, 0);
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(target_type, "Menu"));
                yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CustomItemGrabMenu), "get_mRealBounds"));
                yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Rectangle), "X"));
                yield return new CodeInstruction(OpCodes.Ldc_I4, 32);
                yield return new CodeInstruction(OpCodes.Add);
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(target_type, "Menu"));
                yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CustomItemGrabMenu), "get_mRealBounds"));
                yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Rectangle), "Y"));
                yield return new CodeInstruction(OpCodes.Ldc_I4, 132);
                yield return new CodeInstruction(OpCodes.Add);
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(target_type, "Menu"));
                yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CustomItemGrabMenu), "get_mRealBounds"));
                yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Rectangle), "Width"));
                yield return new CodeInstruction(OpCodes.Ldc_I4, 32);
                yield return new CodeInstruction(OpCodes.Sub);
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(target_type, "Menu"));
                yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CustomItemGrabMenu), "get_mRealBounds"));
                yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Rectangle), "Height"));
                yield return new CodeInstruction(OpCodes.Ldc_I4, 132);
                yield return new CodeInstruction(OpCodes.Sub);
                yield return instruction;

                var jmp = new CodeInstruction(OpCodes.Nop);
                jmp.labels.Add(lbl_skip);

                yield return jmp;

                patched = true;

                continue;
              }

              yield return instruction;
            }

            if (patched) { GlobalVars.gSMAPIMonitor.Log("Successfully patched 'ChestsAnywhere.Menus.Overlays.BaseChestOverlay.ReinitializeComponents'.", LogLevel.Info); }
            else {
              GlobalVars.gSMAPIMonitor.Log("Could not patch 'ChestsAnywhere.Menus.Overlays.BaseChestOverlay.ReinitializeComponents' to move ChestsAnywhere buttons!",
                                           LogLevel.Error);
            }
          }

          [HarmonyTranspiler]
          [UsedImplicitly]
          [SuppressMessage("ReSharper", "InconsistentNaming")]
          private static IEnumerable<CodeInstruction> transpilerDrawUi(IEnumerable<CodeInstruction> instructions, ILGenerator ilg) {
            var             target_type        = Type.GetType("Pathoschild.Stardew.ChestsAnywhere.Menus.Overlays.BaseChestOverlay, ChestsAnywhere");
            ConstructorInfo xna_rectangle_ctor = AccessTools.Constructor(typeof(Rectangle), new[] { typeof(Int32), typeof(Int32), typeof(Int32), typeof(Int32) });

            Boolean patched  = false;
            Label   lbl_skip = ilg.DefineLabel();

            foreach (CodeInstruction instruction in instructions) {
              if (!patched && instruction.opcode == OpCodes.Newobj && (MethodBase)instruction.operand == xna_rectangle_ctor) {
                yield return instruction;

                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(target_type, "Menu"));
                yield return new CodeInstruction(OpCodes.Isinst, typeof(MainMenu));
                yield return new CodeInstruction(OpCodes.Brfalse, lbl_skip);

                yield return new CodeInstruction(OpCodes.Pop);
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(target_type, "Menu"));
                yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CustomItemGrabMenu), "get_mRealBounds"));
                yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Rectangle), "X"));
                yield return new CodeInstruction(OpCodes.Ldc_I4, 48);
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(target_type, "Menu"));
                yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CustomItemGrabMenu), "get_mRealBounds"));
                yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Rectangle), "Width"));
                yield return new CodeInstruction(OpCodes.Ldsflda, AccessTools.Field(typeof(Game1), "uiViewport"));
                yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(xTile.Dimensions.Rectangle), "get_Height"));
                yield return new CodeInstruction(OpCodes.Ldc_I4, 80);
                yield return new CodeInstruction(OpCodes.Sub);
                yield return instruction;

                var jmp = new CodeInstruction(OpCodes.Nop);
                jmp.labels.Add(lbl_skip);

                yield return jmp;

                patched = true;

                continue;
              }

              yield return instruction;
            }

            if (patched)
              GlobalVars.gSMAPIMonitor.Log("Successfully patched 'ChestsAnywhere.Menus.Overlays.BaseChestOverlay.DrawUi'.", LogLevel.Info);
            else { GlobalVars.gSMAPIMonitor.Log("Could not patch 'ChestsAnywhere.Menus.Overlays.BaseChestOverlay.DrawUi' to move ChestsAnywhere edit menu!", LogLevel.Error); }
          }
        }

        [HarmonyPatch]
        private static class ChestOverlay {
          [HarmonyTranspiler]
          [UsedImplicitly]
          [SuppressMessage("ReSharper", "InconsistentNaming")]
          private static IEnumerable<CodeInstruction> transpilerReinitializeComponents(IEnumerable<CodeInstruction> instructions) {
            var        target_type = Type.GetType("Pathoschild.Stardew.ChestsAnywhere.Framework.ModConfig, ChestsAnywhere");
            MethodInfo patch_fn    = AccessTools.Method(target_type, "get_AddOrganizePlayerInventoryButton");

            Boolean found_target = false;
            Boolean patched      = false;

            foreach (CodeInstruction instruction in instructions) {
              if (!found_target)
                found_target = instruction.opcode == OpCodes.Callvirt && (MethodBase)instruction.operand == patch_fn;
              else if (!patched) {
                if (instruction.opcode == OpCodes.Brfalse) {
                  instruction.opcode = OpCodes.Br;
                  patched            = true;
                }
                else if (instruction.opcode == OpCodes.Brfalse_S) {
                  instruction.opcode = OpCodes.Br_S;
                  patched            = true;
                }

                if (patched) yield return new CodeInstruction(OpCodes.Pop);
              }

              yield return instruction;
            }

            if (patched) { GlobalVars.gSMAPIMonitor.Log("Successfully patched 'ChestsAnywhere.Menus.Overlays.ChestOverlay.ReinitializeComponents'.", LogLevel.Info); }
            else {
              GlobalVars.gSMAPIMonitor
                        .Log("Could not patch 'ChestsAnywhere.Menus.Overlays.ChestOverlay.ReinitializeComponents' to make ChestAnywhere skip creating player inventory organize button!",
                             LogLevel.Error);
            }
          }
        }

        [HarmonyPatch]
        private static class ModEntry {
          [HarmonyTranspiler]
          [UsedImplicitly]
          [SuppressMessage("ReSharper", "InconsistentNaming")]
          private static IEnumerable<CodeInstruction> transpilerOnRenderedHud(IEnumerable<CodeInstruction> instructions) {
            var        target_type = Type.GetType("Pathoschild.Stardew.ChestsAnywhere.Framework.ModConfig, ChestsAnywhere");
            MethodInfo patch_fn    = AccessTools.Method(target_type, "get_ShowHoverTooltips");

            Boolean found_target = false;
            Boolean patched      = false;

            foreach (CodeInstruction instruction in instructions) {
              if (!found_target)
                found_target = instruction.opcode == OpCodes.Callvirt && (MethodBase)instruction.operand == patch_fn;
              else if (!patched) {
                if (instruction.opcode == OpCodes.Brfalse) {
                  instruction.opcode = OpCodes.Br;
                  patched            = true;
                }
                else if (instruction.opcode == OpCodes.Brfalse_S) {
                  instruction.opcode = OpCodes.Br_S;
                  patched            = true;
                }

                if (patched) yield return new CodeInstruction(OpCodes.Pop);
              }

              yield return instruction;
            }

            if (patched) { GlobalVars.gSMAPIMonitor.Log("Successfully patched 'ChestsAnywhere.ModEntry.OnRenderedHud'.", LogLevel.Info); }
            else { GlobalVars.gSMAPIMonitor.Log("Could not patch 'ChestsAnywhere.ModEntry.OnRenderedHud' to make ChestAnywhere skip showing its tooltip!", LogLevel.Error); }
          }
        }

        public static void ApplyAll(HarmonyInstance harmony) {
          if (GlobalVars.gSMAPIHelper.ModRegistry.Get(CONST_UID) is not IModInfo chests_anywhere_mod_info) return;
          GlobalVars.gSMAPIMonitor.Log("ChestsAnywhere is found, installing dynamic compatibility patches...", LogLevel.Info);
          GlobalVars.gSMAPIMonitor.Log($"The target version of ChestsAnywhere for the compatibility patch is '{CONST_VERSION}'.", LogLevel.Debug);
          GlobalVars.gIsChestsAnywhereLoaded = true;

          if (String.Equals(chests_anywhere_mod_info.Manifest.Version.ToString(), CONST_VERSION, StringComparison.OrdinalIgnoreCase))
            GlobalVars.gSMAPIMonitor.Log("ChestEx is fully compatible with this ChestsAnywhere version.", LogLevel.Info);
          else if (chests_anywhere_mod_info.Manifest.Version.IsNewerThan(CONST_VERSION))
            GlobalVars.gSMAPIMonitor
                      .Log("You seem to be running a newer version of ChestsAnywhere! This warning can safely be ignored if you don't experience any issues. However, if you do experience any issues, please report it to me on Discord or on Nexus.",
                           LogLevel.Warn);
          else if (chests_anywhere_mod_info.Manifest.Version.IsOlderThan(CONST_VERSION))
            GlobalVars.gSMAPIMonitor
                      .Log("You seem to be running an older version of ChestsAnywhere! There is a high chance that you will experience issues, please update your copy of ChestsAnywhere.",
                           LogLevel.Warn);

          harmony.Patch(AccessTools.Method(Type.GetType("Pathoschild.Stardew.ChestsAnywhere.Framework.Containers.ChestContainer, ChestsAnywhere"), "OpenMenu"),
                        new HarmonyMethod(AccessTools.Method(typeof(ChestContainer), "prefixOpenMenu")),
                        new HarmonyMethod(AccessTools.Method(typeof(ChestContainer), "postfixOpenMenu")));
          harmony.Patch(AccessTools.Method(Type.GetType("Pathoschild.Stardew.ChestsAnywhere.Menus.Overlays.BaseChestOverlay, ChestsAnywhere"), "ReinitializeComponents"),
                        transpiler: new HarmonyMethod(AccessTools.Method(typeof(BaseChestOverlay), "transpilerReinitializeComponents")));
          harmony.Patch(AccessTools.Method(Type.GetType("Pathoschild.Stardew.ChestsAnywhere.Menus.Overlays.BaseChestOverlay, ChestsAnywhere"), "DrawUi"),
                        transpiler: new HarmonyMethod(AccessTools.Method(typeof(BaseChestOverlay), "transpilerDrawUi")));
          harmony.Patch(AccessTools.Method(Type.GetType("Pathoschild.Stardew.ChestsAnywhere.Menus.Overlays.ChestOverlay, ChestsAnywhere"), "ReinitializeComponents"),
                        transpiler: new HarmonyMethod(AccessTools.Method(typeof(ChestOverlay), "transpilerReinitializeComponents")));
          harmony.Patch(AccessTools.Method(Type.GetType("Pathoschild.Stardew.ChestsAnywhere.ModEntry, ChestsAnywhere"), "OnRenderedHud"),
                        transpiler: new HarmonyMethod(AccessTools.Method(typeof(ModEntry), "transpilerOnRenderedHud")));
        }
      }
    }

    public override void Entry(IModHelper helper) {
      GlobalVars.gSMAPIHelper  = helper;
      GlobalVars.gSMAPIMonitor = this.Monitor;

      // Config
      {
        Config.Load();
        helper.Events.Multiplayer.PeerContextReceived += Config.PlayerConnecting;
        helper.Events.Multiplayer.ModMessageReceived  += Config.MPDataReceived;
      }

      // ExtendenChest Draw Event
      helper.Events.Display.RenderingHud += ExtendedChest.OnRenderingHud;

      var harmony = HarmonyInstance.Create("mod.berkayylmao.ChestEx");
      Patches.SVItemGrabMenu.ApplyAll(harmony);
      Patches.SVChest.ApplyAll(harmony);
      Patches.SVObject.ApplyAll(harmony);
      Patches.AutomateCompatibility.ApplyAll(harmony);
      Patches.ChestsAnywhereCompatibility.ApplyAll(harmony);
    }
  }
}
