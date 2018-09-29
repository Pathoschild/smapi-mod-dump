using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHusbandryMod.animals.data
{
    public interface FeetAnimalItem
    {
        int MinimumNumberOfFeetChances { get; set; }
        int MaximumNumberOfFeetChances { get; set; }
    }
}
