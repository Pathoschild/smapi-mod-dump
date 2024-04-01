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
    public class SheepItem : AnimalItem, TreatItem, WoolAnimalItem, ImpregnatableAnimalItem
    {
        public int MinimalNumberOfMeat { get; set; }
        public int MaximumNumberOfMeat { get; set; }
        public int MinimumDaysBetweenTreats { get; set; }
        public object[] LikedTreats { get; set; }
        public ISet<string> LikedTreatsId { get; set; }
        public int MinimumNumberOfExtraWool { get; set; }
        public int MaximumNumberOfExtraWool { get; set; }        
        public int? MinimumDaysUtillBirth { get; set; }
        public bool CanUseDeluxeItemForPregnancy { get; set; }

        public SheepItem()
        {
            MinimalNumberOfMeat = 4;
            MaximumNumberOfMeat = 16;
            MinimumDaysBetweenTreats = 3;
            LikedTreats = new object[] { 78, 250, 280 };
            LikedTreatsId = new HashSet<string>();
            MinimumNumberOfExtraWool = 0;
            MaximumNumberOfExtraWool = 2;
            MinimumDaysUtillBirth = 8;
        }
    }
}
