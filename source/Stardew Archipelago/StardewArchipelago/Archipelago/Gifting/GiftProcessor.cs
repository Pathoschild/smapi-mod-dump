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
using Archipelago.Gifting.Net.Gifts.Versions.Current;
using Archipelago.Gifting.Net.Traits;
using StardewArchipelago.Extensions;
using StardewArchipelago.Stardew;
using StardewModdingAPI;

namespace StardewArchipelago.Archipelago.Gifting
{
    public class GiftProcessor
    {
        private IMonitor _monitor;
        private ArchipelagoClient _archipelago;
        private StardewItemManager _itemManager;
        private Dictionary<string, Func<int, ItemAmount>> _specialItems;
        private Dictionary<int, Dictionary<string[], Func<int, Dictionary<string, GiftTrait>, ItemAmount>>> _recognizedTraits;

        public GiftProcessor(IMonitor monitor, ArchipelagoClient archipelago, StardewItemManager itemManager)
        {
            _monitor = monitor;
            _archipelago = archipelago;
            _itemManager = itemManager;
            InitializeSpecialItems();
            InitializeRecognizedTraits();
        }

        public bool TryMakeStardewItem(Gift gift, out string itemName, out int amount)
        {
            if (_itemManager.ObjectExists(gift.ItemName))
            {
                itemName = gift.ItemName;
                amount = gift.Amount;
                return true;
            }

            var capitalizedName = gift.ItemName.ToCapitalized();
            if (_specialItems.ContainsKey(capitalizedName))
            {
                var specialItem = _specialItems[capitalizedName](gift.Amount);
                itemName = specialItem.ItemName;
                amount = specialItem.Amount;
                return true;
            }

            foreach (var traitNumber in _recognizedTraits.Keys.OrderByDescending(x => x))
            {
                foreach (var (traits, itemFunction) in _recognizedTraits[traitNumber])
                {
                    if (traits.Any(x => !gift.Traits.Select(t => t.Trait).Contains(x)))
                    {
                        continue;
                    }

                    var traitsByName = gift.Traits.ToDictionary(t => t.Trait, t => t);
                    var itemAmount = itemFunction(gift.Amount, traitsByName);
                    itemName = itemAmount.ItemName;
                    amount = itemAmount.Amount;
                    return true;
                }
            }

            foreach (var trait in gift.Traits)
            {
                if (_itemManager.ObjectExists(trait.Trait))
                {
                    itemName = trait.Trait;
                    amount = (int)Math.Round(trait.Quality * gift.Amount);
                    return true;
                }
            }

            itemName = null;
            amount = 0;
            return false;
        }

        private void InitializeSpecialItems()
        {
            _specialItems = new Dictionary<string, Func<int, ItemAmount>>();
            _specialItems.Add("Tree", (amount) => ("Wood", amount * 15));
            _specialItems.Add("Lumber", (amount) => ("Hardwood", amount * 15));
            _specialItems.Add("Boulder", (amount) => ("Stone", amount * 15));
            _specialItems.Add("Rock", (amount) => ("Stone", amount * 2));
            _specialItems.Add("Vine", (amount) => ("Fiber", amount * 2));
        }

        private void InitializeRecognizedTraits()
        {
            _recognizedTraits = new Dictionary<int, Dictionary<string[], Func<int, Dictionary<string, GiftTrait>, ItemAmount>>>();
            InitializeSingleRecognizedTraits();
            InitializeDualRecognizedTraits();
            InitializeTrioRecognizedTraits();
        }

        private void InitializeTrioRecognizedTraits()
        {
            var trioRecognizedTraits = new Dictionary<string[], Func<int, Dictionary<string, GiftTrait>, ItemAmount>>();
            _recognizedTraits.Add(3, trioRecognizedTraits);
        }

        private void InitializeDualRecognizedTraits()
        {
            var dualRecognizedTraits = new Dictionary<string[], Func<int, Dictionary<string, GiftTrait>, ItemAmount>>();
            // Speed + Animal = Horse
            _recognizedTraits.Add(2, dualRecognizedTraits);
        }

        private void InitializeSingleRecognizedTraits()
        {
            var singleRecognizedTraits = new Dictionary<string[], Func<int, Dictionary<string, GiftTrait>, ItemAmount>>();

            singleRecognizedTraits.Add(new[] { GiftFlag.Speed }, MakeCoffee);
            singleRecognizedTraits.Add(new[] { "Fan" }, (amount, _) => ("Ornamental Fan", amount));

            _recognizedTraits.Add(1, singleRecognizedTraits);
        }

        private ItemAmount MakeCoffee(int amount, Dictionary<string, GiftTrait> traits)
        {
            var speedTrait = traits[GiftFlag.Speed];
            var totalSpeed = speedTrait.Duration + speedTrait.Quality - 1;
            return totalSpeed >= 3 ? ("Triple Shot Espresso", (int)Math.Round(amount * (totalSpeed / 3))) : ("Coffee", (int)Math.Round(amount * totalSpeed));
        }
    }
}
