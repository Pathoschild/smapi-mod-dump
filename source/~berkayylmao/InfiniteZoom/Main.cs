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
//    InfiniteZoom (StardewValleyMods)
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
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using Harmony;

using JetBrains.Annotations;

using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Menus;

namespace InfiniteZoom {
  [UsedImplicitly]
  public class Main : Mod {
    private const Int32  CONST_MIN_VALUE     = 25;
    private const Int32  CONST_MAX_VALUE     = 250;
    private const Single CONST_MIN_VALUE_FLT = 0.25f;
    private const Single CONST_MAX_VALUE_FLT = 2.5f;

    private static readonly List<String> sZoomLevels = new();

    [HarmonyPatch]
    private static class DayTimeMoneyBoxPatches {
      [HarmonyTranspiler]
      [UsedImplicitly]
      [SuppressMessage("ReSharper", "InconsistentNaming")]
      private static IEnumerable<CodeInstruction> transpilerDraw(IEnumerable<CodeInstruction> instructions) {
        Boolean editNext = false;

        foreach (CodeInstruction instruction in instructions) {
          if (editNext && instruction.opcode == OpCodes.Ldc_R4) {
            if ((Single)instruction.operand > 1.5f) // max
              instruction.operand = CONST_MAX_VALUE_FLT;
            else if ((Single)instruction.operand < 1.0f) // min
              instruction.operand = CONST_MIN_VALUE_FLT;
            editNext = false;
          }

          if (instruction.opcode == OpCodes.Callvirt && (MethodBase)instruction.operand == AccessTools.Method(typeof(Options), "get_desiredBaseZoomLevel")) editNext = true;

          yield return instruction;
        }
      }

      [HarmonyPostfix]
      [UsedImplicitly]
      [SuppressMessage("ReSharper", "InconsistentNaming")]
      private static void postfixReceiveLeftClick(DayTimeMoneyBox __instance, Int32 x, Int32 y) {
        if (!Game1.options.zoomButtons) return;

        if (__instance.zoomInButton.containsPoint(x, y) && Game1.options.desiredBaseZoomLevel < CONST_MAX_VALUE_FLT) {
          Int32 zoom = (Int32)Math.Round(Game1.options.desiredBaseZoomLevel * 100f);
          zoom                                -= zoom % 5;
          Game1.options.desiredBaseZoomLevel  =  Math.Min(CONST_MAX_VALUE, zoom + 5) / 100f;
          Game1.forceSnapOnNextViewportUpdate =  true;
          Game1.playSound("drumkit6");
        }
        else if (__instance.zoomOutButton.containsPoint(x, y) && Game1.options.desiredBaseZoomLevel > CONST_MIN_VALUE_FLT) {
          Int32 zoom = (Int32)Math.Round(Game1.options.desiredBaseZoomLevel * 100f);
          zoom                                -= zoom % 5;
          Game1.options.desiredBaseZoomLevel  =  Math.Max(CONST_MIN_VALUE, zoom - 5) / 100f;
          Game1.forceSnapOnNextViewportUpdate =  true;
          Program.gamePtr.refreshWindowSettings();
          Game1.playSound("drumkit6");
        }
      }
    }

    [HarmonyPatch]
    private static class OptionsPagePatches {
      [HarmonyPostfix]
      [UsedImplicitly]
      [SuppressMessage("ReSharper", "InconsistentNaming")]
      private static void postfixCtor(OptionsPage __instance) {
        var zoom     = (OptionsPlusMinus)__instance.options.First(o => o.whichOption == Options.zoom);
        var ui_scale = (OptionsPlusMinus)__instance.options.First(o => o.whichOption == Options.uiScaleSlider);

        zoom.options     = zoom.displayOptions     = sZoomLevels;
        ui_scale.options = ui_scale.displayOptions = sZoomLevels;
        Game1.options.setPlusMinusToProperValue(zoom);
        Game1.options.setPlusMinusToProperValue(ui_scale);
      }
    }

    public override void Entry(IModHelper helper) {
      var harmony = HarmonyInstance.Create("mod.berkayylmao.InfiniteZoom");
      for (Int32 i = CONST_MIN_VALUE; i <= CONST_MAX_VALUE; i += 5) sZoomLevels.Add($"{i}%");

      harmony.Patch(AccessTools.Method(typeof(DayTimeMoneyBox), "draw", new[] { typeof(SpriteBatch) }),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(DayTimeMoneyBoxPatches), "transpilerDraw")));
      harmony.Patch(AccessTools.Method(typeof(DayTimeMoneyBox), "receiveLeftClick"),
                    postfix: new HarmonyMethod(AccessTools.Method(typeof(DayTimeMoneyBoxPatches), "postfixReceiveLeftClick")));
      harmony.Patch(AccessTools.Constructor(typeof(OptionsPage), new[] { typeof(Int32), typeof(Int32), typeof(Int32), typeof(Int32) }),
                    postfix: new HarmonyMethod(AccessTools.Method(typeof(OptionsPagePatches), "postfixCtor")));
    }
  }
}
