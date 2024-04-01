/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using StardewValley;
using System.Security.Policy;

namespace ObjectProductDisplay
{
    public partial class ModEntry
    {
        private static float GetDoneFraction(Object instance)
        {
            if (!Config.ShowProgress || !instance.modData.TryGetValue(modKey, out string total))
                return Config.ShowProgressing ? Game1.ticks / 30 % 16 / 16f : 1f;
            return 1 - (float)instance.MinutesUntilReady / int.Parse(total);
        }
    }
}