using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using StardewValley.Menus;
using System;

namespace GenericShopExtender
{
    public class ModEntry : Mod
    {

        private ModConfig config;

        /*********
        ** Public methods
        *********/
        /// <summary>Initialise the mod.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
        public override void Entry(IModHelper helper)
        {
            MenuEvents.MenuChanged += Events_MenuChanged;

            config = this.Helper.ReadConfig<ModConfig>();
            //this.Monitor.Log("Printing the configuration", LogLevel.Info);
            //this.Monitor.Log(config.ToString(), LogLevel.Info);
            //foreach (string s in config.shopkeepers.Keys)
            //{
            //    this.Monitor.Log(s, LogLevel.Info);
            //}
        }

        void Events_MenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            //Is this even a shop?
            if (e.NewMenu is ShopMenu)
            {
                //Yup, sure is.
                ShopMenu currentShopMenu = (ShopMenu)e.NewMenu;
                //But who runs it?
                foreach (string shopkeep in config.shopkeepers.Keys)
                {
                    string formattedShopkeep = shopkeep;
                    int yearDefined = 0;
                    List<string> seasonsDefined = new List<string>();
                    //this.Monitor.Log("Checking " + shopkeep + " if it contains " + "_Year", LogLevel.Info);
                    if (formattedShopkeep.Contains("_Year"))
                    {
                        this.Monitor.Log("Found a year modifier", LogLevel.Info);
                        yearDefined = Int32.Parse(formattedShopkeep.Substring(formattedShopkeep.IndexOf("_Year") + 5,1));
                        formattedShopkeep = formattedShopkeep.Remove(formattedShopkeep.IndexOf("_Year"), 6);
                        this.Monitor.Log(formattedShopkeep, LogLevel.Info);
                        this.Monitor.Log("With year " + yearDefined, LogLevel.Info);
                    }

                    if(formattedShopkeep.Contains("_Season"))
                    {
                        this.Monitor.Log("Found a season modifier", LogLevel.Info);
                        if (formattedShopkeep.IndexOf("_spring") != -1)
                        {
                            seasonsDefined.Add("spring");
                            formattedShopkeep = formattedShopkeep.Remove(formattedShopkeep.IndexOf("_spring"), 7);
                        }
                        if (formattedShopkeep.IndexOf("_summer") != -1)
                        {
                            seasonsDefined.Add("summer");
                            formattedShopkeep = formattedShopkeep.Remove(formattedShopkeep.IndexOf("_summer"), 7);
                        }
                        if (formattedShopkeep.IndexOf("_winter") != -1)
                        {
                            seasonsDefined.Add("winter");
                            formattedShopkeep = formattedShopkeep.Remove(formattedShopkeep.IndexOf("_winter"), 7);
                        }
                        if (formattedShopkeep.IndexOf("_fall") != -1)
                        {
                            seasonsDefined.Add("fall");
                            formattedShopkeep = formattedShopkeep.Remove(formattedShopkeep.IndexOf("_fall"), 5);
                        }
                        formattedShopkeep = formattedShopkeep.Remove(formattedShopkeep.IndexOf("_Season"), 7);
                    }
                    else
                    {
                        seasonsDefined.Add("spring");
                        seasonsDefined.Add("summer");
                        seasonsDefined.Add("winter");
                        seasonsDefined.Add("fall");
                    }
                    this.Monitor.Log(formattedShopkeep, LogLevel.Info);

                    if (currentShopMenu.portraitPerson.Equals(StardewValley.Game1.getCharacterFromName(formattedShopkeep, false)) && Game1.year >= yearDefined && seasonsDefined.Contains(Game1.currentSeason))
                    {
                        //The current shopkeep does! Now we need to get the list of what's being sold
                        IReflectedField<Dictionary<Item, int[]>> inventoryInformation = this.Helper.Reflection.GetField<Dictionary<Item, int[]>>(currentShopMenu, "itemPriceAndStock");
                        Dictionary<Item, int[]> itemPriceAndStock = inventoryInformation.GetValue();
                        IReflectedField<List<Item>> forSaleInformation = this.Helper.Reflection.GetField<List<Item>>(currentShopMenu, "forSale");
                        List<Item> forSale = forSaleInformation.GetValue();

                        //Now, lets add a few things...
                        int[,] itemsAndPrices = config.shopkeepers[shopkeep];
                        for (int index = 0; index < itemsAndPrices.GetLength(0); index++)
                        {
                            int itemId = itemsAndPrices[index, 0];
                            int price = itemsAndPrices[index, 1];
                            Item objectToAdd = (Item)new StardewValley.Object(Vector2.Zero, itemId, int.MaxValue);
                            itemPriceAndStock.Add(objectToAdd, new int[2] { price, int.MaxValue });
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