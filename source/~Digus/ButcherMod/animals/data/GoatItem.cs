using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHusbandryMod.animals.data
{
    public class GoatItem : AnimalItem, TreatItem, ImpregnatableAnimalItem
    {
        public int MinimalNumberOfMeat { get; set; }
        public int MaximumNumberOfMeat { get; set; }
        public int MinimumDaysBetweenTreats { get; set; }
        public int[] LikedTreats { get; set; }
        public int MinimumDaysUtillBirth { get; set; }

        public GoatItem()
        {
            MinimalNumberOfMeat = 3;
            MaximumNumberOfMeat = 8;
            MinimumDaysBetweenTreats = 4;
            LikedTreats = new int[] { 78, 398, 613, 274, 436 };
            MinimumDaysUtillBirth = 10;
        }
    }
}
