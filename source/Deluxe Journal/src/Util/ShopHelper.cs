/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using StardewValley;
using StardewValley.GameData.Shops;
using StardewValley.Menus;

namespace DeluxeJournal.Util
{
    public static class ShopHelper
    {
        private static readonly HashSet<string> IgnoredShops = new()
        {
            Game1.shop_adventurersGuildItemRecovery,
            Game1.shop_blacksmithUpgrades,
            Game1.shop_petAdoption,
            Game1.shop_casino,
            Game1.shop_bookseller_trade,
            Game1.shop_desertTrader,
            Game1.shop_islandTrader,
            Game1.shop_qiGemShop,
            Game1.shop_catalogue,
            Game1.shop_furnitureCatalogue,
            Game1.shop_jojaCatalogue,
            Game1.shop_wizardCatalogue,
            Game1.shop_junimoCatalogue,
            Game1.shop_retroCatalogue,
            Game1.shop_trashCatalogue,
            "Concessions",
            "Raccoon"
        };

        /// <summary>Get the item sale price listed by a shop.</summary>
        /// <param name="item">The item to search for in a shop's stock.</param>
        /// <param name="shopId">A specific shop ID to check the item price from. If <c>null</c>, then search all shops.</param>
        /// <returns>The item price listed by a shop or <c>-1</c> if none was found.</returns>
        public static int GetItemSalePrice(Item item, string? shopId = null)
        {
            foreach (KeyValuePair<string, ShopData> pair in DataLoader.Shops(Game1.content))
            {
                if ((shopId != null ? shopId == pair.Key : !(IgnoredShops.Contains(pair.Key) || pair.Key.StartsWith("DesertFestival")))
                    && pair.Value.Currency == 0)
                {
                    foreach (ShopItemData itemData in pair.Value.Items)
                    {
                        if (!itemData.IsRecipe && itemData.ItemId == item.QualifiedItemId && itemData.TradeItemId == null)
                        {
                            switch (item.QualifiedItemId)
                            {
                                case "(O)378": // copper ore
                                case "(O)380": // iron ore
                                case "(O)382": // coal
                                case "(O)384": // gold ore
                                case "(O)388": // wood
                                case "(O)390": // stone
                                    if (itemData.Condition != null && !GameStateQuery.CheckConditions(itemData.Condition))
                                    {
                                        continue;
                                    }
                                    break;
                            }

                            float price = itemData.Price;

                            if (price < 0)
                            {
                                price = itemData.UseObjectDataPrice && item is SObject obj ? obj.Price : item.salePrice(true);
                            }

                            if (itemData.ApplyProfitMargins ?? item.appliesProfitMargins())
                            {
                                price *= Game1.MasterPlayer.difficultyModifier;
                            }

                            if (!itemData.IgnoreShopPriceModifiers)
                            {
                                price = Utility.ApplyQuantityModifiers(price, pair.Value.PriceModifiers, pair.Value.PriceModifierMode, targetItem: item);
                            }

                            return (int)price;
                        }
                    }
                }
            }

            return -1;
        }

        /// <summary>Attach an onPurchase callback to a ShopMenu.</summary>
        /// <param name="shop">The ShopMenu.</param>
        /// <param name="onPurchase">The callback to be attached. A return value of true exits the menu.</param>
        public static void AttachPurchaseCallback(ShopMenu shop, Func<ISalable, Farmer, int, bool> onPurchase)
        {
            Func<ISalable, Farmer, int, bool> origOnPurchase = shop.onPurchase;

            shop.onPurchase = delegate (ISalable salable, Farmer player, int amount)
            {
                bool exit = onPurchase(salable, player, amount);

                if (origOnPurchase != null)
                {
                    return origOnPurchase(salable, player, amount) || exit;
                }

                return exit;
            };
        }
    }
}
