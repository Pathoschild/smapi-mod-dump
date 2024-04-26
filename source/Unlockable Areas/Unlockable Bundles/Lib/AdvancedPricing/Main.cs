/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Unlockable_Bundles.ModEntry;


namespace Unlockable_Bundles.Lib.AdvancedPricing
{
    public class Main
    {
        public static void Initialize()
        {
            AdvancedPricingItem.Initialize();
        }
    }
}
