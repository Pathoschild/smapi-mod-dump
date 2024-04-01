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
using Newtonsoft.Json;

namespace AnimalHusbandryMod.animals.data
{
    public class CustomAnimalItem : AnimalItem, MeatAnimalItem, TreatItem, ImpregnatableAnimalItem
    {
        public String Name { get; set; }
        public int MinimalNumberOfMeat { get; set; }
        public int MaximumNumberOfMeat { get; set; }
        public int MinimumDaysBetweenTreats { get; set; }
        public object[] LikedTreats { get; set; }
        public ISet<string> LikedTreatsId { get; set; }
        public int? MinimumDaysUtillBirth { get; set; }
        public bool CanUseDeluxeItemForPregnancy { get; set; }

        public CustomAnimalItem(string name)
        {
            Name = name;
            MinimumDaysBetweenTreats = 4;
            LikedTreats = new object[] { 78 };
            LikedTreatsId = new HashSet<string>();
        }
    }
}
