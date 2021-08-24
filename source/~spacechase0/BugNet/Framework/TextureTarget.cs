/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BugNet.Framework
{
    /// <summary>A pixel area within a texture.</summary>
    internal class TextureTarget
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The texture instance.</summary>
        public Texture2D Texture { get; }

        /// <summary>The pixel area within the <see cref="Texture"/>.</summary>
        public Rectangle SourceRect { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="texture">The texture instance.</param>
        /// <param name="sourceRect">The pixel area within the <paramref name="texture"/>.</param>
        public TextureTarget(Texture2D texture, Rectangle sourceRect)
        {
            this.Texture = texture;
            this.SourceRect = sourceRect;
        }
    }
}
