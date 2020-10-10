/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace Entoarox.Framework.Extensions
{
    public static class PointExtensions
    {
        public static Vector2 ToVector2(this Point self)
        {
            return new Vector2(self.X, self.Y);
        }
    }
}
