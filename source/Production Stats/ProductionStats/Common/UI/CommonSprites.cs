/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlameHorizon/ProductionStats
**
*************************************************/

// derived from code by Jesse Plamondon-Willard under MIT license: https://github.com/Pathoschild/StardewMods

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ProductionStats.Common.UI
{
    internal static class CommonSprites
    {
        public static class Icons
        {
            /// <summary>The sprite sheet containing the icon sprites.</summary>
            public static Texture2D Sheet => Game1.mouseCursors;

            /// <summary>A down arrow.</summary>
            public static readonly Rectangle DownArrow = new(12, 76, 40, 44);

            /// <summary>An up arrow.</summary>
            public static readonly Rectangle UpArrow = new(76, 72, 40, 44);
        }
    }
}