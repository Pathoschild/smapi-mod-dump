/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Util;
using System.Collections.Generic;
using xTile.ObjectModel;
using xTile.Tiles;
using Microsoft.Xna.Framework;
using Object = StardewValley.Object;

namespace BNWCore
{
    public class Tile_Utility
    {
        public static IPropertyCollection GetTileProperty(GameLocation map, string layer, Vector2 tile)
        {
            if (map == null)
                return null;
            Tile checkTile = map.Map.GetLayer(layer).Tiles[(int)tile.X, (int)tile.Y];
            return checkTile?.Properties;
        }
        public static IClickableMenu CheckVanillaShop(string shopProperty, out bool warpingShop)
        {
            warpingShop = false;
            switch (shopProperty)
            {
                case "bnw_PierreShop":
                    {
                        var seedShop = new SeedShop();
                        return new ShopMenu(seedShop.shopStock(), 0, null, null, null, null);
                    }
                case "bnw_JojaShop":
                    return new ShopMenu(Utility.getJojaStock());
                case "bnw_RobinShop":
                    return new ShopMenu(Utility.getCarpenterStock(), 0, null, null, null, null);
                case "bnw_RobinBuilder":
                    warpingShop = true;
                    if (Game1.player.daysUntilHouseUpgrade.Value < 0 && !Game1.getFarm().isThereABuildingUnderConstruction())
                        return new CarpenterMenu();
                    else
                        return new DialogueBox(ModEntry.ModHelper.Translation.Get("translator_dialog_box_robin_busy"));
                case "bnw_ClintShop":
                    return new ShopMenu(Utility.getBlacksmithStock(), 0, null, null, null, null);
                case "bnw_MarlonShop":
                    return new ShopMenu(Utility.getAdventureShopStock(), 0, null, null, null, null);
                case "bnw_MarnieShop":
                    warpingShop = true;
                    return new ShopMenu(Utility.getAnimalShopStock(), 0, null, null, null, null);
                case "bnw_TravellingMerchant":
                    return new ShopMenu(Utility.getTravelingMerchantStock((int)((long)Game1.uniqueIDForThisGame + Game1.stats.DaysPlayed)),
                        0, null, Utility.onTravelingMerchantShopPurchase);
                case "bnw_HarveyShop":
                    return new ShopMenu(Utility.getHospitalStock());
                case "bnw_SandyShop":
                    {
                        var SandyStock = ModEntry.ModHelper.Reflection.GetMethod(Game1.currentLocation, null).Invoke<Dictionary<ISalable, int[]>>();
                        return new ShopMenu(SandyStock, 0, null, onSandyShopPurchase);
                    }
                case "bnw_DesertTrader":
                    return new ShopMenu(Desert.getDesertMerchantTradeStock(Game1.player), 0, null, boughtTraderItem);
                case "bnw_KrobusShop":
                    {
                        var sewer = new Sewer();
                        return new ShopMenu(sewer.getShadowShopStock(), 0, null, sewer.onShopPurchase);
                    }
                case "bnw_DwarfShop":
                    return new ShopMenu(Utility.getDwarfShopStock(), 0, null, null, null, null);
                case "bnw_AdventureRecovery":
                    return new ShopMenu(Utility.getAdventureRecoveryStock(), 0, null);
                case "bnw_GusShop":
                    {
                        return new ShopMenu(Utility.getSaloonStock(), 0, null, (item, farmer, amount) =>
                        {
                            Game1.player.team.synchronizedShopStock.OnItemPurchased(SynchronizedShopStock.SynchedShop.Saloon, item, amount);
                            return false;
                        });
                    }
                case "bnw_WillyShop":
                    return new ShopMenu(StardewValley.Utility.getFishShopStock(Game1.player), 0, "Willy");
                case "bnw_QiShop":
                    Game1.activeClickableMenu = new ShopMenu(Utility.getQiShopStock(), 2);
                    break;
                case "bnw_IceCreamStand":
                    return new ShopMenu(new Dictionary<ISalable, int[]>
                    {
                        {
                            new Object(233, 1), new[]{ 250, int.MaxValue }
                        }
                    });
            }
            return null;
        }
        private static bool boughtTraderItem(ISalable s, Farmer f, int i)
        {
            if (s.Name == "Magic Rock Candy")
                Desert.boughtMagicRockCandy = true;
            return false;
        }
        private static bool onSandyShopPurchase(ISalable item, Farmer who, int amount)
        {
            Game1.player.team.synchronizedShopStock.OnItemPurchased(SynchronizedShopStock.SynchedShop.Sandy, item, amount);
            return false;
        }
    }
}
