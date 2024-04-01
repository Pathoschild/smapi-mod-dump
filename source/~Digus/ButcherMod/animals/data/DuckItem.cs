/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHusbandryMod.animals.data
{
    public class DuckItem : AnimalItem, TreatItem, FeatherAnimalItem
    {
        public int MinimalNumberOfMeat { get; set; }
        public int MaximumNumberOfMeat { get; set; }
        public int MinimumDaysBetweenTreats { get; set; }
        public object[] LikedTreats { get; set; }
        public ISet<string> LikedTreatsId { get; set; }
        public int MinimumNumberOfFeatherChances { get; set; }
        public int MaximumNumberOfFeatherChances { get; set; }

        public DuckItem()
        {
            MinimalNumberOfMeat = 1;
            MaximumNumberOfMeat = 5;
            MinimumDaysBetweenTreats = 4;
            LikedTreats = new object[] { 78, 278, 207 };
            LikedTreatsId = new HashSet<string>();
            MinimumNumberOfFeatherChances = 0;
            MaximumNumberOfFeatherChances = 2;
        }
    }
}
