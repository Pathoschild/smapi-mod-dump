using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHusbandryMod.animals.data
{
    public class PetItem : TreatItem
    {
        public int MinimumDaysBetweenTreats { get; set; }
        public object[] LikedTreats { get; set; }
        public ISet<int> LikedTreatsId { get; set; }

        public PetItem()
        {
            MinimumDaysBetweenTreats = 1;
            LikedTreats = new object[] { 130, 639, 136 };
            LikedTreatsId = new HashSet<int>();
        }
    }
}
