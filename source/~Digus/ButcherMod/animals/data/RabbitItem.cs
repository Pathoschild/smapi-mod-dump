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
    public class RabbitItem : AnimalItem, TreatItem, WoolAnimalItem, ImpregnatableAnimalItem, FeetAnimalItem
    {
        public int MinimalNumberOfMeat { get; set; }
        public int MaximumNumberOfMeat { get; set; }
        public int MinimumDaysBetweenTreats { get; set; }
        public object[] LikedTreats { get; set; }
        public ISet<string> LikedTreatsId { get; set; }
        public int MinimumNumberOfExtraWool { get; set; }
        public int MaximumNumberOfExtraWool { get; set; }
        public int MinimumNumberOfFeetChances { get; set; }
        public int MaximumNumberOfFeetChances { get; set; }
        public int? MinimumDaysUtillBirth { get; set; }
        public bool CanUseDeluxeItemForPregnancy { get; set; }

        public RabbitItem()
        {

            MinimalNumberOfMeat = 1;
            MaximumNumberOfMeat = 4;
            MinimumDaysBetweenTreats = 4;
            LikedTreats = new object[] { 78, 190, 414, 266 };
            LikedTreatsId = new HashSet<string>();
            MinimumNumberOfExtraWool = 0;
            MaximumNumberOfExtraWool = 1;
            MinimumNumberOfFeetChances = 0;
            MaximumNumberOfFeetChances = 4;
            MinimumDaysUtillBirth = 9;
        }    
    }
}
