using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHusbandryMod.animals.data
{
    public class DuckItem : AnimalItem, TreatItem, FeatherAnimalItem
    {
        public int MinimalNumberOfMeat { get; set; }
        public int MaximumNumberOfMeat { get; set; }
        public int MinimumDaysBetweenTreats { get; set; }
        public object[] LikedTreats { get; set; }
        public ISet<int> LikedTreatsId { get; set; }
        public int MinimumNumberOfFeatherChances { get; set; }
        public int MaximumNumberOfFeatherChances { get; set; }

        public DuckItem()
        {
            MinimalNumberOfMeat = 2;
            MaximumNumberOfMeat = 6;
            MinimumDaysBetweenTreats = 3;
            LikedTreats = new object[] { 78, 278, 207 };
            LikedTreatsId = new HashSet<int>();
            MinimumNumberOfFeatherChances = 0;
            MaximumNumberOfFeatherChances = 2;
        }
    }
}
