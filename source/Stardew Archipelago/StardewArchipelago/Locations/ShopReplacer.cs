/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net.Models;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace StardewArchipelago.Locations
{
    public class ShopReplacer
    {
        private IMonitor _monitor;
        private IModHelper _modHelper;
        private ArchipelagoClient _archipelago;
        private LocationChecker _locationChecker;

        public ShopReplacer(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

        public void ReplaceShopItem(Dictionary<ISalable, int[]> itemPriceAndStock, ISalable itemOnSale, string apLocation, Func<Object, bool> conditionToMeet, Hint[] myActiveHints)
        {
            if (itemOnSale is not Object salableObject || !conditionToMeet(salableObject))
            {
                return;
            }
            
            var apName = BigCraftable.ConvertToApName(salableObject);
            var shouldRemoveOriginal = !_archipelago.HasReceivedItem(apName);

            ReplaceShopItem(itemPriceAndStock, itemOnSale, apLocation, shouldRemoveOriginal, myActiveHints);
        }

        public void ReplaceShopItem(Dictionary<ISalable, int[]> itemPriceAndStock, ISalable itemOnSale, string apLocation, Func<Furniture, bool> conditionToMeet, Hint[] myActiveHints)
        {
            if (itemOnSale is not Furniture salableFurniture || !conditionToMeet(salableFurniture))
            {
                return;
            }

            ReplaceShopItem(itemPriceAndStock, itemOnSale, apLocation, true, myActiveHints);
        }

        public void ReplaceShopItem(Dictionary<ISalable, int[]> itemPriceAndStock, ISalable itemOnSale, string apLocation, Func<Hat, bool> conditionToMeet, Hint[] myActiveHints)
        {
            if (itemOnSale is not Hat salableHat || !conditionToMeet(salableHat))
            {
                return;
            }

            ReplaceShopItem(itemPriceAndStock, itemOnSale, apLocation, true, myActiveHints);
        }

        private void ReplaceShopItem(Dictionary<ISalable, int[]> itemPriceAndStock, ISalable itemOnSale, string apLocationName, bool removeOriginal, Hint[] myActiveHints)
        {
            var itemPrice = itemPriceAndStock[itemOnSale][0];
            if (removeOriginal)
            {
                itemPriceAndStock.Remove(itemOnSale);
            }

            if (_locationChecker.IsLocationChecked(apLocationName))
            {
                return;
            }

            if (itemPriceAndStock.Keys.Any(x => (x is PurchaseableArchipelagoLocation apLocation) && apLocation.ApLocationName.Equals(apLocationName)))
            {
                return;
            }

            var purchaseableLocation = new PurchaseableArchipelagoLocation(apLocationName, apLocationName, _modHelper, _locationChecker, _archipelago, myActiveHints);
            itemPriceAndStock.Add(purchaseableLocation, new[] { itemPrice, 1 });
        }

        private bool IsRarecrow(Object item)
        {
            return item.IsScarecrow() &&
                   item.Name == "Rarecrow";
        }

        public bool IsRarecrow(Object item, int rarecrowNumber)
        {
            if (!IsRarecrow(item))
            {
                return false;
            }

            return rarecrowNumber switch
            {
                1 => item.ParentSheetIndex == 110,
                2 => item.ParentSheetIndex == 113,
                3 => item.ParentSheetIndex == 126,
                4 => item.ParentSheetIndex == 136,
                5 => item.ParentSheetIndex == 137,
                6 => item.ParentSheetIndex == 138,
                7 => item.ParentSheetIndex == 139,
                8 => item.ParentSheetIndex == 140,
                _ => false,
            };
        }
    }
}
