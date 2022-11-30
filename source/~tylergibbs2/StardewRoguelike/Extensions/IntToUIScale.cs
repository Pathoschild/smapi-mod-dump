/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace StardewRoguelike.Extensions
{
    public static class IntToUIScale
    {
        public static int ToUIScale(this int i)
        {
            return (int)Utility.ModifyCoordinateForUIScale(i);
        }
    }
}