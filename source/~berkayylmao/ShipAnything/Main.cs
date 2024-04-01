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
//    Copyright (c) 2024 Berkay Yigit <mail@berkay.link>
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
using System.Xml.Serialization;

using HarmonyLib;

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
      private sealed class SVObjectEx : StardewValley.Object {
        private delegate void DrawInMenuDelegate(SpriteBatch spriteBatch, Vector2 location, Single scaleSize, Single transparency,
                                                 Single layerDepth, StackDrawType drawStackNumber, Color color, Boolean drawShadow);

        private readonly DrawInMenuDelegate orgCall;

        [XmlIgnore]
        public override String DisplayName {
          get => this.displayName;
        }

        public override Int32 salePrice(Boolean ignoreProfitMargins = false) {
          Single salePrice = this.Price;
          if (!ignoreProfitMargins && this.appliesProfitMargins())
            salePrice = Math.Max(1.0f, salePrice * Game1.MasterPlayer.difficultyModifier);

          return Convert.ToInt32(salePrice);
        }
        public override Int32 sellToStorePrice(Int64 specificPlayerID = -1L) {
          Single salePrice = this.Price;
          salePrice = this.getPriceAfterMultipliers(salePrice, specificPlayerID);
          if (salePrice > 0.0f)
            salePrice = Math.Max(1.0f, salePrice * Game1.MasterPlayer.difficultyModifier);

          return Convert.ToInt32(salePrice);
        }

        public SVObjectEx(Item item) {
          this.orgCall = item.drawInMenu;

          this.Category = 4; // Misc. category
          this.displayName = item.DisplayName;
          this.HasBeenInInventory = item.HasBeenInInventory;
          this.ItemId = item.ItemId;
          this.Name = item.Name;
          this.ParentSheetIndex = item.ParentSheetIndex;
          this.Price = Math.Max(1, item.salePrice());
          this.specialItem = item.specialItem;
          this.SpecialVariable = item.SpecialVariable;
          this.Stack = Math.Max(0, item.Stack);
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, Single scaleSize, Single transparency,
                                        Single layerDepth, StackDrawType drawStackNumber, Color color, Boolean drawShadow) {
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

      public static class SVObject {
        [HarmonyPrefix]
        [UsedImplicitly]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static Boolean prefix_canBeShipped(StardewValley.Object __instance, ref Boolean __result) {
          __result = __instance.Type != null && !__instance.Type.Equals("Quest");
          return false;
        }
      }

      public static class SVUtility {
        [HarmonyPrefix]
        [UsedImplicitly]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static Boolean prefix_highlightShippableObjects(ref Boolean __result) {
          __result = true;
          return false;
        }
      }

      public static class SVShippingMenu {

        [HarmonyPostfix]
        [UsedImplicitly]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static void postfix_parseItems(IList<Item> items, ref List<List<Item>> ___categoryItems, ref List<Int32> ___categoryTotals, ref List<MoneyDial> ___categoryDials,
                                              ref Dictionary<Item, Int32> ___itemValues, ref Dictionary<Item, Int32> ___singleItemValues) {
          const Int32 misc_category = 4; // Misc. category
          const Int32 total_category = 5; // Total category

          foreach (Item item in items) {
            if (item is StardewValley.Object) continue;

            var obj_ex = new SVObjectEx(item);
            Int32 sell_to_store_price = obj_ex.sellToStorePrice();
            Int32 price = sell_to_store_price * obj_ex.Stack;

            ___categoryItems[misc_category].Add(obj_ex); // Add item to misc.
            ___categoryItems[total_category].Add(obj_ex); // Add item to total

            ___categoryTotals[misc_category] += price;
            ___categoryTotals[total_category] += price;

            ___itemValues[obj_ex] = price;
            ___singleItemValues[obj_ex] = sell_to_store_price;

            Game1.stats.ItemsShipped += Convert.ToUInt32(obj_ex.Stack);

            if (obj_ex.countsForShippedCollection())
              Game1.player.shippedBasic(obj_ex.ItemId, obj_ex.Stack);
          }
          ___categoryDials[misc_category].previousTargetValue = ___categoryDials[misc_category].currentValue = ___categoryTotals[misc_category];
          ___categoryDials[total_category].currentValue = ___categoryTotals[total_category];

          Game1.setRichPresence("earnings", ___categoryTotals[total_category]);
        }
      }

      public static class SVGame1 {
        [HarmonyPrefix]
        [UsedImplicitly]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static Boolean prefix__newDayAfterFade() {

          if (Game1.player.useSeparateWallets || Game1.player.IsMainPlayer) {
            var shipping_bin = Game1.getFarm().getShippingBin(Game1.player);
            Int32 extra_gained = 0;

            foreach (Item item in shipping_bin) {
              if (item is StardewValley.Object) continue;

              var obj_ex = new SVObjectEx(item);
              Int32 sell_to_store_price = obj_ex.sellToStorePrice();
              Int32 price = sell_to_store_price * obj_ex.Stack;
              extra_gained += price;
            }

            Game1.player.Money += extra_gained;
          }

          if (Game1.player.useSeparateWallets && Game1.player.IsMainPlayer) {
            foreach (Farmer who2 in Game1.getOfflineFarmhands()) {
              if (who2.isUnclaimedFarmhand)
                continue;

              var shipping_bin = Game1.getFarm().getShippingBin(who2);
              Int32 extra_gained = 0;
              foreach (Item item in shipping_bin) {
                if (item is StardewValley.Object) continue;

                var obj_ex = new SVObjectEx(item);
                Int32 sell_to_store_price = obj_ex.sellToStorePrice();
                Int32 price = sell_to_store_price * obj_ex.Stack;
                extra_gained += price;
              }

              Game1.player.team.AddIndividualMoney(who2, extra_gained);
            }
          }

          return true;
        }
      }
    }

    public override void Entry(IModHelper helper) {
      var harmony = new Harmony("mod.berkayylmao.ShipAnything");
      harmony.Patch(AccessTools.Method(typeof(StardewValley.Object), "canBeShipped"), new HarmonyMethod(AccessTools.Method(typeof(Extensions.SVObject), "prefix_canBeShipped")));
      harmony.Patch(AccessTools.Method(typeof(Utility), "highlightShippableObjects"),
                    new HarmonyMethod(AccessTools.Method(typeof(Extensions.SVUtility), "prefix_highlightShippableObjects")));
      harmony.Patch(AccessTools.Method(typeof(StardewValley.Menus.ShippingMenu), "parseItems"),
                    postfix: new HarmonyMethod(AccessTools.Method(typeof(Extensions.SVShippingMenu), "postfix_parseItems")));
      harmony.Patch(AccessTools.Method(typeof(StardewValley.Game1), "_newDayAfterFade"),
                    new HarmonyMethod(AccessTools.Method(typeof(Extensions.SVGame1), "prefix__newDayAfterFade")));
    }
  }
}
