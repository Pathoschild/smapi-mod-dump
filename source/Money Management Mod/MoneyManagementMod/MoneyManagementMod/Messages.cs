/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tbonetomtom/StardewMods
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManagementMod
{
    public class Messages
    {
        public int PublicBal { get; set; }
        public int TaxPercentile { get; set; }
        public int TransferAmount { get; set; }
        public string? TransferType { get; set; }
        public long PlayerID { get; set; }
    }
}
