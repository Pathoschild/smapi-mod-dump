/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using MarketDay.API;
using MarketDay.Data;
using MarketDay.Utility;

namespace MarketDay.Shop
{
    /// <summary>
    /// This class represents each animal store
    /// </summary>
    public class AnimalShop : AnimalShopModel
    {

        private List<Object> _shopAnimalStock;
        private List<Object> _allAnimalsStock;

        internal static List<string> ExcludeFromMarnie = new List<string>();

        /// <summary>
        /// Translate what needs to be translated on game saved, in case of the language being changed
        /// </summary>
        public void UpdateTranslations()
        {
            ClosedMessage = Translations.Localize(ClosedMessage, LocalizedClosedMessage);
        }

        /// <summary>
        /// Updates the stock by grabbing the current data from getPurchaseAnimalStock and taking the info 
        /// for the animals that will be sold in this store
        /// </summary>
        private void UpdateShopAnimalStock()
        {
            //BFAV patches this anyways so it'll automatically work if installed
            _allAnimalsStock = StardewValley.Utility.getPurchaseAnimalStock();

            _shopAnimalStock = new List<Object>();
            foreach (var animal in _allAnimalsStock)
            {
                if (AnimalStock.Contains(animal.Name))
                {
                    _shopAnimalStock.Add(animal);
                }
            }
        }
        public void DisplayShop(bool debug = false)
        {
            //skip condition checking if called from console commands
            if (debug || APIs.Conditions.CheckConditions(When))
            {
                //get animal stock each time to refresh requirement checks
                UpdateShopAnimalStock();

                //sets variables I use to control hardcoded warps
                MarketDay.SourceLocation = Game1.currentLocation;
                Game1.activeClickableMenu = new PurchaseAnimalsMenu(_shopAnimalStock);
            }
            else if (ClosedMessage != null)
            {
                Game1.activeClickableMenu = new DialogueBox(ClosedMessage);
            }
        }
    }
}