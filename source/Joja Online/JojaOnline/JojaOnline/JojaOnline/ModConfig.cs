/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/JojaOnline
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JojaOnline.JojaOnline
{
    public class ModConfig
    {
        public bool areAllSeedsAvailableBeforeYearOne { get; set; }
        public bool giveJojaMemberDiscount { get; set; }
        public bool giveJojaPrimeShipping { get; set; }
        public int minSalePercentage { get; set; }
        public int maxSalePercentage { get; set; }

        public Dictionary<string, int> itemNameToPriceOverrides { get; set; }

        public ModConfig()
        {
            this.areAllSeedsAvailableBeforeYearOne = false;
            this.giveJojaMemberDiscount = false;
            this.giveJojaPrimeShipping = false;

            this.minSalePercentage = 5;
            this.maxSalePercentage = 35;

            this.itemNameToPriceOverrides = new Dictionary<string, int>();
        }
    }
}
