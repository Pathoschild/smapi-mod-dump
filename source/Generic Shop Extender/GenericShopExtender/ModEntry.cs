using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using SObject = StardewValley.Object;

namespace GenericShopExtender
{
    /// <summary>The mod entry class.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        private ModConfig _config;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.MenuChanged += OnMenuChanged;

            _config = Helper.ReadConfig<ModConfig>();
            //this.Monitor.Log("Printing the configuration", LogLevel.Info);
            //this.Monitor.Log(config.ToString(), LogLevel.Info);
            //foreach (string s in config.shopkeepers.Keys)
            //{
            //    this.Monitor.Log(s, LogLevel.Info);
            //}
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            //Is this even a shop?
            if (e.NewMenu is ShopMenu menu)
            {
                //Yup, sure is.
                ShopMenu currentShopMenu = menu;
                //But who runs it?
                foreach (string shopKeeperKey in _config.Shopkeepers.Keys)
                {
                    string shopKeeper = shopKeeperKey;
                    int yearDefined = 0;
                    List<string> seasonsDefined = new List<string>();
                    //this.Monitor.Log("Checking " + shopkeep + " if it contains " + "_Year", LogLevel.Info);
                    if (shopKeeper.Contains("_Year"))
                    {
                        Monitor.Log("Found a year modifier", LogLevel.Info);
                        yearDefined = int.Parse(shopKeeper.Substring(shopKeeper.IndexOf("_Year", StringComparison.Ordinal) + 5, 1));
                        shopKeeper = shopKeeper.Remove(shopKeeper.IndexOf("_Year", StringComparison.Ordinal), 6);
                        Monitor.Log(shopKeeper, LogLevel.Info);
                        Monitor.Log("With year " + yearDefined, LogLevel.Info);
                    }

                    if (shopKeeper.Contains("_Season"))
                    {
                        Monitor.Log("Found a season modifier", LogLevel.Info);

                        foreach (string season in new[] { "spring", "summer", "fall", "winter" })
                        {
                            string marker = $"_{season}";
                            if (shopKeeper.IndexOf(marker, StringComparison.Ordinal) != -1)
                            {
                                seasonsDefined.Add(season);
                                shopKeeper = shopKeeper.Remove(shopKeeper.IndexOf(marker, StringComparison.Ordinal), marker.Length);
                            }
                        }
                        shopKeeper = shopKeeper.Remove(shopKeeper.IndexOf("_Season", StringComparison.Ordinal), 7);
                    }
                    else
                    {
                        seasonsDefined.Add("spring");
                        seasonsDefined.Add("summer");
                        seasonsDefined.Add("winter");
                        seasonsDefined.Add("fall");
                    }
                    Monitor.Log(shopKeeper, LogLevel.Info);

                    if (currentShopMenu.portraitPerson.Equals(Game1.getCharacterFromName(shopKeeper)) && Game1.year >= yearDefined && seasonsDefined.Contains(Game1.currentSeason))
                    {
                        //The current shopkeep does! Now we need to get the list of what's being sold
                        IReflectedField<Dictionary<Item, int[]>> inventoryInformation = Helper.Reflection.GetField<Dictionary<Item, int[]>>(currentShopMenu, "itemPriceAndStock");
                        Dictionary<Item, int[]> itemPriceAndStock = inventoryInformation.GetValue();
                        IReflectedField<List<Item>> forSaleInformation = Helper.Reflection.GetField<List<Item>>(currentShopMenu, "forSale");
                        List<Item> forSale = forSaleInformation.GetValue();

                        //Now, lets add a few things...
                        int[,] itemsAndPrices = _config.Shopkeepers[shopKeeperKey];
                        for (int index = 0; index < itemsAndPrices.GetLength(0); index++)
                        {
                            int itemId = itemsAndPrices[index, 0];
                            int price = itemsAndPrices[index, 1];
                            Item objectToAdd = new SObject(Vector2.Zero, itemId, int.MaxValue);
                            itemPriceAndStock.Add(objectToAdd, new[] { price, int.MaxValue });
                            forSale.Add(objectToAdd);
                        }

                        //Now, lets update that shop inventory with the new items
                        inventoryInformation.SetValue(itemPriceAndStock);
                        forSaleInformation.SetValue(forSale);
                    }
                }
            }
        }
    }
}
