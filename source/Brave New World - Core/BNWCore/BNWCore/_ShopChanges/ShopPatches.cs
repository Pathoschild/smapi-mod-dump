/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Tools;
using StardewValley.Util;
using System;
using System.Collections.Generic;
using xTile.ObjectModel;
using xTile.Tiles;
using Microsoft.Xna.Framework;
using Object = StardewValley.Object;

namespace BNWCore.Patches
{
    internal class ShopPatches
    {
        public static bool GameLocation_performAction_Prefix(GameLocation __instance, string action, ref bool __result)
        {
            if (ModEntry.ModHelper.ModRegistry.IsLoaded("DiogoAlbano.BNWChapter1"))
            {
                string[] actionParams = action.Split(' ');
                string text = actionParams[0];
                List<Response> options = new List<Response>();
                switch (text)
                {
                    case "Buy":
                        __instance.openShopMenu(actionParams[1]);
                        __result = true;
                        return false;
                    case "Carpenter":
                        if (Game1.player.daysUntilHouseUpgrade.Value < 0 && !Game1.getFarm().isThereABuildingUnderConstruction())
                        {
                            if (Game1.IsMasterGame)
                            {
                                if (Game1.player.HouseUpgradeLevel < 3)
                                {
                                    options.Add(new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeHouse")));
                                }
                            }
                            else if (Game1.player.HouseUpgradeLevel < 3)
                            {
                                options.Add(new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeCabin")));
                            }
                            if (Game1.player.HouseUpgradeLevel >= 2)
                            {
                                if (Game1.IsMasterGame)
                                {
                                    options.Add(new Response("Renovate", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_RenovateHouse")));
                                }
                                else
                                {
                                    options.Add(new Response("Renovate", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_RenovateCabin")));
                                }
                            }
                            options.Add(new Response("Construct", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Construct")));
                            options.Add(new Response("Leave", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Leave")));
                            __instance.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu"), options.ToArray(), "carpenter");
                        }
                        else
                        {
                            Game1.activeClickableMenu = new ShopMenu(Utility.getCarpenterStock(), 0, null, null, null, null);
                        }
                        __result = true;
                        return false;
                    case "Blacksmith":
                        if (Game1.player.toolBeingUpgraded.Value != null && Game1.player.daysLeftForToolUpgrade.Value <= 0)
                        {
                            if (Game1.player.freeSpotsInInventory() > 0 || Game1.player.toolBeingUpgraded.Value is GenericTool)
                            {
                                Tool tool = Game1.player.toolBeingUpgraded.Value;
                                Game1.player.toolBeingUpgraded.Value = null;
                                Game1.player.hasReceivedToolUpgradeMessageYet = false;
                                Game1.player.holdUpItemThenMessage(tool, true);
                                if (tool is GenericTool)
                                {
                                    tool.actionWhenClaimed();
                                }
                                else
                                {
                                    Game1.player.addItemToInventoryBool(tool, false);
                                }
                                if (Game1.player.team.useSeparateWallets.Value && tool.UpgradeLevel == 4)
                                {
                                    AccessTools.FieldRefAccess<Game1, Multiplayer>(null, "multiplayer").globalChatInfoMessage("IridiumToolUpgrade", new string[]
                                    {
                                    Game1.player.Name,
                                    tool.DisplayName
                                    });
                                }
                            }
                            else
                            {
                                Game1.drawDialogue(Game1.getCharacterFromName("Clint"), Game1.content.LoadString("Data\\ExtraDialogue:Clint_NoInventorySpace"));
                            }
                        }
                        else
                        {
                            Response[] responses;
                            if (Game1.player.hasItemInInventory(535, 1, 0) || Game1.player.hasItemInInventory(536, 1, 0) || Game1.player.hasItemInInventory(537, 1, 0) || Game1.player.hasItemInInventory(749, 1, 0) || Game1.player.hasItemInInventory(275, 1, 0) || Game1.player.hasItemInInventory(791, 1, 0))
                            {
                                responses = new Response[]
                                {
                                new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Upgrade")),
                                new Response("Process", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Geodes")),
                                new Response("Leave", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Leave"))
                                };
                            }
                            else
                            {
                                responses = new Response[]
                                {
                                new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Upgrade")),
                                new Response("Leave", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Leave"))
                                };
                            }
                            __instance.createQuestionDialogue("", responses, "Blacksmith");
                        }
                        __result = true;
                        return false;
                    case "AnimalShop":
                        options = new List<Response>
                    {
                        new Response("Purchase", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Animals")),
                        new Response("Leave", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Leave"))
                    };
                        __instance.createQuestionDialogue("", options.ToArray(), "Marnie");
                        __result = true;
                        return false;
                }
            }
            return true;
        }
        public static bool GameLocation_openShopMenu_Prefix(GameLocation __instance, string which, ref bool __result)
        {
            if (ModEntry.ModHelper.ModRegistry.IsLoaded("DiogoAlbano.BNWChapter1"))
            {
                string name = null;
                switch (which)
                {
                    case "Fish":
                        foreach (var c in __instance.characters)
                        {
                            if (c.Name == "Willy")
                            {
                                name = "Willy";
                                break;
                            }
                        }
                        Game1.activeClickableMenu = new ShopMenu(Utility.getFishShopStock(Game1.player), 0, name, null, null, null);
                        __result = true;
                        return false;
                    case "General":
                        foreach (var c in __instance.characters)
                        {
                            if (c.Name == "Pierre")
                            {
                                name = "Pierre";
                                break;
                            }
                        }
                        Game1.activeClickableMenu = new ShopMenu((__instance as SeedShop).shopStock(), 0, name, null, null, null);
                        __result = true;
                        return false;
                    case "SandyShop":
                        foreach (var c in __instance.characters)
                        {
                            if (c.Name == "Sandy")
                            {
                                name = "Sandy";
                                break;
                            }
                        }
                        Game1.activeClickableMenu = new ShopMenu((Dictionary<ISalable, int[]>)AccessTools.Method(typeof(GameLocation), "sandyShopStock").Invoke(__instance, new object[] { }), 0, name, (Func<ISalable, Farmer, int, bool>)Delegate.CreateDelegate(typeof(Func<ISalable, Farmer, int, bool>), __instance, AccessTools.Method(typeof(GameLocation), "onSandyShopPurchase")), null, null);
                        __result = true;
                        return false;
                }
            }
            return true;
        }
    }
    public class TileUtility
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
                case "bnw!PierreShop":
                    {
                        var seedShop = new SeedShop();
                        return new ShopMenu(seedShop.shopStock(), 0, null, null, null, null);
                    }
                case "bnw!JojaShop":
                    return new ShopMenu(Utility.getJojaStock());
                case "bnw!RobinShop":
                    return new ShopMenu(Utility.getCarpenterStock(), 0, null, null, null, null);
                case "bnw!ClintShop":
                    return new ShopMenu(Utility.getBlacksmithStock(), 0, null, null, null, null);
                case "bnw!ClintToolUpgrades":
                    return new ShopMenu(Utility.getBlacksmithUpgradeStock(Game1.player), 0, null);
                case "bnw!MarlonShop":
                    return new ShopMenu(Utility.getAdventureShopStock(), 0, null, null, null, null);
                case "bnw!MarnieShop":
                    return new ShopMenu(Utility.getAnimalShopStock(), 0, null, null, null, null);
                case "bnw!TravellingMerchant":
                    return new ShopMenu(Utility.getTravelingMerchantStock((int)((long)Game1.uniqueIDForThisGame + Game1.stats.DaysPlayed)),
                        0, null, Utility.onTravelingMerchantShopPurchase);
                case "bnw!HarveyShop":
                    return new ShopMenu(Utility.getHospitalStock());
                case "bnw!SandyShop":
                    {
                        var SandyStock = ModEntry.ModHelper.Reflection.GetMethod(Game1.currentLocation, null).Invoke<Dictionary<ISalable, int[]>>();
                        return new ShopMenu(SandyStock, 0, null, onSandyShopPurchase);

                    }
                case "bnw!DesertTrader":
                    return new ShopMenu(Desert.getDesertMerchantTradeStock(Game1.player), 0,
                        null, boughtTraderItem);
                case "bnw!KrobusShop":
                    {
                        var sewer = new Sewer();
                        return new ShopMenu(sewer.getShadowShopStock(),
                            0, null, sewer.onShopPurchase);

                    }
                case "bnw!DwarfShop":
                    return new ShopMenu(StardewValley.Utility.getDwarfShopStock(), 0, null, null, null, null);
                case "bnw!AdventureRecovery":
                    return new ShopMenu(StardewValley.Utility.getAdventureRecoveryStock(), 0, null);
                case "bnw!GusShop":
                    {
                        return new ShopMenu(StardewValley.Utility.getSaloonStock(), 0, null, (item, farmer, amount) =>
                        {
                            Game1.player.team.synchronizedShopStock.OnItemPurchased(SynchronizedShopStock.SynchedShop.Saloon, item, amount);
                            return false;
                        });
                    }
                case "bnw!WillyShop":
                    return new ShopMenu(StardewValley.Utility.getFishShopStock(Game1.player), 0, "Willy");
                case "bnw!QiShop":
                    Game1.activeClickableMenu = new ShopMenu(StardewValley.Utility.getQiShopStock(), 2);
                    break;
                case "bnw!IceCreamStand":
                    return new ShopMenu(new Dictionary<ISalable, int[]>
                    {
                                    {
                                         new Object(233, 1),
                                        new[]{ 250, int.MaxValue }
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
