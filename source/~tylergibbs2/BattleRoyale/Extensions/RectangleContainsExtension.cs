/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace BattleRoyale.Extensions
{
    public static class RectangleContainsExtension
    {
        public static bool Contains(this Rectangle bounds, DoorOrWarp warp)
        {
            return bounds.Contains(warp.Position);
        }
    }
}
