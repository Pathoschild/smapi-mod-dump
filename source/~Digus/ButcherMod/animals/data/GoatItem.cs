using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHusbandryMod.animals.data
{
    public class GoatItem : AnimalItem, MeatAnimalItem, TreatItem, ImpregnatableAnimalItem
    {
        public int MinimalNumberOfMeat { get; set; }
        public int MaximumNumberOfMeat { get; set; }
        public int MinimumDaysBetweenTreats { get; set; }
        public object[] LikedTreats { get; set; }
        public ISet<int> LikedTreatsId { get; set; }
        public int? MinimumDaysUtillBirth { get; set; }
        public bool CanUseDeluxeItemForPregnancy { get; set; }

        public GoatItem()
        {
            MinimalNumberOfMeat = 3;
            MaximumNumberOfMeat = 8;
            MinimumDaysBetweenTreats = 4;
            LikedTreats = new object[] { 78, 398, 613, 274, 436 };
            LikedTreatsId = new HashSet<int>();
            MinimumDaysUtillBirth = 10;
            CanUseDeluxeItemForPregnancy = true;
        }
    }
}
