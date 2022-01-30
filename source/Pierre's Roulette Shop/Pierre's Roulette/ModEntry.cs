/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/xynerorias/pierre-roulette-shop-SV
**
*************************************************/

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using Object = StardewValley.Object;

namespace pierresroulette
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********** Properties ***********/

        /// <summary>The mod configuration from the player.</summary>
        private ModConfig config;


        /*********** Public methods ***********/

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.config = this.Helper.ReadConfig<ModConfig>();
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.Display.MenuChanged += this.MenuChanged;
        }

        /*********** Private methods ***********/

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is ShopMenu shopMenu)
            {
                string owner = null;
                owner = GetOwner(shopMenu);

                if (owner != null && config.Owners.Contains(owner) || (config.JojaEnabled && owner == "Joja"))
                {
                    int uniqueID = Game1.GetSaveGameName().GetHashCode();
                    Random rnd = new Random(Game1.Date.TotalDays + uniqueID + config.SeedStock + config.SaplingStock);

                    List<ISalable> forSale = shopMenu.forSale;
                    Dictionary<ISalable, int[]> itemPriceAndStock = shopMenu.itemPriceAndStock;
                    int seedMinStock = config.SeedStock;
                    int saplingMinStock = config.SaplingStock;

                    switch (config.Mode)
                    {
                        case "OnlySaplings": //Just the saplings

                            var saplingsToKeep = forSale.Where(x => x is Item item && item.Name.EndsWith("Sapling")).ToList();

                            if (saplingMinStock > 0)
                                KeepSaplings(rnd, saplingMinStock, saplingsToKeep);

                            var toRemove = forSale.Where(y => y is Item item && item.Name.EndsWith("Sapling") && !saplingsToKeep.Contains(item)).ToList();

                            RemoveStock(shopMenu, forSale, itemPriceAndStock, toRemove);

                            saplingsToKeep.Clear();

                            break;

                        case "OnlySeeds": //Just the crops

                            var seedsToKeep = forSale.Where(x => x is Item item && item.Category == Object.SeedsCategory && !item.Name.EndsWith("Sapling")).ToList();

                            if (seedMinStock > 0)
                                KeepSeeds(rnd, seedMinStock, seedsToKeep);

                            toRemove = forSale.Where(y => y is Item item && item.Category == Object.SeedsCategory && !item.Name.EndsWith("Sapling") && !seedsToKeep.Contains(item)).ToList();

                            RemoveStock(shopMenu, forSale, itemPriceAndStock, toRemove);

                            seedsToKeep.Clear();

                            break;

                        case "Both": //Saplings and crops but each have their seperate stock

                            seedsToKeep = forSale.Where(x => x is Item item && item.Category == Object.SeedsCategory & !item.Name.EndsWith("Sapling")).ToList();

                            if (seedMinStock > 0)
                                KeepSeeds(rnd, seedMinStock, seedsToKeep);

                            saplingsToKeep = forSale.Where(x => x is Item item && item.Category == Object.SeedsCategory & item.Name.EndsWith("Sapling")).ToList();

                            if (saplingMinStock > 0)
                                KeepSaplings(rnd, saplingMinStock, saplingsToKeep);

                            toRemove = forSale.Where(y => y is Item item && item.Category == Object.SeedsCategory && !seedsToKeep.Contains(item) && !saplingsToKeep.Contains(item)).ToList();

                            RemoveStock(shopMenu, forSale, itemPriceAndStock, toRemove);

                            seedsToKeep.Clear();
                            saplingsToKeep.Clear();

                            break;

                        default:
                            string error = string.Format("Defined '{0}' is not a valid mode ! Only acceptable cases are 'OnlySaplings', 'OnlyCrops' or 'Both'. Change this in the config. Check the mod page for more info.", (object)config.Mode);
                            throw new ArgumentException(error);
                    }
                }
                else
                    return;
            }
        }

        private static string GetOwner(ShopMenu shopMenu)
        {
            string owner;
            if (shopMenu.portraitPerson == null)
            {
                if (Game1.currentLocation.Name == "JojaMart")
                    owner = "Joja";
                else
                    owner = null; 
            }   
            else
                owner = shopMenu.portraitPerson.Name;
            return owner;
        }

        private static void RemoveStock(ShopMenu shopMenu, List<ISalable> forSale, Dictionary<ISalable, int[]> itemPriceAndStock, List<ISalable> toRemove)
        {
            forSale.RemoveAll(z => z is Item item && toRemove.Contains(item));

            foreach (ISalable item in toRemove)
                itemPriceAndStock.Remove(item);

            shopMenu.setItemPriceAndStock(itemPriceAndStock);

            toRemove.Clear();
        }

        private static void KeepSeeds(Random rnd, int seedMinStock, List<ISalable> seedsToKeep)
        {
            while (seedsToKeep.Count > seedMinStock)
            {
                int now = rnd.Next(0, seedsToKeep.Count);
                seedsToKeep.RemoveAt(now);
                if (seedsToKeep.Count == seedMinStock)
                    break;
            }
        }

        private static void KeepSaplings(Random rnd, int saplingMinStock, List<ISalable> saplingsToKeep)
        {
            while (saplingsToKeep.Count > saplingMinStock)
            {
                int now = rnd.Next(0, saplingsToKeep.Count);
                saplingsToKeep.RemoveAt(now);
                if (saplingsToKeep.Count == saplingMinStock)
                    break;
            }
        }
    }
}
