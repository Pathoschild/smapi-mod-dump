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

using ChestEx.LanguageExtensions;
using ChestEx.Types.BaseTypes;

using Harmony;

using JetBrains.Annotations;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

using Object = System.Object;

namespace ChestEx.CompatibilityPatches {
  // TODO: fix the button being behind the dark background
  internal class RemoteFridgeStorage : CompatibilityPatch {
  #region Patches

    [HarmonyPatch]
    private static class ChestController {
      private static Type sType => Type.GetType("RemoteFridgeStorage.controller.ChestController, RemoteFridgeStorage");

      [HarmonyPostfix]
      [UsedImplicitly]
      [SuppressMessage("ReSharper", "InconsistentNaming")]
      private static void postfixUpdatePos(ref Object ____config) {
        if (Game1.activeClickableMenu is not CustomItemGrabMenu menu) return;

        Traverse.Create(____config).Property<Boolean>("OverrideOffset").Value = true;
        Traverse.Create(____config).Property<Int32>("XOffset").Value          = menu.mSourceInventoryOptions.mBounds.X - 48;
        Traverse.Create(____config).Property<Int32>("YOffset").Value          = menu.mSourceInventoryOptions.mBounds.Y + 96;
      }

      [HarmonyTranspiler]
      [UsedImplicitly]
      [SuppressMessage("ReSharper", "InconsistentNaming")]
      private static IEnumerable<CodeInstruction> transpilerHandleClick(IEnumerable<CodeInstruction> instructions) {
        Boolean patched = false;

        foreach (CodeInstruction instruction in instructions) {
          if (!patched && instruction.opcode == OpCodes.Callvirt && (MethodInfo)instruction.operand == AccessTools.Method(typeof(ICursorPosition), "get_ScreenPixels")) {
            yield return instruction;
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Utility), "ModifyCoordinatesForUIScale"));

            patched = true;
            continue;
          }

          yield return instruction;
        }
        patched.ReportTranspilerStatus();
      }

      public static void Install() {
        GlobalVars.gHarmony.PatchEx(AccessTools.Method(sType, "HandleClick"),
                                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(ChestController), "transpilerHandleClick")),
                                    reason: "fix RemoteFridgeStorage's click handler");
        GlobalVars.gHarmony.PatchEx(AccessTools.Method(sType, "UpdatePos"),
                                    postfix: new HarmonyMethod(AccessTools.Method(typeof(ChestController), "postfixUpdatePos")),
                                    reason: "move RemoteFridgeStorage's button");
      }
    }

    [HarmonyPatch]
    private static class ModEntry {
      private static Type sType => Type.GetType("RemoteFridgeStorage.ModEntry, RemoteFridgeStorage");

      [HarmonyTranspiler]
      [UsedImplicitly]
      [SuppressMessage("ReSharper", "InconsistentNaming")]
      private static IEnumerable<CodeInstruction> transpilerEntry(IEnumerable<CodeInstruction> instructions) {
        Boolean patched = false;

        foreach (CodeInstruction instruction in instructions) {
          if (!patched && instruction.opcode == OpCodes.Callvirt && (MethodInfo)instruction.operand == AccessTools.Method(typeof(IDisplayEvents), "add_RenderingActiveMenu")) {
            instruction.operand = AccessTools.Method(typeof(IDisplayEvents), "add_RenderedActiveMenu");
            patched             = true;
          }

          yield return instruction;
        }
        patched.ReportTranspilerStatus();
      }

      public static void Install() {
        GlobalVars.gHarmony.PatchEx(AccessTools.Method(sType, "Entry"),
                                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(ModEntry), "transpilerEntry")),
                                    reason: "fix RemoteFridgeStorage's button rendering");
      }
    }

  #endregion

    // Protected:
  #region Protected

    protected override void InstallPatches() {
      ChestController.Install();
      ModEntry.Install();
    }

    protected override void OnLoaded() {
      GlobalVars.gIsRemoteFridgeStorageLoaded = true;

      base.OnLoaded();
    }

  #endregion

    // Constructors:
  #region Constructors

    internal RemoteFridgeStorage()
      : base("EternalSoap.RemoteFridgeStorage", new SemanticVersion("1.8.1")) { }

  #endregion
  }
}
