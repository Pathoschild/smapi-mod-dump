/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Collections.Generic;
using BirbShared.Asset;

namespace BetterFestivalNotifications
{
    [AssetClass]
    internal class Assets
    {
        [AssetProperty("assets/festivalnotifications.json")]
        public Dictionary<string, List<int>> FestivalNotifications { get; set; }
    }
}
