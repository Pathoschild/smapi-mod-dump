using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHusbandryMod.animals.data
{
    public interface ImpregnatableAnimalItem 
    {
        int? MinimumDaysUtillBirth { get; set; }
        bool CanUseDeluxeItemForPregnancy { get; set; }
    }
}
