using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHusbandryMod.animals.data
{
    public class PigItem : AnimalItem, TreatItem, ImpregnatableAnimalItem
    {
        public int MinimalNumberOfMeat { get; set; }
        public int MaximumNumberOfMeat { get; set; }
        public int MinimumDaysBetweenTreats { get; set; }
        public int[] LikedTreats { get; set; }
        public int MinimumDaysUtillBirth { get; set; }

        public PigItem()
        {
            MinimalNumberOfMeat = 4;
            MaximumNumberOfMeat = 16;
            MinimumDaysBetweenTreats = 3;
            LikedTreats = new int[] { 78, 254, 276 };
            MinimumDaysUtillBirth = 18;
        }
    }
}
