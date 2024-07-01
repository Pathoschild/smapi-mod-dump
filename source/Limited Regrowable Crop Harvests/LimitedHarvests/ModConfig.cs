/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Adradis/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LimitedHarvests
{
    public sealed class ModConfig
    {
        public bool GentleJunimos { get; set; }
        public bool AllowOverride { get; set; }
        public string HarvestCountMethod { get; set; }

        public string RandomizationSetting { get; set; }
        public int UpperRandModifier { get; set; }
        public int LowerRandModifier { get; set; }
        public int BaseHarvests { get; set; }

        public ModConfig()
        {
            GentleJunimos = false;
            AllowOverride = true;
            HarvestCountMethod = "Harvests per Season";

            RandomizationSetting = "None";
            UpperRandModifier = 2;
            LowerRandModifier = 2;
            BaseHarvests = 5;
        }
     }
}
