/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

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
