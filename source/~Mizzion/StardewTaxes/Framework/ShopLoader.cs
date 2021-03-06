/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

namespace StardewTaxes.Framework
{
    internal class ShopLoader : Mod
    {
        /*
         *
         * Public Methods
         *
         */
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += DayStarted;
        }

        /*
         *
         *Private Methods
         *
         */

        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            IDictionary<ISalable, int[]> PierresShop = (Game1.getLocationFromName("SeedShop") as SeedShop)?.shopStock();

            foreach (var s in PierresShop.ToDictionary())
            {

            }
        }
    }
}
