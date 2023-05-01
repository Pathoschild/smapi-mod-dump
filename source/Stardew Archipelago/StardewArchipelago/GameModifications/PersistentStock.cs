/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Collections.Generic;
using StardewValley;

namespace StardewArchipelago.GameModifications
{
    public class PersistentStock
    {
        private int _stockDay = -1;
        private Dictionary<ISalable, int[]> _stock;

        public PersistentStock()
        {
            _stockDay = -1;
            _stock = null;
        }

        public bool TryGetStockForToday(out Dictionary<ISalable, int[]> stock)
        {
            var today = Game1.stats.DaysPlayed;
            if (today != _stockDay || _stock == null)
            {
                stock = null;
                return false;
            }

            stock = _stock;
            return true;
        }

        public void SetStockForToday(Dictionary<ISalable, int[]> stock)
        {
            _stockDay = (int)Game1.stats.DaysPlayed;
            _stock = stock;
        }

        public void InvalidateStock()
        {
            _stockDay = -1;
            _stock = null;
        }

        public bool OnPurchase(ISalable item, Farmer farmer, int amount)
        {
            if (!_stock.ContainsKey(item))
            {
                return false;
            }

            // _stock[item][1] -= amount;
            return false;
        }
    }
}
