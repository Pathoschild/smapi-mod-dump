/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewGamePlus
{
    internal class Configuration
    {
        public bool StockList { get; set; } = true;
        public bool GingerIsland { get; set; } = true;
        public float AdditionalProfitMultiplier { get; set; } = 0.5f;
        public float RelationshipPenaltyPercentage { get; set; } = 0.5f;
        public float ExpCurveExponent { get; set; } = 1.10f;
        public int StartingPoints { get; set; } = 25;
        public int GoldPerLeftoverPoint { get; set; } = 500;
    }
}
