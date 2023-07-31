/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unlockable_Bundles.Lib
{
    public class UnlockableSaveData
    {
        public bool Purchased = false;
        public int DayPurchased = -1;
        public Dictionary<string, int> Price = new Dictionary<string, int>();
        public Dictionary<string, int> AlreadyPaid = new Dictionary<string, int>();
        public Dictionary<string, int> AlreadyPaidIndex = new Dictionary<string, int>();
    }
}
