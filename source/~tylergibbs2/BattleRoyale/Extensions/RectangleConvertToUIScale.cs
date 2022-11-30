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
    public static class RectangleConvertToUIScale
    {
        public static Rectangle ToUIScale(this Rectangle rect)
        {
            return new Rectangle(
                (int)Utility.ModifyCoordinateForUIScale(rect.X),
                (int)Utility.ModifyCoordinateForUIScale(rect.Y),
                (int)Utility.ModifyCoordinateForUIScale(rect.Width),
                (int)Utility.ModifyCoordinateForUIScale(rect.Height)
            );
        }
    }
}
