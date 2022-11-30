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
using StardewValley;

namespace BattleRoyale.Extensions
{
    public static class Vector2ConvertToUIScale
    {
        public static Vector2 ToUIScale(this Vector2 v2)
        {
            return new Vector2(
                (int)Utility.ModifyCoordinateForUIScale(v2.X),
                (int)Utility.ModifyCoordinateForUIScale(v2.Y)
            );
        }
    }
}
