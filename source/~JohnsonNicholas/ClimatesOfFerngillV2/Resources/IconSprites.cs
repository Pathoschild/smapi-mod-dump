using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace TwilightShards.ClimatesOfFerngillV2
{
    public static class Icons
    {
        /// <summary>The sprite sheet containing the icon sprites.</summary>
        public static Texture2D Sheet => Game1.mouseCursors;

        /// <summary>A down arrow for scrolling content.</summary>
        public static readonly Rectangle DownArrow = new Rectangle(12, 76, 40, 44);

        /// <summary>An up arrow for scrolling content.</summary>
        public static readonly Rectangle UpArrow = new Rectangle(76, 72, 40, 44);
    }
}
