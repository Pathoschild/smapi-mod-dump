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
//    Copyright (c) 2022 Berkay Yigit <berkaytgy@gmail.com>
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

using HarmonyLib;

using JetBrains.Annotations;

using StardewValley;
using StardewValley.Menus;

using Object = System.Object;

namespace ChestEx.Patches {
  [HarmonyPatch(typeof(ItemGrabMenu))]
  public static class SVItemGrabMenu {
    [HarmonyTranspiler]
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private static IEnumerable<CodeInstruction> transpilerCtor(IEnumerable<CodeInstruction> instructions, ILGenerator ilg) {
      ConstructorInfo base_ctor = AccessTools.Constructor(typeof(MenuWithInventory),
                                                          new[] {
                                                            typeof(InventoryMenu.highlightThisItem), typeof(Boolean), typeof(Boolean), typeof(Int32), typeof(Int32), typeof(Int32)
                                                          });
      Boolean patched = false;
      Label lbl_skip = ilg.DefineLabel();

      foreach (CodeInstruction instruction in instructions) {
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
      }
      patched.ReportTranspilerStatus();
    }

    public static void Install() {
      GlobalVars.gHarmony.PatchEx(AccessTools.Constructor(typeof(ItemGrabMenu), new[] { typeof(IList<Item>), typeof(Object) }),
                                  transpiler: new HarmonyMethod(AccessTools.Method(typeof(SVItemGrabMenu), "transpilerCtor")),
                                  reason: "force to use ChestEx's menu");
    }
  }
}
