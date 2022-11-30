/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityUpgradeFramework
{
    internal class CommunityUpgrade
    {
        public string Location;
        public string Name;
        public string Description;
        public Dictionary<int, int> ItemPriceDict;
        public Dictionary<string, int> CurrencyPriceDict;
        public string ThumbnailPath;
    }
}
