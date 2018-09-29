using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHusbandryMod.animals.data
{
    public interface TreatItem
    {
        int MinimumDaysBetweenTreats { get; set; }
        int[] LikedTreats { get; set; }
    }
}
