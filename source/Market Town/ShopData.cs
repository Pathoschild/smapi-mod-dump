/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/congha22/foodstore
**
*************************************************/

using HarmonyLib;
using MarketTown.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Shops;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MarketTown
{
    public partial class ModEntry
    {

        public static void GenerateShop(string shopId, Vector2 tile)
        {
            ShopsJsonData.Clear();
            ShopsJsonData = ModEntry.Instance.Helper.ModContent.Load<IDictionary<string, List<string>>>("assets/MarketTownShops.json");

            List<string> itemBlackList = new List<string>();

            if (ShopsJsonData.ContainsKey("MarketTown.ITEM_BLACK_LIST"))
                itemBlackList = ShopsJsonData["MarketTown.ITEM_BLACK_LIST"];

            List<string> shopNames = new List<string> {
                "MarketTown.bluemoonShop",     // PREFER: ARTISAN, VEGETABLES, FLOWER      
                "MarketTown.clintShop",        // PREFER: ORE, BAR, GEMS
                "MarketTown.emeraldShop",      // PREFER: MINERAL, GEMS
                "MarketTown.emilyShop",        // PREFER: CLOTHES, HAT,BOOTS
                "MarketTown.evelynShop",       // PREFER: FLOWER, VEGETABLES, SEEDS
                "MarketTown.fairhavenShop",    // PREFER: FRUITS, VEGETABLES, FLOWER
                "MarketTown.fishShop",         // PREFER: FISH, BAIT, TACKLE, FURNITURE
                "MarketTown.guntherShop",      // PREFER: ARTIFACT, MINERAL
                "MarketTown.haleyShop",        // PREFER: FLOWER, CLOTHES
                "MarketTown.harveyShop",       // PREFER: FOOD, ENERGY
                "MarketTown.hatsShop",         // PREFER: HAT, CLOTHES
                "MarketTown.heapsShop",        // PREFER: FERTILIZER, VEGETABLE, MISC
                "MarketTown.jodiShop",         // PREFER: FOOD, FRUIT
                "MarketTown.jvShop",           // PREFER: FORAGE, FLOWER, SWEET
                "MarketTown.labShop",          // PREFER: MACHINE, CRAFTING
                "MarketTown.leahShop",         // PREFER: FURNITURE
                "MarketTown.linusShop",        // PREFER: FORAGE, FRUIT, FISH
                "MarketTown.marnieShop",       // PREFER: ANIMAL, MISC, CROP
                "MarketTown.pikaShop",         // PREFER: FOOD
                "MarketTown.primeShop",        // PREFER: MISC
                "MarketTown.quillShop",        // PREFER: FISH, BEACH FORAGE
                "MarketTown.roboshackShop",    // PREFER: TECH, BAR
                "MarketTown.rockShop",         // PREFER: DECOR, FRUIT, FURNITURE
                "MarketTown.saloonShop",       // PREFER: FOOD
                "MarketTown.serrupShop",       // PREFER: ARTISAN
                "MarketTown.teaShop",          // PREFER: VEGETABLE, SEED
                "MarketTown.weaponsShop"       // PREFER: WEAPON, MONSTER
            };

            Dictionary<string, string> shopOwner = new Dictionary<string, string>
            {
                {"MarketTown.bluemoonShop", "Sophia"  },
                {"MarketTown.clintShop",    "Clint"  },
                {"MarketTown.emeraldShop",  "Susan"  },
                {"MarketTown.emilyShop",    "Emily"  },
                {"MarketTown.evelynShop",   "Evelyn"  },
                {"MarketTown.fairhavenShop","Andy"  },
                {"MarketTown.fishShop",     "Willy"  },
                {"MarketTown.guntherShop",  "Gunther"  },
                {"MarketTown.haleyShop",    "Haley"  },
                {"MarketTown.harveyShop",   "Harvey"  },
                {"MarketTown.hatsShop",     ""  },
                {"MarketTown.heapsShop",    "Lorenzo"  },
                {"MarketTown.jodiShop",     "Jodi"  },
                {"MarketTown.jvShop",       "Jas"  },
                {"MarketTown.labShop",      "Demetrius"  },
                {"MarketTown.leahShop",     "Leah"  },
                {"MarketTown.linusShop",    "Linus"  },
                {"MarketTown.marnieShop",   "Marnie"  },
                {"MarketTown.pikaShop",     "Pika"  },
                {"MarketTown.primeShop",    "Pierre"  },
                {"MarketTown.quillShop",    "Elliott"  },
                {"MarketTown.roboshackShop","Maru"  },
                {"MarketTown.rockShop",     "Sam"  },
                {"MarketTown.saloonShop",   "Gus"  },
                {"MarketTown.serrupShop",   ""  },
                {"MarketTown.teaShop",      "Caroline"  },
                {"MarketTown.weaponsShop",  "Marlon"  }    
            };
            
            // Set shop owner tile
            if (shopOwner.TryGetValue(shopId, out string owner) && owner != null && owner != "" )
            {
                var thisNPC = new NPC();

                if (Game1.getCharacterFromName(owner) != null)
                {
                    thisNPC = Game1.getCharacterFromName(owner);
                    if (!IslandNPCList.Contains(owner)) IslandNPCList.Add(owner);
                }
                else if (Game1.getCharacterFromName("MT.Guest_" + owner) != null)
                {
                    thisNPC = Game1.getCharacterFromName("MT.Guest_" + owner);
                    if (!IslandNPCList.Contains("MT.Guest_" + owner) ) IslandNPCList.Add("MT.Guest_" + owner);
                }

                thisNPC.modData["hapyke.FoodStore/shopOwnerToday"] = $"{(tile + new Vector2(3, -1)).X}, {(tile + new Vector2(3, -1)).Y}";
            }

            Dictionary<string, List<string>> shopLists = new Dictionary<string, List<string>>();

            foreach (var name in shopNames)
            {
                if (shopLists.ContainsKey(name)) shopLists[name].Clear();
                shopLists[name] = ShopsJsonData[name];
            }

            // This will add all items in a specified CATEGORY to the possible shop stock
            foreach (var x in shopLists)
            {
                IEnumerable<string> categoryKeyElements = x.Value.Where(element => element.StartsWith("CATEGORYKEY"));

                if (categoryKeyElements.Any())
                {
                    List<string> eleToAdd = new List<string>();
                    foreach (var element in categoryKeyElements)
                    {
                        switch (element)
                        {
                            case "CATEGORYKEY-97":
                                foreach (var kvp in DataLoader.Boots(Game1.content))
                                {
                                    if( !itemBlackList.Contains("(B)" + kvp.Key) ) eleToAdd.Add("(B)" +kvp.Key);
                                }
                                break;
                            case "CATEGORYKEY-9":
                                foreach (var kvp in DataLoader.BigCraftables(Game1.content))
                                {
                                    if (!itemBlackList.Contains("(BC)" + kvp.Key)) eleToAdd.Add("(BC)" + kvp.Key);
                                }
                                break;
                            case "CATEGORYKEY-95":
                                foreach (var kvp in DataLoader.Hats(Game1.content))
                                {
                                    if (!itemBlackList.Contains("(H)" + kvp.Key)) eleToAdd.Add("(H)" + kvp.Key);
                                }
                                break;
                            case "CATEGORYKEY-98":
                                foreach (var kvp in Game1.weaponData)
                                {
                                    if (!itemBlackList.Contains("(W)" + kvp.Key)) eleToAdd.Add("(W)" + kvp.Key);
                                }
                                break;
                            case "CATEGORYKEY-99":
                                foreach (var kvp in DataLoader.Tools(Game1.content))
                                {
                                    if (!itemBlackList.Contains("(T)" + kvp.Key)) eleToAdd.Add("(T)" + kvp.Key);
                                }
                                break;

                        }
                    }
                    x.Value.AddRange(eleToAdd);
                }

            }

            foreach (var obj in Game1.objectData)
            {
                if (   itemBlackList.Contains("(O)" + obj.Key) 
                    || itemBlackList.Contains("(BC)" + obj.Key) 
                    || itemBlackList.Contains("(W)" + obj.Key)
                    || itemBlackList.Contains("(F)" + obj.Key)
                    || itemBlackList.Contains("(S)" + obj.Key)
                    || itemBlackList.Contains("(P)" + obj.Key)
                    || itemBlackList.Contains("(H)" + obj.Key)
                    || itemBlackList.Contains("(T)" + obj.Key)
                    || itemBlackList.Contains("(B)" + obj.Key)
                    || itemBlackList.Contains("(WP)" + obj.Key)
                    || itemBlackList.Contains("(FL)" + obj.Key)
                    || itemBlackList.Contains("(M)" + obj.Key)
                    || itemBlackList.Contains("(TR))" + obj.Key)
                    || obj.Value.Category == -999 ) continue; // Blacklist item


                foreach ( var shopGenerate in shopLists)
                {
                    if ( shopGenerate.Value.Contains("CATEGORYKEY" + obj.Value.Category.ToString()) )
                    {
                        shopGenerate.Value.Add("(O)" + obj.Key);
                    }
                }
            }

            foreach (var obj in Game1.pantsData)
            {
                if (itemBlackList.Contains("(P)" + obj.Key)) continue;
                shopLists["MarketTown.emilyShop"].Add("(P)" + obj.Key);
            }

            foreach (var obj in Game1.shirtData)
            {
                if (itemBlackList.Contains("(S)" + obj.Key)) continue;
                shopLists["MarketTown.emilyShop"].Add("(S)" + obj.Key);
            }

            List<string> initShopStockList = new List<string>();
            foreach (var shop in StardewValley.DataLoader.Shops(Game1.content))
            {
                if (shop.Key == shopId)
                {
                    shop.Value.Items.Clear();
                    int i = 1;
                    int tried = 0;
                    while (i <= shopLists[shopId].Count && i <= 9 && tried <= 18)
                    {
                        var randomItemId = shopLists[shopId][Game1.random.Next(shopLists[shopId].Count)];
                        if (randomItemId.Contains("CATEGORYKEY")) { tried++; continue; }

                        var salePrice = 100;

                        // try to create the item and add it to the shop stock if item is valid
                        var realItem = ItemRegistry.Create<Item>(randomItemId, default, default, true);
                        if (realItem == null)
                        {
                            tried++;
                            continue;
                        }

                        salePrice = realItem.sellToStorePrice();
                        if (0 < salePrice && salePrice < 10) salePrice *= 15;
                        else if (10 <= salePrice && salePrice < 20) salePrice *= 10;
                        else if (salePrice <= 0)
                        {
                            salePrice = realItem.salePrice();
                            if (0 < salePrice && salePrice < 10) salePrice *= 10;
                            else if (10 <= salePrice && salePrice < 20) salePrice *= 7;
                            else if (salePrice <= 0) salePrice = Game1.random.Next(200, 1000);
                        }

                        if (randomItemId.StartsWith("(W)"))
                        {
                            if (salePrice >= 800) salePrice = (int)(salePrice * 1.1);
                            salePrice = salePrice * salePrice / 20;
                        }

                        if (randomItemId.StartsWith("(H)"))
                        {
                            salePrice = Game1.random.Next(500, 1000);
                        }

                        if (randomItemId.StartsWith("(S)") || randomItemId.StartsWith("(P)"))
                        {
                            salePrice = Game1.random.Next(1000, 1500);
                        }

                        if (randomItemId.StartsWith("(F)") && !randomItemId.ToLower().Contains("catalogue"))
                        {
                            salePrice = (int)(salePrice / 6);
                        }

                        if (randomItemId.StartsWith("(T)"))
                        {
                            salePrice = Game1.random.Next(500, 1000);
                        }

                        if (randomItemId.StartsWith("(B)"))
                        {
                            salePrice = salePrice * 2;
                        }

                        ShopItemData newItem = new ShopItemData { AvailableStock = 1, IsRecipe = false, AvoidRepeat = true,
                            ItemId = randomItemId,
                            Price = (int)(salePrice * ( 1 + Game1.random.NextDouble()) ),
                        };

                        if (!initShopStockList.Contains(randomItemId) )
                        {
                            shop.Value.Items.Add(newItem);
                            initShopStockList.Add(randomItemId);
                            i++;
                        }
                    }
                }
            }
            TodayShopInventory.Add(new MarketShopData(shopId, tile, initShopStockList.Distinct().ToList()));
        }
    }
}