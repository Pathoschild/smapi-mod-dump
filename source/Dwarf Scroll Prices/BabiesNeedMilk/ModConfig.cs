/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/noriteway/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace BabiesNeedMilk
{
    public sealed class ModConfig
    {
        public int DaysUntilCalfWeaned { get; set; } = 3;
        public int DaysUntilKidWeaned { get; set; } = 3;
    }
}
