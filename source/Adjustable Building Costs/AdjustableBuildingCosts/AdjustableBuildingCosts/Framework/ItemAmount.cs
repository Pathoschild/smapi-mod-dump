/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MiguelLucas/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdjustableBuildingCosts.Framework
{
    class ItemAmount
    {
        public int ItemID { get; set; } = 300;
        public int Amount { get; set; } = 1000;

        public ItemAmount(int ItemID, int Amount)
        {
            this.ItemID = ItemID;
            this.Amount = Amount;
        }
    }
}
