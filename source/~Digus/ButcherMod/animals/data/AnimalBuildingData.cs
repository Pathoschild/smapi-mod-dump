using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHusbandryMod.animals.data
{
    public class AnimalBuildingData
    {
        public int? BarnPregnancyLimit;
        public int? BigBarnPregnancyLimit;
        public int? DeluxeBarnPregnancyLimit;
        public int? CoopPregnancyLimit;
        public int? BigCoopPregnancyLimit;
        public int? DeluxeCoopPregnancyLimit;

        public AnimalBuildingData()
        {
            BarnPregnancyLimit = 0;
            BigBarnPregnancyLimit = 1;
            DeluxeBarnPregnancyLimit = 2;
            CoopPregnancyLimit = null;
            BigCoopPregnancyLimit = null;
            DeluxeCoopPregnancyLimit = null;
        }
    }
}
