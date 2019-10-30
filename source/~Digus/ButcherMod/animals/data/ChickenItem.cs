using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHusbandryMod.animals.data
{
    public class ChickenItem : AnimalItem, MeatAnimalItem, TreatItem
    {
        public int MinimalNumberOfMeat { get; set; }
        public int MaximumNumberOfMeat { get; set; }
        public int MinimumDaysBetweenTreats { get; set; }
        public object[] LikedTreats { get ; set ; }
        public ISet<int> LikedTreatsId { get; set; }

        public ChickenItem()
        {
            MinimalNumberOfMeat = 1;
            MaximumNumberOfMeat = 4;
            MinimumDaysBetweenTreats = 4;
            LikedTreats = new object[] {78, 262, 270};
            LikedTreatsId = new HashSet<int>();
        }
    }
}
