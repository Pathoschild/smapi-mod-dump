/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/xynerorias/Seed-Shortage
**
*************************************************/

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using JsonAssets;

namespace SeedShortage
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : StardewModdingAPI.Mod
    {
        private ModConfig config;
        private List<int> exclusions = new List<int>();

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<ModConfig>();
            
            helper.Events.Input.ButtonPressed += InputButtonPressed;
            helper.Events.GameLoop.DayStarted += GameLoopDayStarted;
            helper.Events.Display.MenuChanged += MenuChanged;
            helper.Events.GameLoop.SaveLoaded += SaveLoaded;
            helper.Events.GameLoop.ReturnedToTitle += ReturnedToTitle;
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void InputButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
        }
        
        /// <summary>Raised before/after the game reads data from a save file and initialises the world (including when day one starts on a new save).</summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event data</param>
        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            Monitor.Log("Grabbing IDs for the exceptions...", LogLevel.Debug);

            IApi api = Helper.ModRegistry.GetApi<IApi>("spacechase0.JsonAssets");
            Dictionary<string,int> VanillaSeeds = new Dictionary<string, int>(ID.Dict);
            List<string> exceptions = new List<string>(config.CropExceptions.ToList());
            var NewList = exceptions.Where(s => VanillaSeeds.ContainsKey(s)).ToList();
            var dict = VanillaSeeds.Where(kvp => NewList.Contains(kvp.Key)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            this.exclusions = new List<int>(dict.Values);

            foreach (string item in NewList)
                exceptions.Remove(item);

            if(api != null)
            foreach (string ex in exceptions)
                this.exclusions.Add(api.GetObjectId(ex));

            string log = string.Join(",", this.exclusions.ToArray());
            Monitor.Log("IDs marked as exception: " + log, LogLevel.Trace);

            Monitor.Log("All IDs grabbed !", LogLevel.Debug);
        }

        /// <summary>Raised after a new in-game day starts. Everything has already been initialised at this point.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void GameLoopDayStarted(object sender, DayStartedEventArgs e)
        {
            if (!Game1.player.craftingRecipes.Keys.Contains("Seed Maker"))
            {
                Monitor.Log("Adding Seed Maker recipe", LogLevel.Trace);
                Game1.player.craftingRecipes.Add("Seed Maker", 0);
            }
            if (Game1.player.farmingLevel >= 0)
                Helper.Events.GameLoop.DayStarted -= GameLoopDayStarted;
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is ShopMenu shopMenu)
            {
                string shopOwner = null;

                bool hatmouse = shopMenu != null && shopMenu.potraitPersonDialogue == Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11494"), Game1.dialogueFont, Game1.tileSize * 5 - Game1.pixelZoom * 4);
                bool magicboat = shopMenu != null && shopMenu.potraitPersonDialogue == Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.magicBoat"), Game1.dialogueFont, Game1.tileSize * 5 - Game1.pixelZoom * 4);
                bool travelnight = shopMenu != null && shopMenu.potraitPersonDialogue == Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.travelernightmarket"), Game1.dialogueFont, Game1.tileSize * 5 - Game1.pixelZoom * 4);

                if (shopMenu.portraitPerson != null)
                    shopOwner = shopMenu.portraitPerson.Name;
                if (shopMenu.portraitPerson == null && Game1.currentLocation.Name == "Hospital")
                    shopOwner = "Harvey";
                if ((shopMenu.portraitPerson == null && Game1.currentLocation.Name == "Forest" && !hatmouse) || (shopMenu.portraitPerson == null && travelnight))
                    shopOwner = "Travelling Merchant";
                if (Game1.currentLocation.Name == "JojaMart")
                    shopOwner = "Joja";
                if (magicboat)
                    shopOwner = "Magic Boat";
                if (Game1.currentLocation.Name == "WizardHouse")
                    shopOwner = "Wizard";
                if (shopMenu.portraitPerson == null && Game1.currentLocation.Name == "SeedShop")
                    shopOwner = "Seed Catalogue";

                if ((shopMenu.portraitPerson == null && shopOwner == null && !hatmouse) || hatmouse)
                    return;

                Dictionary<ISalable, int[]> itemPriceAndStock = shopMenu.itemPriceAndStock;
                List<ISalable> forSale = shopMenu.forSale;

                List<string> vendors2 = new List<string>(config.VendorsPrice);
                if (vendors2.Contains(shopOwner))
                {
                    List<ISalable> seeds = itemPriceAndStock.Keys.Where(item =>
                        item is StardewValley.Object obj
                        && obj.Category == StardewValley.Object.SeedsCategory
                        && !exclusions.Contains(obj.ParentSheetIndex)
                        && !item.Name.EndsWith("Sapling")).ToList();
                    foreach(ISalable item in seeds)
                    {
                        int[] array = itemPriceAndStock[item];
                        int price = item.salePrice();
                        array[0] = this.NewPrice(price);

                        string PricesUpdated = string.Format("{0} price increased by {1} at {2}", (object)item.Name , (object)config.PriceIncrease, (object)shopOwner);
                        Monitor.Log(PricesUpdated, LogLevel.Trace);
                    }
                    shopMenu.setItemPriceAndStock(itemPriceAndStock);
                }

                List<string> vendors = new List<string>(config.VendorsWithoutSeeds);
                if (vendors.Contains(shopOwner))
                {
                    forSale.RemoveAll((ISalable sale) =>
                        sale is Item item
                        && item.Category == StardewValley.Object.SeedsCategory
                        && !item.Name.EndsWith("Sapling")
                        && !item.Name.Equals(config.CropExceptions));

                    List<ISalable> seeds = itemPriceAndStock.Keys.Where(item =>
                        item is StardewValley.Object obj
                        && obj.Category == StardewValley.Object.SeedsCategory
                        && !exclusions.Contains(obj.ParentSheetIndex)
                        && !item.Name.EndsWith("Sapling")).ToList();

                    foreach (ISalable item in seeds)
                        itemPriceAndStock.Remove(item);
                    shopMenu.setItemPriceAndStock(itemPriceAndStock);

                    if (config.CropExceptions != null)
                    {
                        string ex = string.Format("{0}", string.Join(", ", config.CropExceptions));
                        Monitor.Log(string.Format("Seeds removed from {0}, except for {1}.", (object)shopOwner, (object)ex, (LogLevel.Trace)));
                    }
                    else
                        Monitor.Log(string.Format("Seeds removed from {0}.", (object)shopOwner, (LogLevel.Trace)));
                }
                else return;
            }
        }
        /// <summary>Raised after the game returns to the title screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void ReturnedToTitle (object sender, ReturnedToTitleEventArgs e)
        {
            this.exclusions.Clear();
        }
        private int NewPrice (int price)
        {
            string increase = config.PriceIncrease;
            if (increase.EndsWith("%"))
            {
                int num = int.Parse(increase.Substring(0, increase.Length - 1));
                int newprice = (int)((double)(100 + num) / 100 * (double)price);
                return newprice;
            }
            if (increase.EndsWith("x"))
            {
                int num = int.Parse(increase.Substring(0, increase.Length - 1));
                int newprice = price * num;
                return newprice;
            }
            else
            {
                string err = string.Format("{0} is not a valid increment value. Use either {0}% or {0}x in the config.", (object)config.PriceIncrease);
                Monitor.Log(err, LogLevel.Error);
                throw new ArgumentException(err);
            }
        }
    }
}