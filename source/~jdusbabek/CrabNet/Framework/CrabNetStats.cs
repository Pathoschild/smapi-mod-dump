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

namespace CrabNet.Framework
{
    internal class CrabNetStats : IStats
    {
        /*********
        ** Accessors
        *********/
        // The total number of crab pots that are placed
        public int NumTotal { get; set; }

        // The number of crab pots that were successfully checked
        public int NumChecked { get; set; }

        // The number of crab pots that were successfully emptied
        public int NumEmptied { get; set; }

        // The number of crab pots that were successfully baited
        public int NumBaited { get; set; }

        // The number of crab pots that were completed.  Possibly deprecated, check for occurences and delete.
        public int NumCompleted { get; set; }

        // The number of crab pots that were traversed, but not checked.
        public int NotChecked { get; set; }

        // The number of crab pots that were not emptied.
        public int NotEmptied { get; set; }

        // The number of crab pots that were not baited.
        public int NotBaited { get; set; }

        // The number of crab pots that had nothing to retrieve.  Possibly deprecated, check for occurrences and delete.
        public int NothingToRetrieve { get; set; }

        // The number of crab pots that did not need to be baited.  Possibly deprecated, check for occurences and delete.
        public int NothingToBait { get; set; }

        // A running total of the costs, used to check for "can afford" while waiting to deduct costs until the end.
        public int RunningTotal { get; set; }


        /*********
        ** Public methods
        *********/
        // Whether all pots were checked, emptied, and baited.
        public bool HasUnfinishedBusiness()
        {
            int total = this.NumBaited + this.NothingToBait + this.NumEmptied + this.NothingToRetrieve + this.NumChecked;
            return total != (this.NumTotal * 3);
        }

        public IDictionary<string, object> GetFields()
        {
            IDictionary<string, object> fields = typeof(CrabNetStats)
                .GetProperties()
                .ToDictionary(p => p.Name, p => p.GetValue(this));

            // TODO: fix this bug (carried over from previous code)
            fields["numChecked"] = this.NumTotal;

            return fields;
        }
    }
}
