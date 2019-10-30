using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHusbandryMod.animals.data
{
    public class DinosaurItem : AnimalItem, TreatItem
    {
        public int MinimumDaysBetweenTreats { get; set; }
        public object[] LikedTreats { get; set; }
        public ISet<int> LikedTreatsId { get; set; }

        public DinosaurItem()
        {
            MinimumDaysBetweenTreats = 4;
            LikedTreats = new object[] { 78, 18, 418 };
            LikedTreatsId = new HashSet<int>();
        }
    }
}
