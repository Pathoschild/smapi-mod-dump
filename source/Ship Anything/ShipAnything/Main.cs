using System;
using System.Collections.Generic;
using System.Reflection;

using Harmony;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

using StardewValley.BellsAndWhistles;

namespace ShipEverything {
   public class Main : Mod {
      public static class Extensions {
         public static class SVObject {
            [HarmonyPrefix]
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
            public static Boolean prefix_highlightShippableObjects(ref Boolean __result) {
               __result = true;
               return false;
            }
         }
         public static class SVShippingMenu {
            public class SVObjectEx : StardewValley.Object {
               public delegate void drawInMenuDelegate(SpriteBatch spriteBatch, Vector2 location, Single scaleSize, Single transparency, Single layerDepth, StardewValley.StackDrawType drawStackNumber, Color color, Boolean drawShadow);
               private drawInMenuDelegate orgCall;
               public SVObjectEx(StardewValley.Item item) : base() {
                  orgCall = new drawInMenuDelegate(item.drawInMenu);

                  this.Category = item.Category;
                  this.DisplayName = item.DisplayName;
                  this.HasBeenInInventory = item.HasBeenInInventory;
                  this.Name = item.Name;
                  this.ParentSheetIndex = item.ParentSheetIndex;
                  this.Price = Math.Max(1, item.salePrice());
                  this.specialItem = item.specialItem;
                  this.SpecialVariable = item.SpecialVariable;
               }

               public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, Single scaleSize, Single transparency, Single layerDepth, StardewValley.StackDrawType drawStackNumber, Color color, Boolean drawShadow) {
                  orgCall(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
               }
            }
            public static void postfix_parseItems(IList<StardewValley.Item> items, ref List<List<StardewValley.Item>> ___categoryItems, ref List<Int32> ___categoryTotals, ref List<MoneyDial> ___categoryDials, ref Dictionary<StardewValley.Item, int> ___itemValues) {
               Int32 gainedMoney = 0;
               foreach (var item in items) {
                  if (!(item is StardewValley.Object)) {
                     var _obj = new SVObjectEx(item);
                     ___categoryItems[4].Add(_obj); // Add item to misc.
                     ___categoryItems[5].Add(_obj); // Add item to total
                     ___categoryTotals[4] += _obj.Price;
                     ___categoryTotals[5] += _obj.Price;
                     ___itemValues[_obj] = _obj.Price;
                     ___categoryDials[4].previousTargetValue = ___categoryDials[4].currentValue = ___categoryTotals[4];
                     gainedMoney += _obj.Price;
                     StardewValley.Game1.stats.itemsShipped += 1;
                  }
               }
               ___categoryDials[5].currentValue = ___categoryTotals[5];
               if (StardewValley.Game1.IsMasterGame)
                  StardewValley.Game1.player.Money += gainedMoney;
               StardewValley.Game1.setRichPresence("earnings", ___categoryTotals[5]);
            }
         }
      }

      public override void Entry(IModHelper helper) {
         var harmony = HarmonyInstance.Create("mod.berkay2578.ShipAnything");
         MethodInfo[] targetMethods = {
            AccessTools.Method(typeof(StardewValley.Object), "canBeShipped"),
            AccessTools.Method(typeof(StardewValley.Utility), "highlightShippableObjects"),
            AccessTools.Method(typeof(StardewValley.Menus.ShippingMenu), "parseItems")
         };
         HarmonyMethod[] patches = {
            new HarmonyMethod(AccessTools.Method(typeof(Extensions.SVObject), "prefix_canBeShipped")),
            new HarmonyMethod(AccessTools.Method(typeof(Extensions.SVUtility), "prefix_highlightShippableObjects")),
            new HarmonyMethod(AccessTools.Method(typeof(Extensions.SVShippingMenu), "postfix_parseItems")),
         };
         harmony.Patch(targetMethods[0], patches[0]);
         harmony.Patch(targetMethods[1], patches[1]);
         harmony.Patch(targetMethods[2], null, patches[2]);
      }
   }
}
