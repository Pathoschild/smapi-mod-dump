using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHusbandryMod.animals.data
{
    public class PregnancyItem
    {
        public long Id;
        public int DaysUntilBirth;
        public bool AllowReproductionBeforeInsemination;

        public PregnancyItem(long id, int daysUntilBirth, bool allowReproductionBeforeInsemination)
        {
            this.Id = id;
            this.DaysUntilBirth = daysUntilBirth;
            this.AllowReproductionBeforeInsemination = allowReproductionBeforeInsemination;
        }
    }
}
