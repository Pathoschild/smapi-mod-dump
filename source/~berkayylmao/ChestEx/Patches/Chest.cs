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
using System.Diagnostics.CodeAnalysis;

using ChestEx.LanguageExtensions;
using ChestEx.Types.BaseTypes;
using ChestEx.Types.CustomTypes.ChestExMenu;
using ChestEx.Types.CustomTypes.ExtendedSVObjects;

using HarmonyLib;

using JetBrains.Annotations;

using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace ChestEx.Patches {
  [HarmonyPatch(typeof(Chest))]
  public static class SVChest {
    [HarmonyPostfix]
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private static void postfixGetActualCapacity(Chest __instance, ref Int32 __result) {
      if (__instance.SpecialChestType != Chest.SpecialChestTypes.None || __instance.playerChest is null || !__instance.playerChest.Value) return;
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

    public static void Install() {
      GlobalVars.gHarmony.PatchEx(AccessTools.Method(typeof(Chest), "ShowMenu"),
                                  new HarmonyMethod(AccessTools.Method(typeof(SVChest), "prefixShowMenu")),
                                  reason: "force to use ChestEx's menu");
      GlobalVars.gHarmony.PatchEx(AccessTools.Method(typeof(Chest), "GetActualCapacity"),
                                  postfix: new HarmonyMethod(AccessTools.Method(typeof(SVChest), "postfixGetActualCapacity")),
                                  reason: "edit chest's capacity");
      GlobalVars.gHarmony.PatchEx(AccessTools.Method(typeof(Chest), "draw", new[] { typeof(SpriteBatch), typeof(Int32), typeof(Int32), typeof(Single) }),
                                  new HarmonyMethod(typeof(ExtendedChest).GetMethod("BeforeDraw")),
                                  reason: "allow custom hinge colours");
    }
  }
}
