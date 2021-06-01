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
//    ShipAnything (StardewValleyMods)
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

using Harmony;

using JetBrains.Annotations;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

using StardewValley;
using StardewValley.BellsAndWhistles;

namespace ShipAnything {
  [UsedImplicitly]
  public class Main : Mod {
    public static class Extensions {
      public static class SVObject {
        [HarmonyPrefix]
        [UsedImplicitly]
        // ReSharper disable once InconsistentNaming
        public static Boolean prefix_canBeShipped(StardewValley.Object __instance, ref Boolean __result) {
          if (__instance.Type != null && !__instance.Type.Equals("Quest"))
            __result = true;
          else
            __result = false;
          return false;
        }
      }

      public static class SVUtility {
        [HarmonyPrefix]
        [UsedImplicitly]
        // ReSharper disable once InconsistentNaming
        public static Boolean prefix_highlightShippableObjects(ref Boolean __result) {
          __result = true;
          return false;
        }
      }

      public static class SVShippingMenu {
        private sealed class SVObjectEx : StardewValley.Object {
          private delegate void DrawInMenuDelegate(SpriteBatch spriteBatch, Vector2       location,        Single scaleSize, Single  transparency,
                                                   Single      layerDepth,  StackDrawType drawStackNumber, Color  color,     Boolean drawShadow);

          private readonly DrawInMenuDelegate orgCall;
          public SVObjectEx(Item item) {
            this.orgCall = item.drawInMenu;

            this.Category           = item.Category;
            this.DisplayName        = item.DisplayName;
            this.HasBeenInInventory = item.HasBeenInInventory;
            this.Name               = item.Name;
            this.ParentSheetIndex   = item.ParentSheetIndex;
            this.Price              = Math.Max(1, item.salePrice());
            this.specialItem        = item.specialItem;
            this.SpecialVariable    = item.SpecialVariable;
          }

          public override void drawInMenu(SpriteBatch spriteBatch, Vector2       location,        Single scaleSize, Single  transparency,
                                          Single      layerDepth,  StackDrawType drawStackNumber, Color  color,     Boolean drawShadow) {
            this.orgCall(spriteBatch,
                         location,
                         scaleSize,
                         transparency,
                         layerDepth,
                         drawStackNumber,
                         color,
                         drawShadow);
          }
        }

        [HarmonyPostfix]
        [UsedImplicitly]
        // ReSharper disable once InconsistentNaming
        public static void postfix_parseItems(IList<Item> items, ref List<List<Item>> ___categoryItems, ref List<Int32> ___categoryTotals, ref List<MoneyDial> ___categoryDials,
                                              ref Dictionary<Item, Int32> ___itemValues) {
          Int32 gained_money = 0;
          foreach (Item item in items) {
            if (item is StardewValley.Object) continue;
            var obj_ex = new SVObjectEx(item);

            ___categoryItems[4].Add(obj_ex); // Add item to misc.
            ___categoryItems[5].Add(obj_ex); // Add item to total
            ___categoryTotals[4]                    += obj_ex.Price;
            ___categoryTotals[5]                    += obj_ex.Price;
            ___itemValues[obj_ex]                   =  obj_ex.Price;
            ___categoryDials[4].previousTargetValue =  ___categoryDials[4].currentValue = ___categoryTotals[4];
            gained_money                            += obj_ex.Price;
            Game1.stats.itemsShipped                += 1;
          }
          ___categoryDials[5].currentValue = ___categoryTotals[5];
          if (Game1.IsMasterGame) Game1.player.Money += gained_money;
          Game1.setRichPresence("earnings", ___categoryTotals[5]);
        }
      }
    }

    public override void Entry(IModHelper helper) {
      var harmony = HarmonyInstance.Create("mod.berkayylmao.ShipAnything");
      harmony.Patch(AccessTools.Method(typeof(StardewValley.Object), "canBeShipped"), new HarmonyMethod(AccessTools.Method(typeof(Extensions.SVObject), "prefix_canBeShipped")));
      harmony.Patch(AccessTools.Method(typeof(Utility), "highlightShippableObjects"),
                    new HarmonyMethod(AccessTools.Method(typeof(Extensions.SVUtility), "prefix_highlightShippableObjects")));
      harmony.Patch(AccessTools.Method(typeof(StardewValley.Menus.ShippingMenu), "parseItems"),
                    postfix: new HarmonyMethod(AccessTools.Method(typeof(Extensions.SVShippingMenu), "postfix_parseItems")));
    }
  }
}
