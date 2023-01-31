/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using StardewValley;
using System.Numerics;

namespace Moolah
{
    public class MoolahAPI : IMoolahAPI
    {
        public BigInteger GetTotalMoolah(Farmer f)
        {
            return ModEntry.GetTotalMoolah(f);
        }
        public void AddMoolah(Farmer f, BigInteger add)
        {
            BigInteger total = ModEntry.GetTotalMoolah(f) + add;
            f._money = ModEntry.AdjustMoney(f, total);
        }
    }
    public interface IMoolahAPI
    {
        public BigInteger GetTotalMoolah(Farmer f);
        public void AddMoolah(Farmer f, BigInteger add);
    }
}