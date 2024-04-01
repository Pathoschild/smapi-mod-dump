/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;

namespace PPJAValleyMarket.Shops
{
    class ZamirShop : MarketShop
    {
        public override string Shopkeeper { get; } = "Zamir";
        public override string[] JAPacks { get; } = new []{""};
        public override Dictionary<ISalable, int[]> ItemStockAndPrice { get; set; }

        public ZamirShop(IModHelper helper, IMonitor monitor) : base(helper, monitor)
        {
            
        }
        public override bool CanOpen()
        {
            //only open monday to fri
            var days = new[] {"Mon","Tues","Wed", "Thurs","Fri" };
            return days.Contains(Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth));
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }

    }
}
