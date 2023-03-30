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
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

// Code from Pathoschild, MIT license
// https://github.com/Pathoschild/StardewMods

namespace Circuit.UI
{
    internal class CommonSprites
    {
        /// <summary>Sprites used to draw a dropdown list.</summary>
        public static class DropDown
        {
            /// <summary>The sprite sheet containing the menu sprites.</summary>
            public static readonly Texture2D Sheet = Game1.mouseCursors;

            /// <summary>The background for the selected item.</summary>
            public static readonly Rectangle ActiveBackground = new(258, 258, 4, 4);

            /// <summary>The background for a non-selected, non-hovered item.</summary>
            public static readonly Rectangle InactiveBackground = new(269, 258, 4, 4);

            /// <summary>The background for an item under the cursor.</summary>
            public static readonly Rectangle HoverBackground = new(161, 340, 4, 4);
        }

        /// <summary>Sprites used to draw icons.</summary>
        public static class Icons
        {
            /// <summary>The sprite sheet containing the icon sprites.</summary>
            public static Texture2D Sheet => Game1.mouseCursors;

            /// <summary>A down arrow.</summary>
            public static readonly Rectangle DownArrow = new(12, 76, 40, 44);

            /// <summary>An up arrow.</summary>
            public static readonly Rectangle UpArrow = new(76, 72, 40, 44);
        }

        /// <summary>Sprites used to draw a tab.</summary>
        public static class Tab
        {
            /// <summary>The sprite sheet containing the tab sprites.</summary>
            public static readonly Texture2D Sheet = Game1.mouseCursors;

            /// <summary>The top-left corner.</summary>
            public static readonly Rectangle TopLeft = new(0, 384, 5, 5);
        }
    }
}
