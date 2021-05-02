/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/GreenhouseGatherers
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseGatherers.GreenhouseGatherers.Models
{
    public class SaveData
    {
        public List<HarvestStatueData> SavedStatueData { get; set; }

        public SaveData()
        {
            SavedStatueData = new List<HarvestStatueData>();
        }

        public SaveData(List<HarvestStatueData> savedStatueData)
        {
            SavedStatueData = savedStatueData;
        }
    }
}
