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
//    but WITHOUT ANY WARRANTY, without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU Affero General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program. If not, see <https://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;

using ChestEx.LanguageExtensions;
using ChestEx.Types.BaseTypes;
using ChestEx.Types.CustomTypes.ChestExMenu;

using Harmony;

using JetBrains.Annotations;

using Microsoft.Xna.Framework;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

using Object = System.Object;

namespace ChestEx.CompatibilityPatches {
  internal class ChestsAnywhere : CompatibilityPatch {
  #region Patches

    [HarmonyPatch]
    private static class BaseChestOverlay {
      private static Type sType => Type.GetType("Pathoschild.Stardew.ChestsAnywhere.Menus.Overlays.BaseChestOverlay, ChestsAnywhere");

      [HarmonyTranspiler]
      [UsedImplicitly]
      [SuppressMessage("ReSharper", "InconsistentNaming")]
      private static IEnumerable<CodeInstruction> transpilerDrawUi(IEnumerable<CodeInstruction> instructions, ILGenerator ilg) {
        ConstructorInfo xna_rectangle_ctor = AccessTools.Constructor(typeof(Rectangle), new[] { typeof(Int32), typeof(Int32), typeof(Int32), typeof(Int32) });

        Boolean patched  = false;
        Label   lbl_skip = ilg.DefineLabel();

        foreach (CodeInstruction instruction in instructions) {
          if (!patched && instruction.opcode == OpCodes.Newobj && (MethodBase)instruction.operand == xna_rectangle_ctor) {
            yield return instruction;

            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(sType, "Menu"));
            yield return new CodeInstruction(OpCodes.Isinst, typeof(CustomItemGrabMenu));
            yield return new CodeInstruction(OpCodes.Brfalse, lbl_skip);

            yield return new CodeInstruction(OpCodes.Pop);
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(sType, "Menu"));
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CustomItemGrabMenu), "get_mRealBounds"));
            yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Rectangle), "X"));
            yield return new CodeInstruction(OpCodes.Ldc_I4, 8);
            yield return new CodeInstruction(OpCodes.Add);
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(sType, "Menu"));
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CustomItemGrabMenu), "get_mRealBounds"));
            yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Rectangle), "Y"));
            yield return new CodeInstruction(OpCodes.Ldc_I4, 20);
            yield return new CodeInstruction(OpCodes.Add);
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(sType, "Menu"));
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CustomItemGrabMenu), "get_mRealBounds"));
            yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Rectangle), "Width"));
            yield return new CodeInstruction(OpCodes.Ldc_I4, 8);
            yield return new CodeInstruction(OpCodes.Sub);
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(sType, "Menu"));
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CustomItemGrabMenu), "get_mRealBounds"));
            yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Rectangle), "Height"));
            yield return new CodeInstruction(OpCodes.Ldc_I4, 20);
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
        patched.ReportTranspilerStatus();
      }

      [HarmonyTranspiler]
      [UsedImplicitly]
      [SuppressMessage("ReSharper", "InconsistentNaming")]
      private static IEnumerable<CodeInstruction> transpilerReinitializeComponents(IEnumerable<CodeInstruction> instructions, ILGenerator ilg) {
        ConstructorInfo xna_rectangle_ctor = AccessTools.Constructor(typeof(Rectangle), new[] { typeof(Int32), typeof(Int32), typeof(Int32), typeof(Int32) });

        Boolean patched  = false;
        Label   lbl_skip = ilg.DefineLabel();

        foreach (CodeInstruction instruction in instructions) {
          if (!patched && instruction.opcode == OpCodes.Call && (MethodBase)instruction.operand == xna_rectangle_ctor) {
            yield return instruction;

            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(sType, "Menu"));
            yield return new CodeInstruction(OpCodes.Isinst, typeof(CustomItemGrabMenu));
            yield return new CodeInstruction(OpCodes.Brfalse, lbl_skip);

            yield return new CodeInstruction(OpCodes.Ldloca_S, 0);
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(sType, "Menu"));
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CustomItemGrabMenu), "get_mRealBounds"));
            yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Rectangle), "X"));
            yield return new CodeInstruction(OpCodes.Ldc_I4, 32);
            yield return new CodeInstruction(OpCodes.Add);
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(sType, "Menu"));
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CustomItemGrabMenu), "get_mRealBounds"));
            yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Rectangle), "Y"));
            yield return new CodeInstruction(OpCodes.Ldc_I4, 64);
            yield return new CodeInstruction(OpCodes.Add);
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(sType, "Menu"));
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CustomItemGrabMenu), "get_mRealBounds"));
            yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Rectangle), "Width"));
            yield return new CodeInstruction(OpCodes.Ldc_I4, 32);
            yield return new CodeInstruction(OpCodes.Sub);
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(sType, "Menu"));
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
        patched.ReportTranspilerStatus();
      }

      public static void Install() {
        GlobalVars.gHarmony.PatchEx(AccessTools.Method(sType, "ReinitializeComponents"),
                                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(BaseChestOverlay), "transpilerReinitializeComponents")),
                                    reason: "move ChestAnywhere buttons");
        GlobalVars.gHarmony.PatchEx(AccessTools.Method(sType, "DrawUi"),
                                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(BaseChestOverlay), "transpilerDrawUi")),
                                    reason: "move ChestAnywhere UI");
      }
    }

    [HarmonyPatch]
    private static class ChestOverlay {
      private static Type sType => Type.GetType("Pathoschild.Stardew.ChestsAnywhere.Menus.Overlays.ChestOverlay, ChestsAnywhere");

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
        patched.ReportTranspilerStatus();
      }

      public static void Install() {
        GlobalVars.gHarmony.PatchEx(AccessTools.Method(sType, "ReinitializeComponents"),
                                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(ChestOverlay), "transpilerReinitializeComponents")),
                                    reason: "block ChestAnywhere's organize button");
      }
    }

    [HarmonyPatch]
    private static class ChestContainer {
      private static Type sType => Type.GetType("Pathoschild.Stardew.ChestsAnywhere.Framework.Containers.ChestContainer, ChestsAnywhere");

      [HarmonyPostfix]
      [UsedImplicitly]
      [SuppressMessage("ReSharper", "InconsistentNaming")]
      private static Boolean prefixOpenMenu(Object __instance, Chest ___Chest, Object ___Context, ref IClickableMenu __result) {
        if (Game1.activeClickableMenu is CustomItemGrabMenu menu) menu.exitThisMenu(false);
        if (___Chest.SpecialChestType != Chest.SpecialChestTypes.None || ___Chest.playerChest is null || !___Chest.playerChest.Value) return true;

        __result = new MainMenu(Traverse.Create(__instance).Property<IList<Item>>("Inventory").Value,
                                false,
                                true,
                                (InventoryMenu.highlightThisItem)AccessTools.Method(sType, "CanAcceptItem", new[] { typeof(Item) })
                                                                            .CreateDelegate(typeof(InventoryMenu.highlightThisItem), __instance),
                                (ItemGrabMenu.behaviorOnItemSelect)AccessTools.Method(sType, "GrabItemFromPlayer", new[] { typeof(Item), typeof(Farmer) })
                                                                              .CreateDelegate(typeof(ItemGrabMenu.behaviorOnItemSelect), __instance),
                                null,
                                (ItemGrabMenu.behaviorOnItemSelect)AccessTools.Method(sType, "GrabItemFromContainer", new[] { typeof(Item), typeof(Farmer) })
                                                                              .CreateDelegate(typeof(ItemGrabMenu.behaviorOnItemSelect), __instance),
                                false,
                                true,
                                true,
                                true,
                                true,
                                1,
                                ___Chest,
                                -1,
                                ___Context);
        return false;
      }

      public static void Install() {
        GlobalVars.gHarmony.PatchEx(AccessTools.Method(sType, "OpenMenu"),
                                    new HarmonyMethod(AccessTools.Method(typeof(ChestContainer), "prefixOpenMenu")),
                                    reason: "force to use ChestEx's menu");
      }
    }

    [HarmonyPatch]
    private static class ModEntry {
      private static Type sType => Type.GetType("Pathoschild.Stardew.ChestsAnywhere.ModEntry, ChestsAnywhere");

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
        patched.ReportTranspilerStatus();
      }

      public static void Install() {
        GlobalVars.gHarmony.PatchEx(AccessTools.Method(sType, "OnRenderedHud"),
                                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(ModEntry), "transpilerOnRenderedHud")),
                                    reason: "block ChestAnywhere's chest tooltip");
      }
    }

  #endregion

    // Protected:
  #region Protected

    protected override void InstallPatches() {
      BaseChestOverlay.Install();
      ChestOverlay.Install();
      ChestContainer.Install();
      ModEntry.Install();
    }

    protected override void OnLoaded() {
      GlobalVars.gIsChestsAnywhereLoaded = true;

      base.OnLoaded();
    }

  #endregion

    // Constructors:
  #region Constructors

    internal ChestsAnywhere()
      : base("Pathoschild.ChestsAnywhere", new SemanticVersion("1.20.14")) { }

  #endregion
  }
}
