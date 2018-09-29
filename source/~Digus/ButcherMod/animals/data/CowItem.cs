using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHusbandryMod.animals.data
{
    public class CowItem : AnimalItem, TreatItem, ImpregnatableAnimalItem
    {
        public int MinimalNumberOfMeat { get; set; }
        public int MaximumNumberOfMeat { get; set; }
        public int MinimumDaysBetweenTreats { get; set; }
        public int[] LikedTreats { get; set; }
        public int MinimumDaysUtillBirth { get; set; }

        public CowItem()
        {
            MinimalNumberOfMeat = 5;
            MaximumNumberOfMeat = 20;
            MinimumDaysUtillBirth = 12;
            MinimumDaysBetweenTreats = 5;
            LikedTreats = new int[] { 78, 264, 300, 184 };
        }
    }
}
