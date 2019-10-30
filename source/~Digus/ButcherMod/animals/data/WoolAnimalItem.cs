using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHusbandryMod.animals.data
{
    public interface WoolAnimalItem : MeatAnimalItem
    {
        int MinimumNumberOfExtraWool { get; set; }
        int MaximumNumberOfExtraWool { get; set; }
    }
}
