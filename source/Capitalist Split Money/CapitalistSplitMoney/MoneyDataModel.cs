/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/CapitalistSplitMoney
**
*************************************************/

using System.Collections.Generic;

namespace CapitalistSplitMoney
{
    public class MoneyDataModel
    {
        public Dictionary<long, int> PlayerMoney { get; set; } = new Dictionary<long, int>();
    }
}
