/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/legovader09/SDVLoanMod
**
*************************************************/

using StardewModdingAPI;
using static LoanMod.ModEntry;

namespace LoanMod
{
    internal class LoanMPMessage
    {
        public LoanManager LoanManager { get; set; }
        public long Peer { get; set; }
    }
}
