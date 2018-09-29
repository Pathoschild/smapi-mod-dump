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
        public int[] LikedTreats { get; set; }
        public int MinimumNumberOfExtraWool { get; set; }
        public int MaximumNumberOfExtraWool { get; set; }        
        public int MinimumDaysUtillBirth { get; set; }

        public SheepItem()
        {
            MinimalNumberOfMeat = 4;
            MaximumNumberOfMeat = 16;
            MinimumDaysBetweenTreats = 3;
            LikedTreats = new int[] { 78, 250, 280 };
            MinimumNumberOfExtraWool = 0;
            MaximumNumberOfExtraWool = 2;
            MinimumDaysUtillBirth = 8;
        }
    }
}
