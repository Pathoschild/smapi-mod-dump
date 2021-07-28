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

using Harmony;

using JetBrains.Annotations;

using Microsoft.Xna.Framework;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

using Object = System.Object;

namespace ChestEx.CompatibilityPatches {
  internal class ConvenientChests : CompatibilityPatch {
  #region Patches

    [HarmonyPatch]
    private static class CategoryMenu {
      private static Type sType => Type.GetType("ConvenientChests.CategorizeChests.Interface.Widgets.CategoryMenu, ConvenientChests");

      [HarmonyPrefix]
      [UsedImplicitly]
      [SuppressMessage("ReSharper", "InconsistentNaming")]
      private static void prefixRecreateItemToggles(Object __instance) {
        if (Game1.activeClickableMenu is not CustomItemGrabMenu menu) return;

        Int32 width = menu.mSourceInventoryOptions.mDialogueBoxBounds.Width;
        Traverse.Create(__instance).Property<Int32>("Width").Value = width;
        Traverse.Create(__instance).Property("ToggleBag").Property<Int32>("Width").Value =
          width - Traverse.Create(__instance).Property("ScrollBar").Property<Int32>("Width").Value - 16;
      }

      [HarmonyPostfix]
      [UsedImplicitly]
      [SuppressMessage("ReSharper", "InconsistentNaming")]
      private static void postfixPositionElements(Object __instance) {
        if (Game1.activeClickableMenu is not CustomItemGrabMenu menu) return;

        Int32 height = menu.mPlayerInventoryOptions.mDialogueBoxBounds.Bottom - menu.mSourceInventoryOptions.mDialogueBoxBounds.Y;
        Traverse.Create(__instance).Property<Int32>("Height").Value                        = height;
        Traverse.Create(__instance).Property("Background").Property<Int32>("Height").Value = height;
        Traverse.Create(__instance).Property("ScrollBar").Property<Int32>("Height").Value =
          height - Traverse.Create(__instance).Property("CloseButton").Property<Int32>("Height").Value - 16;
      }

      public static void Install() {
        GlobalVars.gHarmony.PatchEx(AccessTools.Method(sType, "RecreateItemToggles"),
                                    new HarmonyMethod(AccessTools.Method(typeof(CategoryMenu), "prefixRecreateItemToggles")),
                                    reason: "move ConvenientChests' UI");
        GlobalVars.gHarmony.PatchEx(AccessTools.Method(sType, "PositionElements"),
                                    postfix: new HarmonyMethod(AccessTools.Method(typeof(CategoryMenu), "postfixPositionElements")),
                                    reason: "move ConvenientChests' UI");
      }
    }

    [HarmonyPatch]
    private static class ChestOverlay {
      private static Type sType => Type.GetType("ConvenientChests.CategorizeChests.Interface.Widgets.ChestOverlay, ConvenientChests");

      [HarmonyPostfix]
      [UsedImplicitly]
      [SuppressMessage("ReSharper", "InconsistentNaming")]
      private static void postfixPositionButtons(Object __instance) {
        if (Game1.activeClickableMenu is not CustomItemGrabMenu menu) return;

        var openbtn_pos       = Traverse.Create(__instance).Property("OpenButton").Property<Point>("Position");
        var openbtn_width     = Traverse.Create(__instance).Property("OpenButton").Property<Int32>("Width");
        var openbtn_height    = Traverse.Create(__instance).Property("OpenButton").Property<Int32>("Height");
        var openbtn_lpadding  = Traverse.Create(__instance).Property("OpenButton").Property<Int32>("LeftPadding");
        var stashbtn_pos      = Traverse.Create(__instance).Property("StashButton").Property<Point>("Position");
        var stashbtn_width    = Traverse.Create(__instance).Property("StashButton").Property<Int32>("Width");
        var stashbtn_height   = Traverse.Create(__instance).Property("StashButton").Property<Int32>("Height");
        var stashbtn_lpadding = Traverse.Create(__instance).Property("StashButton").Property<Int32>("LeftPadding");

        Rectangle menu_rect = menu.mSourceInventoryOptions.mDialogueBoxBounds;
        stashbtn_pos.Value = new Point(menu_rect.X - 16 - stashbtn_width.Value + stashbtn_lpadding.Value - 4, menu_rect.Bottom - stashbtn_height.Value - 32);
        openbtn_pos.Value  = new Point(menu_rect.X - 16 - openbtn_width.Value + openbtn_lpadding.Value - 4, stashbtn_pos.Value.Y - openbtn_height.Value);
      }

      [HarmonyPostfix]
      [UsedImplicitly]
      [SuppressMessage("ReSharper", "InconsistentNaming")]
      private static void postfixOpenCategoryMenu(Object __instance) {
        if (Game1.activeClickableMenu is not CustomItemGrabMenu menu) return;

        Traverse.Create(__instance).Property("CategoryMenu").Property<Point>("Position").Value =
          new Point(menu.mSourceInventoryOptions.mDialogueBoxBounds.X, menu.mSourceInventoryOptions.mDialogueBoxBounds.Y);
      }

      public static void Install() {
        GlobalVars.gHarmony.PatchEx(AccessTools.Method(sType, "PositionButtons"),
                                    postfix: new HarmonyMethod(AccessTools.Method(typeof(ChestOverlay), "postfixPositionButtons")),
                                    reason: "move ConvenientChests' buttons");
        GlobalVars.gHarmony.PatchEx(AccessTools.Method(sType, "OpenCategoryMenu"),
                                    postfix: new HarmonyMethod(AccessTools.Method(typeof(ChestOverlay), "postfixOpenCategoryMenu")),
                                    reason: "move ConvenientChests' UI");
      }
    }

    [HarmonyPatch]
    private static class WidgetHost {
      private static Type sType => Type.GetType("ConvenientChests.CategorizeChests.Interface.WidgetHost, ConvenientChests");

      [HarmonyPrefix]
      [UsedImplicitly]
      [SuppressMessage("ReSharper", "InconsistentNaming")]
      private static void prefixReceiveLeftClick(ref Int32 x, ref Int32 y) {
        if (Game1.activeClickableMenu is not CustomItemGrabMenu menu) return;

        Vector2 pos = Utility.ModifyCoordinatesForUIScale(GlobalVars.gSMAPIHelper.Input.GetCursorPosition().ScreenPixels);
        x = (Int32)pos.X;
        y = (Int32)pos.Y;
      }

      [HarmonyTranspiler]
      [UsedImplicitly]
      [SuppressMessage("ReSharper", "InconsistentNaming")]
      private static IEnumerable<CodeInstruction> transpilerCtor(IEnumerable<CodeInstruction> instructions) {
        Boolean patched = false;

        foreach (CodeInstruction instruction in instructions) {
          if (instruction.opcode == OpCodes.Ldsflda && (FieldInfo)instruction.operand == AccessTools.Field(typeof(Game1), "viewport")) {
            instruction.operand = AccessTools.Field(typeof(Game1), "uiViewport");
            patched             = true;
          }

          yield return instruction;
        }
        patched.ReportTranspilerStatus();
      }

      public static void Install() {
        GlobalVars.gHarmony.PatchEx(AccessTools.Method(sType, "ReceiveLeftClick"),
                                    new HarmonyMethod(AccessTools.Method(typeof(WidgetHost), "prefixReceiveLeftClick")),
                                    reason: "fix ConvenientChests' click handler");
        GlobalVars.gHarmony.PatchEx(AccessTools.Constructor(sType, new[] { typeof(IModEvents), typeof(IInputHelper) }),
                                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(WidgetHost), "transpilerCtor")),
                                    reason: "fix ConvenientChests' click handler");
      }
    }

  #endregion

    // Protected:
  #region Protected

    protected override void InstallPatches() {
      CategoryMenu.Install();
      ChestOverlay.Install();
      WidgetHost.Install();
    }

    protected override void OnLoaded() {
      GlobalVars.gIsConvenientChestsLoaded = true;

      base.OnLoaded();
    }

  #endregion

    // Constructors:
  #region Constructors

    internal ConvenientChests()
      : base("aEnigma.ConvenientChests", new SemanticVersion("1.5.2-unofficial.2-borthain")) { }

  #endregion
  }
}
