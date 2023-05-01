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
using StardewArchipelago.Archipelago;
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

        public void ReplaceShopItem(Dictionary<ISalable, int[]> itemPriceAndStock, ISalable itemOnSale, string apLocation, Func<Object, bool> conditionToMeet)
        {
            if (itemOnSale is not Object salableObject || !conditionToMeet(salableObject))
            {
                return;
            }

            ReplaceShopItem(itemPriceAndStock, itemOnSale, apLocation, salableObject);
        }

        public void ReplaceShopItem(Dictionary<ISalable, int[]> itemPriceAndStock, ISalable itemOnSale, string apLocation, Func<Furniture, bool> conditionToMeet)
        {
            if (itemOnSale is not Furniture salableFurniture || !conditionToMeet(salableFurniture))
            {
                return;
            }

            ReplaceShopItem(itemPriceAndStock, itemOnSale, apLocation, itemOnSale);
        }

        public void ReplaceShopItem(Dictionary<ISalable, int[]> itemPriceAndStock, ISalable itemOnSale, string apLocation, Func<Hat, bool> conditionToMeet)
        {
            if (itemOnSale is not Hat salableHat || !conditionToMeet(salableHat))
            {
                return;
            }

            ReplaceShopItem(itemPriceAndStock, itemOnSale, apLocation, itemOnSale);
        }

        private void ReplaceShopItem(Dictionary<ISalable, int[]> itemPriceAndStock, ISalable itemOnSale, string apLocation, ISalable salableObject)
        {
            var itemPrice = itemPriceAndStock[itemOnSale][0];
            itemPriceAndStock.Remove(itemOnSale);
            if (_locationChecker.IsLocationChecked(apLocation))
            {
                return;
            }

            var purchaseableLocation =
                new PurchaseableArchipelagoLocation(apLocation, apLocation, _modHelper, _locationChecker,
                    _archipelago);
            itemPriceAndStock.Add(purchaseableLocation, new[] { itemPrice, 1 });
        }

        public bool IsRarecrow(Object item, int rarecrowNumber)
        {
            return item.IsScarecrow() &&
                   item.Name == "Rarecrow" &&
                   item.getDescription().Contains($"{rarecrowNumber} of");
        }
    }
}
