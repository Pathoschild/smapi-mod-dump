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

        public void ReplaceShopItem(Dictionary<ISalable, int[]> itemPriceAndStock, ISalable itemOnSale, string apLocation, Func<Object, bool> conditionToMeet)
        {
            if (itemOnSale is not Object salableObject || !conditionToMeet(salableObject))
            {
                return;
            }

            var shouldRemoveOriginal = true;
            if (IsRarecrow(salableObject))
            {
                var apName = BigCraftable.ConvertToRarecrowAPName(salableObject.Name, salableObject.getDescription());
                shouldRemoveOriginal = !_archipelago.HasReceivedItem(apName);
            }
            ReplaceShopItem(itemPriceAndStock, itemOnSale, apLocation, shouldRemoveOriginal);
        }

        public void ReplaceShopItem(Dictionary<ISalable, int[]> itemPriceAndStock, ISalable itemOnSale, string apLocation, Func<Furniture, bool> conditionToMeet)
        {
            if (itemOnSale is not Furniture salableFurniture || !conditionToMeet(salableFurniture))
            {
                return;
            }

            ReplaceShopItem(itemPriceAndStock, itemOnSale, apLocation, true);
        }

        public void ReplaceShopItem(Dictionary<ISalable, int[]> itemPriceAndStock, ISalable itemOnSale, string apLocation, Func<Hat, bool> conditionToMeet)
        {
            if (itemOnSale is not Hat salableHat || !conditionToMeet(salableHat))
            {
                return;
            }

            ReplaceShopItem(itemPriceAndStock, itemOnSale, apLocation, true);
        }

        private void ReplaceShopItem(Dictionary<ISalable, int[]> itemPriceAndStock, ISalable itemOnSale, string apLocation, bool removeOriginal)
        {
            var itemPrice = itemPriceAndStock[itemOnSale][0];
            if (removeOriginal)
            {
                itemPriceAndStock.Remove(itemOnSale);
            }
            if (_locationChecker.IsLocationChecked(apLocation))
            {
                return;
            }

            var purchaseableLocation =
                new PurchaseableArchipelagoLocation(apLocation, apLocation, _modHelper, _locationChecker,
                    _archipelago);
            itemPriceAndStock.Add(purchaseableLocation, new[] { itemPrice, 1 });
        }

        private bool IsRarecrow(Object item)
        {
            return item.IsScarecrow() &&
                   item.Name == "Rarecrow";
        }

        public bool IsRarecrow(Object item, int rarecrowNumber)
        {
            return IsRarecrow(item) &&
                   item.getDescription().Contains($"{rarecrowNumber} of");
        }
    }
}
