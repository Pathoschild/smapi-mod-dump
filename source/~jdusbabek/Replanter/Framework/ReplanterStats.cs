/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jdusbabek/stardewvalley
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewLib;

namespace Replanter.Framework
{
    internal class ReplanterStats : IStats
    {
        /*********
        ** Accessors
        *********/
        public int CropsHarvested { get; set; }
        public int RunningSeedCost { get; set; }
        public int RunningSellPrice { get; set; }
        public int FarmhandCost { get; set; }
        public int TotalCrops { get; set; }
        public int CropsWatered { get; set; }
        public int PlantsCleared { get; set; }

        public int TotalCost => this.FarmhandCost + this.RunningSeedCost;
        public int NumUnharvested => this.TotalCrops - this.CropsHarvested;


        /*********
        ** Public methods
        *********/
        public bool HasUnfinishedBusiness()
        {
            //int total = 0;
            return (1 < 0);
        }

        public IDictionary<string, object> GetFields()
        {
            IDictionary<string, object> fields = typeof(ReplanterStats)
                .GetProperties()
                .ToDictionary(p => p.Name, p => p.GetValue(this));
            fields["runningLaborCost"] = this.FarmhandCost;
            return fields;
        }
    }
}
