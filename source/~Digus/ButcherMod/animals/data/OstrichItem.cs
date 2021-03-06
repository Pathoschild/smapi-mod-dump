/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;

namespace AnimalHusbandryMod.animals.data
{
    public class OstrichItem : AnimalItem, MeatAnimalItem, TreatItem
    {
        public int MinimumDaysBetweenTreats { get; set; }
        public object[] LikedTreats { get; set; }
        public ISet<int> LikedTreatsId { get; set; }
        public int MinimalNumberOfMeat { get; set; }
        public int MaximumNumberOfMeat { get; set; }

        public OstrichItem()
        {
            MinimalNumberOfMeat = 1;
            MaximumNumberOfMeat = 1;
            MinimumDaysBetweenTreats = 3;
            LikedTreats = new object[] { 78, -75 };
            LikedTreatsId = new HashSet<int>();
        }
    }
}